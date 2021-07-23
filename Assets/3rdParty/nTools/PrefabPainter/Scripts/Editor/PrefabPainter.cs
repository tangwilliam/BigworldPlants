    
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;


namespace nTools.PrefabPainter
{
    //
	// class PrefabPainter
	//

    public partial class PrefabPainter : EditorWindow
	{

        public struct RaycastInfo
        {       
            public Ray     ray;  
            public bool    isHitTargetLayer;
            public bool    isHitMaskedLayer;
            public Vector3 point;
            public Vector3 normal;
            public Vector3 localPoint;
            public Vector3 localNormal;
            public Vector2 textureCoord;
            public Vector2 barycentricCoordinate;
            public float   distance;
            public int     triangleIndex;

            public bool isHit { get { return isHitTargetLayer || isHitMaskedLayer; } }

            public bool IntersectsHitPlane(Ray ray, out Vector3 hitPoint)
            {
                float rayDistance;
                Plane plane = new Plane (normal, point);
                if (plane.Raycast (ray, out rayDistance))
                {
                    hitPoint = ray.GetPoint (rayDistance);
                    return true;
                }
                hitPoint = Vector3.zero;
                return false;
            }
        }


        //
        // class Octree
        //
        public class Octree
        {
            class RectTransfomCollider
            {
                public RectTransform        rectTransform;
                public Vector3[]            cornerPoints = new Vector3[4];
            }

            class OctreeObject
            {
                public GameObject           gameObject;
                public Bounds               bounds;
                public int                  layer;

                public Mesh                 mesh;
                public Matrix4x4            localToWorldMatrix;
                public Collider             collider;
                public RectTransfomCollider rectTransfomCollider;

                public int                  raycastOpID;
            }

            struct RaycastData
            {
                public Ray                  ray;
                public int                  layersMask;
                public int                  ignoreLayersMask;
                public List<GameObject>     objectList;

                public RaycastInfo          raycastInfo;
                public GameObject           gameObject;
            }



            Octree[]                        childs = new Octree[8];
            Bounds                          bounds;
            List<OctreeObject>              objects = null;

            const float                     kNodesOverlapSize = 0.001f; // fixes situations where ray lies between nodes

            static int                      raycastOpID = 0;

            delegate bool HandleUtility_IntersectRayMesh(Ray ray, Mesh mesh, Matrix4x4 matrix, out UnityEngine.RaycastHit raycastHit);
            static HandleUtility_IntersectRayMesh IntersectRayMesh = null;


            // Static Constructor
            static Octree()
            {            
                MethodInfo methodIntersectRayMesh = typeof(HandleUtility).GetMethod("IntersectRayMesh", BindingFlags.Static | BindingFlags.NonPublic);

                if (methodIntersectRayMesh != null)
                {
                    IntersectRayMesh = delegate(Ray ray, Mesh mesh, Matrix4x4 matrix, out UnityEngine.RaycastHit raycastHit)
                    {
                        object[] parameters = new object[] { ray, mesh, matrix, null };
                        bool result = (bool)methodIntersectRayMesh.Invoke(null, parameters);
                        raycastHit = (UnityEngine.RaycastHit)parameters[3];
                        return result;
                    };
                }

            }

            public Octree(int depth)
            {
                if(depth <= 0)
                    return;

                for (int i = 0; i < 8; i++)
                    childs[i] = new Octree(depth-1);
            }


            void Resize(Bounds treeBounds)
            {
                bounds = treeBounds;

                if (childs[0] == null)
                    return;                

                Vector3 center = treeBounds.center;
                Vector3 offset = treeBounds.extents * 0.5f;
                Vector3 childSize = treeBounds.extents + new Vector3(kNodesOverlapSize, kNodesOverlapSize, kNodesOverlapSize);;

                childs[0].Resize(new Bounds(new Vector3(center.x + offset.x, center.y + offset.y, center.z + offset.z), childSize));
                childs[1].Resize(new Bounds(new Vector3(center.x + offset.x, center.y + offset.y, center.z - offset.z), childSize));
                childs[2].Resize(new Bounds(new Vector3(center.x + offset.x, center.y - offset.y, center.z + offset.z), childSize));
                childs[3].Resize(new Bounds(new Vector3(center.x + offset.x, center.y - offset.y, center.z - offset.z), childSize));
                childs[4].Resize(new Bounds(new Vector3(center.x - offset.x, center.y + offset.y, center.z + offset.z), childSize));
                childs[5].Resize(new Bounds(new Vector3(center.x - offset.x, center.y + offset.y, center.z - offset.z), childSize));
                childs[6].Resize(new Bounds(new Vector3(center.x - offset.x, center.y - offset.y, center.z + offset.z), childSize));
                childs[7].Resize(new Bounds(new Vector3(center.x - offset.x, center.y - offset.y, center.z - offset.z), childSize));
            }


            // Adds gameObject to root node, it bounds always be raycasted
            public void AddObjectFast(GameObject gameObject, bool useAdditionalVertexStreams)
            {
                Utility.ForAllInHierarchy(gameObject, (go) => {
                
                    OctreeObject octreeObject = MakeOctreeObject(go, useAdditionalVertexStreams);
                    if(octreeObject != null)
                    {
                        if (objects == null)
                            objects = new List<OctreeObject>(16);
                        
                        objects.Add(octreeObject);
                        bounds.Encapsulate(octreeObject.bounds);
                    }
                });
            }


            OctreeObject MakeOctreeObject(GameObject gameObject, bool useAdditionalVertexStreams)
            {
                if(!gameObject.activeInHierarchy)
                    return null;

                Renderer        renderer = gameObject.GetComponent<Renderer>();
                MeshFilter      meshFilter = gameObject.GetComponent<MeshFilter>();
                Collider        collider;
                RectTransform   rectTransform;

                if (renderer != null && renderer.enabled && 
                    meshFilter != null && meshFilter.sharedMesh != null)
                {
                    OctreeObject obj = new OctreeObject();

                    MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();

                    obj.bounds = renderer.bounds;
                    obj.layer = 1 << gameObject.layer;
                    obj.raycastOpID = raycastOpID;

                    obj.gameObject = gameObject;
                    obj.localToWorldMatrix = renderer.localToWorldMatrix;

                    if(useAdditionalVertexStreams && meshRenderer != null && meshRenderer.additionalVertexStreams != null)
                        obj.mesh = meshRenderer.additionalVertexStreams;
                    else
                        obj.mesh = meshFilter.sharedMesh;

                    return obj;
                }
                else if ((collider = gameObject.GetComponent<Collider>()) != null && collider.enabled)
                {
                    OctreeObject obj = new OctreeObject();

                    obj.bounds = collider.bounds;
                    obj.layer = 1 << gameObject.layer;
                    obj.raycastOpID = raycastOpID;

                    obj.gameObject = gameObject;
                    obj.collider = collider;

                    return obj;
                }
                else if((rectTransform = gameObject.GetComponent<RectTransform>()) != null)
                {
                    RectTransfomCollider rectCollider = new RectTransfomCollider();
                    OctreeObject obj = new OctreeObject();
                    Bounds bounds = new Bounds();

                    rectTransform.GetWorldCorners(rectCollider.cornerPoints);
                    bounds.Encapsulate(rectCollider.cornerPoints[0]);
                    bounds.Encapsulate(rectCollider.cornerPoints[1]);
                    bounds.Encapsulate(rectCollider.cornerPoints[2]);
                    bounds.Encapsulate(rectCollider.cornerPoints[3]);
                    rectCollider.rectTransform = rectTransform;

                    obj.bounds = bounds;
                    obj.layer = 1 << gameObject.layer;
                    obj.raycastOpID = raycastOpID;

                    obj.gameObject = gameObject;
                    obj.rectTransfomCollider = rectCollider;

                    return obj;
                }

                return null;
            }

            public void Populate(GameObject[] sceneObjects, bool useAdditionalVertexStreams)
            {                   
                Cleanup();

                Bounds worldBounds = new Bounds(Vector3.zero, Vector3.zero);
                List<OctreeObject> raycastObjects = new List<OctreeObject>(sceneObjects.Length);

                for(int i = 0; i < sceneObjects.Length; i++)
                {
                    OctreeObject octreeObject = MakeOctreeObject(sceneObjects[i], useAdditionalVertexStreams);
                    if(octreeObject != null)
                    {
                        worldBounds.Encapsulate(octreeObject.bounds);
                        raycastObjects.Add(octreeObject);
                    }
                }

                Resize(worldBounds);

                for(int i = 0; i < raycastObjects.Count; i++)
                    AddObject(raycastObjects[i]);
            }



            public void Cleanup()
            {
                if (objects != null)
                    objects.Clear();

                if (childs[0] != null) {
                    for(int i = 0; i < 8; i++)
                        childs[i].Cleanup();
                }
            }


            public bool RemoveGameObject(GameObject gameObject)
            {
                if (childs[0] == null)
                {
                    if (objects != null)
                    {
                        objects.RemoveAll((o) => o.gameObject == gameObject);
                        return true;
                    }
                }
                else {
                    for(int i = 0; i < 8; i++)
                        if(childs[i].RemoveGameObject(gameObject))
                            return true;
                }
                return false;
            }


            void AddObject(OctreeObject obj)
            {
                if(bounds.Intersects(obj.bounds))
                {
                    if (childs[0] == null)  {
                        if (objects == null)
                            objects = new List<OctreeObject>(16);
                        objects.Add(obj);
                    }
                    else {
                        for(int i = 0; i < 8; i++)
                            childs[i].AddObject(obj);
                    }
                }
            }


            static bool RaycastTriangle(Ray ray, Vector3 p1, Vector3 p2, Vector3 p3, out float u, out float v, out float t)
            {
                Vector3 e1 = p2 - p1;
                Vector3 e2 = p3 - p1;

                Vector3 pvec = Vector3.Cross (ray.direction, e2);
                float det = Vector3.Dot(e1, pvec);

                u = v = t = 0;

                if (det == 0.0f)
                    return false;

                float invDet = 1.0f / det;
                Vector3 tvec  = ray.origin  - p1;

                u = invDet * Vector3.Dot(tvec, pvec);
                if (u < 0.0f || u > 1.0f)
                    return false;

                Vector3 qvec = Vector3.Cross (tvec, e1);
                v = invDet * Vector3.Dot(qvec, ray.direction);
                if (v < 0.0f || u+v > 1.0f)
                    return false;

                t = Vector3.Dot(e2, qvec) * invDet;

                if(t > Mathf.Epsilon)
                    return true;

                return false;
            }

            public static bool IntersectRayMeshEx(Ray ray, Mesh mesh, Matrix4x4 matrix, out RaycastInfo raycastInfo)
            {
                raycastInfo = default(RaycastInfo);
                raycastInfo.isHitTargetLayer = false;
                raycastInfo.distance = Mathf.Infinity;
                raycastInfo.ray = ray;

                UnityEngine.RaycastHit unityRaycastHit = default(UnityEngine.RaycastHit);

                if (IntersectRayMesh != null && 
                    IntersectRayMesh(ray, mesh, matrix, out unityRaycastHit))
                {       
                    raycastInfo.isHitTargetLayer = true;
                    raycastInfo.point = unityRaycastHit.point;
                    raycastInfo.normal = unityRaycastHit.normal.normalized;
                    raycastInfo.ray = ray;
                    raycastInfo.triangleIndex = unityRaycastHit.triangleIndex;
                    raycastInfo.textureCoord = unityRaycastHit.textureCoord;
                    raycastInfo.distance = unityRaycastHit.distance;

                    raycastInfo.barycentricCoordinate.x = unityRaycastHit.barycentricCoordinate.x;
                    raycastInfo.barycentricCoordinate.y = unityRaycastHit.barycentricCoordinate.y;

                    raycastInfo.localNormal = matrix.transpose.inverse.MultiplyVector(raycastInfo.normal).normalized;
                    raycastInfo.localPoint = matrix.inverse.MultiplyPoint (raycastInfo.point);

                    return true;
                }

                return false;
            }


            void Raycast(ref RaycastData raycastData)
            {                
                float distance;

                // Raycast node bounds
                if(!bounds.IntersectRay(raycastData.ray, out distance) || distance > raycastData.raycastInfo.distance)
                    return;

                // Raycast childs
                if (childs[0] != null)
                {
                    for(int i = 0; i < 8; i++)
                        childs[i].Raycast(ref raycastData);
                }

                // no objects in node
                if(objects == null)
                    return;

                // raycast all objects in leaf
                for(int i = 0; i < objects.Count; i++)
                {
                    OctreeObject obj = objects[i];

                    if (obj.gameObject == null) // object removed
                        continue;

                    // skip raycasted in current raycast operation 
                    if (obj.raycastOpID == raycastOpID)
                        continue;

                    objects[i].raycastOpID = raycastOpID;


                    bool isMasked = (obj.layer & raycastData.layersMask) == 0;

                    // through ignore layers
                    if((obj.layer & raycastData.ignoreLayersMask) != 0)
                        continue;


                    bool isObjectInList = raycastData.objectList != null && raycastData.objectList.Contains(obj.gameObject);

                    if(raycastData.objectList != null && !isObjectInList)
                        continue;

                    // check object bounds and distance
                    if(!objects[i].bounds.IntersectRay(raycastData.ray, out distance) || distance > raycastData.raycastInfo.distance)
                        continue;                        


                    RaycastInfo raycastInfo = default(RaycastInfo);
                    if(obj.mesh != null)
                    {
                        if(IntersectRayMeshEx(raycastData.ray, obj.mesh, obj.localToWorldMatrix, out raycastInfo))
                        {
                            if(raycastInfo.distance < raycastData.raycastInfo.distance)
                            {
                                raycastData.gameObject = obj.gameObject;
                                raycastData.raycastInfo = raycastInfo;
                            }
                        }

                    }
                    else if (obj.collider != null)
                    {
                        UnityEngine.RaycastHit unityRaycastHit;

                        // NOTE: do not use Mathf.Infinity, strange bug
                        if(obj.collider.Raycast(raycastData.ray, out unityRaycastHit, 100000.0f))
                        {
                            if(unityRaycastHit.distance < raycastData.raycastInfo.distance)
                            {
                                raycastInfo.ray = raycastData.ray;
                                raycastInfo.isHitTargetLayer = true;
                                raycastInfo.isHitMaskedLayer = false;
                                raycastInfo.distance = unityRaycastHit.distance;
                                raycastInfo.point = unityRaycastHit.point;
                                raycastInfo.normal = unityRaycastHit.normal;

                                raycastData.gameObject = obj.gameObject;
                                raycastData.raycastInfo = raycastInfo;
                            }
                        }
                    }
                    else if(obj.rectTransfomCollider != null && obj.rectTransfomCollider != null)
                    {
                        float u = 0, v = 0, t = 0;

                        if(RaycastTriangle(raycastData.ray, obj.rectTransfomCollider.cornerPoints[0], obj.rectTransfomCollider.cornerPoints[1], obj.rectTransfomCollider.cornerPoints[2], out u, out v, out t) ||
                           RaycastTriangle(raycastData.ray, obj.rectTransfomCollider.cornerPoints[0], obj.rectTransfomCollider.cornerPoints[2], obj.rectTransfomCollider.cornerPoints[3], out u, out v, out t))
                        {
                            if(t < raycastData.raycastInfo.distance)
                            {
                                raycastInfo.ray = raycastData.ray;
                                raycastInfo.isHitTargetLayer = true;
                                raycastInfo.isHitMaskedLayer = false;
                                raycastInfo.distance = t;
                                raycastInfo.point = raycastData.ray.GetPoint(t);
                                raycastInfo.normal = (new Plane(obj.rectTransfomCollider.cornerPoints[0], obj.rectTransfomCollider.cornerPoints[1], obj.rectTransfomCollider.cornerPoints[2])).normal;

                                raycastData.gameObject = obj.gameObject;
                                raycastData.raycastInfo = raycastInfo;
                            }
                        }
                    }


                    if(raycastInfo.isHitTargetLayer && isMasked && raycastData.objectList == null)
                    {
                        raycastData.raycastInfo.isHitTargetLayer = false;
                        raycastData.raycastInfo.isHitMaskedLayer = true;
                    }
                }

            }

            public GameObject Raycast(Ray ray, out RaycastInfo raycastInfo, int layersMask, int ignoreLayersMask, List<GameObject> objectList)
            {                
                raycastOpID++;

                RaycastData raycastData = new RaycastData();
                raycastData.raycastInfo = default(RaycastInfo);
                raycastData.ray = ray;
                raycastData.layersMask = layersMask;
                raycastData.objectList = objectList;
                raycastData.ignoreLayersMask = ignoreLayersMask;
                raycastData.raycastInfo.distance = float.PositiveInfinity;
                raycastData.gameObject = null;

                Raycast(ref raycastData);

                raycastInfo = raycastData.raycastInfo;

                if (raycastData.gameObject != null)
                {
                    // if we hit backwards - reverse normal
                    if (Vector3.Dot (ray.direction, raycastInfo.normal) > 0.0f)
                    {
                        raycastInfo.normal = -raycastInfo.normal;
                        raycastInfo.localNormal = -raycastInfo.localNormal;
                    }
                }

                return raycastData.gameObject;
            }



            struct IntersectSphereData
            {
                public Vector3 spherePoint;
                public float sphereRadiusSq;
                public Func<GameObject, bool> func;
            }


            bool IntersectSphere(ref IntersectSphereData data)
            {
                if(bounds.SqrDistance(data.spherePoint) > data.sphereRadiusSq)
                    return true;

                // Raycast childs
                if (childs[0] != null)
                {
                    for(int i = 0; i < 8; i++)
                        if(!childs[i].IntersectSphere(ref data))
                            return false;
                }

                // no objects in leaf
                if(objects == null)
                    return true;

                for(int i = 0; i < objects.Count; i++)
                {
                    OctreeObject obj = objects[i];

                    if (obj.gameObject == null) // object removed
                        continue;

                    // skip raycasted in current raycast operation 
                    if (obj.raycastOpID == raycastOpID)
                        continue;

                    obj.raycastOpID = raycastOpID;

                    if(obj.bounds.SqrDistance(data.spherePoint) < data.sphereRadiusSq)
                    {
                        if(!data.func.Invoke(obj.gameObject))
                            return false;
                    }
                }

                return true;
            }

            public void IntersectSphere(Vector3 point, float radius, Func<GameObject, bool> func)
            {                
                if(func == null)
                    return;

                raycastOpID++;
                
                IntersectSphereData data = new IntersectSphereData();
                data.spherePoint = point;
                data.sphereRadiusSq = radius * radius;
                data.func = func;

                IntersectSphere(ref data);
            }


            struct IntersectBoundsData
            {
                public Bounds bounds;
                public Func<GameObject, bool> func;
            }

            bool IntersectBounds(ref IntersectBoundsData data)
            {
                if(!bounds.Intersects(data.bounds))
                    return true;

                // Raycast childs
                if (childs[0] != null)
                {
                    for(int i = 0; i < 8; i++)
                    {
                        if(!childs[i].IntersectBounds(ref data))
                            return false;
                    }
                }

                // no objects in leaf
                if(objects == null)
                    return true;

                for(int i = 0; i < objects.Count; i++)
                {     
                    OctreeObject obj = objects[i];

                    if (obj.gameObject == null) // object removed
                        continue;

                    // skip raycasted in current raycast operation 
                    if (obj.raycastOpID == raycastOpID)
                        continue;

                    obj.raycastOpID = raycastOpID;

                    if(obj.bounds.Intersects(data.bounds))
                    {
                        if(!data.func.Invoke(obj.gameObject))
                            return false;
                    }
                }

                return true;
            }

            public void IntersectBounds(Bounds bounds, Func<GameObject, bool> func)
            {
                if(func == null)
                    return;

                raycastOpID++;

                IntersectBoundsData data = new IntersectBoundsData();
                data.bounds = bounds;
                data.func = func;

                IntersectBounds(ref data);
            }
        } // class Octree






        enum PaintTool
        {
            None         = -1,
            Brush        = 0,
            PrecisePlace = 1,
            Erase        = 2,
            Select       = 3,
            Settings     = 4,
        }

#region Variables
        PaintTool _lastTool = PaintTool.Brush;
        PaintTool lastTool {
            get { return _lastTool; }
            set {
                if(value != PaintTool.None && value != PaintTool.Settings)
                    _lastTool = value;
            }
        }
        PaintTool _currentTool  = PaintTool.None;
        PaintTool currentTool {
            get { return _currentTool; }
            set
            {
                if (_currentTool == value)
                    return;

                OnToolDisabled(_currentTool);

                switch(value)
                {
                case PaintTool.Brush:
                    {
                        UnityEditor.Tools.current = Tool.None;
                        if (selectedObjects == null)
                            selectedObjects = Selection.objects;
                        Selection.objects = new UnityEngine.Object[0];

                        octree.Populate(GameObject.FindObjectsOfType<GameObject>(), settings.useAdditionalVertexStreams);
                    }
                    break;
                case PaintTool.PrecisePlace:
                    {
                        UnityEditor.Tools.current = Tool.None;
                        if (selectedObjects == null)
                            selectedObjects = Selection.objects;
                        Selection.objects = new UnityEngine.Object[0];

                        octree.Populate(GameObject.FindObjectsOfType<GameObject>(), settings.useAdditionalVertexStreams);
                    }
                    break;                
                case PaintTool.Erase:
                    {
                        UnityEditor.Tools.current = Tool.None;
                        if (selectedObjects == null)
                            selectedObjects = Selection.objects;
                        Selection.objects = new UnityEngine.Object[0];

                        octree.Populate(GameObject.FindObjectsOfType<GameObject>(), settings.useAdditionalVertexStreams);
                    }
                    break;
                case PaintTool.Select:
                    {
                        UnityEditor.Tools.current = Tool.None;

                        octree.Populate(GameObject.FindObjectsOfType<GameObject>(), settings.useAdditionalVertexStreams);

                        selectedObjects = null;
                        selectionTool.selectedObjects.Clear();
                        selectionTool.selectedObjects.AddRange(Selection.gameObjects);
                    }
                    break;
                case PaintTool.Settings:
                    {
                        if (selectedObjects != null)
                        {
                            Selection.objects = selectedObjects;
                            selectedObjects = null;
                        }
                        if (octree != null)
                            octree.Cleanup();
                    }
                    break;
                case PaintTool.None:
                    {
						if (selectedObjects != null && Selection.objects.Length == 0)
                        {
                            Selection.objects = selectedObjects;                            
                        }

						selectedObjects = null;

                        if (octree != null)
                            octree.Cleanup();
                    }
                    break;
                }

                _currentTool = value;
                lastTool = value;

                OnToolEnabled(_currentTool);
            }
        }

        bool IsPaintToolEnabled()
        {
            return currentTool == PaintTool.Brush || currentTool == PaintTool.PrecisePlace || currentTool == PaintTool.Erase || currentTool == PaintTool.Select;
        }






        class PlacedObjectInfo
        {
            public RaycastInfo     raycastInfo;
            public GameObject      gameObject;
            public Brush           brush;
            public int             prefabSlot;

            public PlacedObjectInfo(GameObject gameObject, Brush brush, int prefabSlot)
            {
                this.gameObject = gameObject;
                this.brush = brush;
                this.prefabSlot = prefabSlot;
            }

            public PlacedObjectInfo(RaycastInfo raycastInfo, GameObject gameObject, Brush brush, int prefabSlot)
            {
                this.raycastInfo = raycastInfo;
                this.gameObject = gameObject;
                this.brush = brush;
                this.prefabSlot = prefabSlot;
            }
        }

        class BrushTool
        {            
            public RaycastInfo      raycast;
            public RaycastInfo      prevRaycast;
            public float            dragDistance;
            public Vector3          strokeDirection;
            public Vector3          strokeDirectionRefPoint;
            public PlacedObjectInfo lastPlacedObjectInfo;
            public Vector3          firstNormal;

            //public List<GameObject> prefabList = new List<GameObject>(64);
        }

        class PrecisePlaceTool
        {
//            public Brush            brush;
//
//            public PlacedObjectInfo previewObject;            
            public PlacedObjectInfo placedObjectInfo;

            public Vector3       scaleFactor;
            public Quaternion    orienation;

            public float         angle;
            public float         radius;
            public Vector3       point;
        }

        class EraseTool
        {
            public List<GameObject> prefabList = new List<GameObject>(64);
        }

        class SelectionTool
        {
            public List<GameObject> prefabList = new List<GameObject>(64);
            public List<GameObject> selectedObjects = new List<GameObject>();
        }

        class Grid
        {
            public const int            kSize = 7;
            public const float          kDeadZoneSize = 0.2f;

            public RaycastInfo          originRaycastInfo;
            public GameObject           originHitObject;
            public Vector3              visualOrigin;
            public bool                 inDeadZone = false;
        }

        BrushTool                   brushTool = new BrushTool();
        PrecisePlaceTool            precisePlaceTool = new PrecisePlaceTool();
        EraseTool                   eraseTool = new EraseTool();
        SelectionTool               selectionTool = new SelectionTool();
        Grid                        grid = new Grid();

        RaycastInfo                 currentRaycast;
        GameObject                  currentHitObject;

		bool							onPickObject = false;
		Action<GameObject, RaycastInfo> onPickObjectAction = null;
		string 							onPickObjectMessage = "";

        #if UNITY_5_5_OR_NEWER
        // uses EditorApplication.delayCall
        #else
        List<Action>                delayedActions = new List<Action>(); // run once in EditorApplicationUpdateCallback
        #endif

        const int                   kOctreeDepth = 4;
        Octree                      octree = null;


        static int                  s_PrefabPainterEditorHash = "PrefabPainterEditor".GetHashCode();
        static PrefabPainter        s_activeWindow;


		//
		// Database
        PrefabPainterSettings 	    settings;
        PrefabPainterSceneSettings  sceneSettings;      

		string   					workDirectoryPath		   = null;
		const string 				kGUIDirectoryName          = "GUI";
		const string 				kSettingsDirectoryName     = "Settings";
		const string 				kPresetsDirectoryName      = "Presets";
		const string 				kSettingsFileName          = "settings.asset";
		const string 				kDefaultSettingsFileName   = "defaultSettings.asset";
		const string 				kSettingsObjectName        = "PrefabPainterSceneSettings";

        //
        // Selected objects 
        UnityEngine.Object[]        _selectedObjects = null;
        List<GameObject>            _selectedGameObjects = null;

        List<GameObject>            selectedGameObjects
        {
            get { return _selectedGameObjects; }
        }

        UnityEngine.Object[]        selectedObjects
        {
            get
            {
                return _selectedObjects;
            }

            set
            {
                _selectedObjects = value;

                if (_selectedObjects == null)
                {
                    _selectedGameObjects = null;
                }
                else
                {
                    _selectedGameObjects = new List<GameObject>(_selectedObjects.Length);

                    foreach(UnityEngine.Object obj in _selectedObjects) {
                        if (obj is GameObject)
                        {
                            Utility.ForAllInHierarchy(obj as GameObject, (gameObject) => {
                                _selectedGameObjects.Add(gameObject);
                            });
                        }
                    }
                }
            }
        }

#endregion // Variables







#region Initialization

		// Unity Editor Menu Item
		[MenuItem ("Window/nTools/Prefab Painter")]
		static void Init ()
		{
            // Get existing open window or if none, make a new one:
            PrefabPainter window = (PrefabPainter)EditorWindow.GetWindow (typeof (PrefabPainter));
            window.ShowUtility(); 
		}


		public string GetWorkDirectory()
		{
			if (workDirectoryPath != null)
				return workDirectoryPath;

			MonoScript ownerScript;
			string ownerPath;            

			// work dir based on PrefabPainter.cs script path. 
			// get PrefabPainter.cs script 
			if ((ownerScript = MonoScript.FromScriptableObject(this)) != null)
			{                
				// get .../PrefabPainter/Scripts/Editor/PrefabPainter.cs
				if((ownerPath = AssetDatabase.GetAssetPath(ownerScript)) != null)
				{
					// get .../PrefabPainter/Scripts/Editor/
					ownerPath = Path.GetDirectoryName(ownerPath);
					// get .../PrefabPainter/Scripts/
					ownerPath = Path.GetDirectoryName(ownerPath);
					// get .../PrefabPainter/
					workDirectoryPath = Path.GetDirectoryName(ownerPath);
				}
			}

			return workDirectoryPath;
		}

		public string GetGUIDirectory() {
			string directory =  Path.Combine(GetWorkDirectory(), kGUIDirectoryName);
			if(!Directory.Exists(directory)) {
				Directory.CreateDirectory(directory);
				AssetDatabase.Refresh();
			}
			return directory;
		}
		
		public string GetSettingsDirectory() {            
			string directory =  Path.Combine(GetWorkDirectory(), kSettingsDirectoryName);
			if(!Directory.Exists(directory)) {
				Directory.CreateDirectory(directory);
				AssetDatabase.Refresh();
			}
			return directory;
		}
		
		public string GetPresetsDirectory()
		{
			string directory = Path.Combine(GetWorkDirectory(), kPresetsDirectoryName);
			if(!Directory.Exists(directory)) {
				Directory.CreateDirectory(directory);
				AssetDatabase.Refresh();
			}
			return directory;
		}


		void LoadSceneSettings()
		{			
			GameObject gameObject = GameObject.Find(kSettingsObjectName);
            if (gameObject == null) {
                gameObject = new GameObject(kSettingsObjectName);
                Utility.MarkActiveSceneDirty();
            }           

            sceneSettings = gameObject.GetComponent<PrefabPainterSceneSettings>();
            if (sceneSettings == null) {
                sceneSettings = gameObject.AddComponent<PrefabPainterSceneSettings>();				
                Utility.MarkActiveSceneDirty();
            }

            HideFlags hideFlags = settings.hideSceneSettingsObject ? (HideFlags.HideInHierarchy|HideFlags.HideInInspector|HideFlags.DontSaveInBuild) : (HideFlags.DontSaveInBuild);

            if(gameObject.hideFlags != hideFlags)
            {
                gameObject.hideFlags = hideFlags;
                Utility.MarkActiveSceneDirty();
                EditorApplication.RepaintHierarchyWindow();
            }
        }



        PrefabPainterSettings LoadSettings()
        {
            string settingsDirectoryPath = GetSettingsDirectory();

            // Try load settings asset
            PrefabPainterSettings settings = AssetDatabase.LoadAssetAtPath(Path.Combine(settingsDirectoryPath, kSettingsFileName), typeof(PrefabPainterSettings)) as PrefabPainterSettings;
            if (settings == null)
            {
                // if no settings file, try load default settings file
                settings = AssetDatabase.LoadAssetAtPath(Path.Combine(settingsDirectoryPath, kDefaultSettingsFileName), typeof(PrefabPainterSettings)) as PrefabPainterSettings;
                if (settings != null)
                {
                    // Duplicate
                    settings = Instantiate(settings);

                    // Save as settingsFileName
                    AssetDatabase.CreateAsset(settings, Path.Combine(settingsDirectoryPath, kSettingsFileName));
                }
                else
                    // if no default settings file - create new instance
                {
                    settings = ScriptableObject.CreateInstance<PrefabPainterSettings>();

                    // Save as settingsFileName
                    AssetDatabase.CreateAsset(settings, Path.Combine(settingsDirectoryPath, kSettingsFileName));
                }
            }

            PrefabPainterSettings.current = settings;

            return settings;
        }


        public void Awake()
        {
        }

		void OnEnable () 
		{
            hideFlags = HideFlags.HideAndDontSave;

			s_activeWindow = this;


            settings = LoadSettings();
            LoadSceneSettings();


            currentTool = PaintTool.None;

            // Initialize Octree
            octree = new Octree(kOctreeDepth);


            OnInitGUI();


            // Setup callbacks
            if(SceneView.onSceneGUIDelegate != OnSceneGUI)
                SceneView.onSceneGUIDelegate += OnSceneGUI;

            if (EditorApplication.update != EditorApplicationUpdateCallback)
                EditorApplication.update += EditorApplicationUpdateCallback; 

            if (Undo.undoRedoPerformed != UndoRedoPerformedCallback) 
                Undo.undoRedoPerformed += UndoRedoPerformedCallback;

            if(EditorApplication.modifierKeysChanged != ModifierKeysChangedCallback)
                EditorApplication.modifierKeysChanged += ModifierKeysChangedCallback;

		}



		void OnDisable () 
		{
            currentTool = PaintTool.None;

            OnCleanupGUI();
            
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
            EditorApplication.update -= EditorApplicationUpdateCallback;
            Undo.undoRedoPerformed -= UndoRedoPerformedCallback; 
            EditorApplication.modifierKeysChanged -= ModifierKeysChangedCallback;

            s_activeWindow = null;
                        
            EditorUtility.SetDirty(settings);
		}



		void UndoRedoPerformedCallback()
		{
			Repaint();
		}



        void EditorApplicationUpdateCallback()
        {
            #if UNITY_5_5_OR_NEWER
            #else
            RunDelayedActions();
            #endif

            if (UnityEditor.Tools.current != Tool.None && IsPaintToolEnabled())
            {
                currentTool = PaintTool.None;
                Repaint();
            }


            if (Time.realtimeSinceStartup - lastUIRepaintTime > kUIRepaintInterval)
            {
                lastUIRepaintTime = Time.realtimeSinceStartup;
                Repaint();
            }


            // Lose scene settings - lose scene
            if(sceneSettings == null)
            {
                currentTool = PaintTool.None;
                
                octree.Cleanup();

                LoadSceneSettings();
                Repaint();
            }
        }

        #if UNITY_5_5_OR_NEWER
        void AddDelayedAction(EditorApplication.CallbackFunction action)
        {
            EditorApplication.delayCall += action;
        }
        #else
        void AddDelayedAction(Action action)
        {
            delayedActions.Add(action);
        }

        void RunDelayedActions()
        {
            if(delayedActions.Count > 0)
            {
                delayedActions.ForEach((action) => { action.Invoke(); } );
                delayedActions.Clear();
            }
        }
        #endif

        void ModifierKeysChangedCallback()
        {
            Repaint();
        }


#endregion // Initialization













#region Object Placement

        GameObject Raycast(Ray ray, out RaycastInfo raycastInfo, int layersMask, int ignoreLayersMask)
        {
            RaycastInfo hit = default(RaycastInfo);
            raycastInfo = default(RaycastInfo);

            if (octree == null)
                return null;

            GameObject gameObject = null;


            if (settings.paintOnSelected)
            {
                if (selectedObjects == null)
                    return null;
                
                gameObject = octree.Raycast(ray, out hit, ~0, ignoreLayersMask, _selectedGameObjects);
            }
            else
            {
                gameObject = octree.Raycast(ray, out hit, layersMask, ignoreLayersMask, null);
            }

            if (gameObject == null)
                return null;
         
            raycastInfo = hit;
            return gameObject;
        }


        OrientationMode ToOrientationMode(PrecisePlaceOrientationMode mode)
        {
            switch(mode) {
            case PrecisePlaceOrientationMode.AlongSurfaceNormal: return OrientationMode.AlongSurfaceNormal;
            case PrecisePlaceOrientationMode.X: return OrientationMode.X;
            case PrecisePlaceOrientationMode.Y: return OrientationMode.Y;
            case PrecisePlaceOrientationMode.Z: return OrientationMode.Z;
            }
            return OrientationMode.AlongSurfaceNormal;
        }


        void OrientObject(PlacedObjectInfo placedObjectInfo)
        {            
            Transform transform = placedObjectInfo.gameObject.transform;
            bool isRectTransform = transform is RectTransform;
            Vector3 normal = placedObjectInfo.raycastInfo.normal;
            BrushSettings brushSettings = placedObjectInfo.brush.settings;
            int prefabSlot = placedObjectInfo.prefabSlot;

            Quaternion placeOrientation = Quaternion.identity;
            Quaternion randomRotation = Quaternion.identity;
            Vector3 rotation = Vector3.zero;

			OrientationMode orientationMode = brushSettings.orientationMode;
			bool flipOrientation = brushSettings.flipOrientation;
			
			if(currentTool == PaintTool.PrecisePlace)
			{
				orientationMode = ToOrientationMode(brushSettings.ppOrientationMode);
				flipOrientation = brushSettings.ppFlipOrientation;
			}

            // Place orientation
            {
                Vector3 forward;
                Vector3 upwards;

                switch(orientationMode)
                {
                case OrientationMode.AlongBrushStroke:
                    {
                        forward = brushTool.strokeDirection;
                                                
						upwards = normal;
                        if (flipOrientation)
                            forward = -forward;			

						if (isRectTransform)
							placeOrientation = Quaternion.LookRotation(Vector3.Cross(-upwards, forward).normalized, upwards);
						else
                        	placeOrientation = Quaternion.LookRotation(Vector3.Cross(forward, upwards).normalized, upwards);
                    }
                    break;
                case OrientationMode.AlongSurfaceNormal:
                    {
                        Vector3 right;
						upwards = normal;
                        if (flipOrientation)
                            upwards = -upwards;
                        GetRightForward(upwards, out right, out forward);
						if (isRectTransform)
							placeOrientation = Quaternion.LookRotation(-upwards, forward);
						else
                        	placeOrientation = Quaternion.LookRotation(forward, upwards);
						
                    }
                    break;
                case OrientationMode.X:
                    {
                        Vector3 right;
                        upwards = new Vector3(1, 0, 0);
                        if (flipOrientation)
                            upwards = -upwards;
                        GetRightForward(upwards, out right, out forward);
                        placeOrientation = Quaternion.LookRotation(forward, upwards);

                    }
                    break;
                case OrientationMode.Y:
                    {
                        Vector3 right;
                        upwards = new Vector3(0, 1, 0);
                        if (flipOrientation)
                            upwards = -upwards;
                        GetRightForward(upwards, out right, out forward);
                        placeOrientation = Quaternion.LookRotation(forward, upwards);

                    }
                    break;
                case OrientationMode.Z:
                    {
                        Vector3 right;
                        upwards = new Vector3(0, 0, 1);
                        if (flipOrientation)
                            upwards = -upwards;
                        GetRightForward(upwards, out right, out forward);
                        placeOrientation = Quaternion.LookRotation(forward, upwards);
                    }
                    break;
                default: case OrientationMode.None:
                    placeOrientation = Quaternion.identity;
                    break;
                }

            }




            // Random rotation
			if (currentTool == PaintTool.Brush && orientationMode != OrientationMode.AlongBrushStroke)
            {
                Vector3 randomVector = UnityEngine.Random.insideUnitSphere * 0.5f;
                randomRotation = Quaternion.Euler(new Vector3(brushSettings.randomizeOrientationX * 3.6f * randomVector.x,
                    brushSettings.randomizeOrientationY * 3.6f * randomVector.y,
                    brushSettings.randomizeOrientationZ * 3.6f * randomVector.z));
            }


            if (currentTool == PaintTool.PrecisePlace)
            {
                if(brushSettings.ppFixedRotation)
                {
                    rotation = brushSettings.ppFixedRotationValue;
                }
            }

            if (currentTool == PaintTool.Brush)
            {
                rotation = brushSettings.rotation;
            }


            // Orient Mode
            switch (brushSettings.orientationTransformMode)
            {
            case TransformMode.Absolute:
                {
                    rotation = brushSettings.rotation;
                    transform.eulerAngles = Vector3.zero;
                    transform.localEulerAngles = Vector3.zero;

                    if(brushSettings.multibrushEnabled)
                        rotation += brushSettings.multibrushSlots[prefabSlot].rotation;

                    transform.rotation =  placeOrientation * (randomRotation * Quaternion.Euler(rotation));
                }
                break;
            default: case TransformMode.Relative:
                {
                    rotation = rotation + transform.localEulerAngles;
                    transform.eulerAngles = Vector3.zero;
                    transform.localEulerAngles = Vector3.zero;

                    if(brushSettings.multibrushEnabled)
                        rotation += brushSettings.multibrushSlots[prefabSlot].rotation;

                    transform.rotation = placeOrientation * (randomRotation * Quaternion.Euler(rotation));
                }
                break;
            }
        }




        void PositionObject(RaycastInfo raycastInfo, GameObject gameObject, BrushSettings brushSettings, int prefabSlot)
        {            
            Transform transform = gameObject.transform;

            Vector3 right;
            Vector3 forward;
            Vector3 upwards = raycastInfo.normal;

            OrientationMode orientationMode = brushSettings.orientationMode;

            if(currentTool == PaintTool.PrecisePlace)
            {
                orientationMode = ToOrientationMode(brushSettings.ppOrientationMode);
            }


            switch(orientationMode)
            {
            case OrientationMode.X:
                upwards = new Vector3(1, 0, 0);
                break;
            case OrientationMode.Y:   
                upwards = new Vector3(0, 1, 0);
                break;
            case OrientationMode.Z:
                upwards = new Vector3(0, 0, 1);    
                break;
            }

            if(brushSettings.orientationMode == OrientationMode.AlongBrushStroke)
            {
                forward = brushTool.strokeDirection;
                right = GetRightVector(upwards, forward);
            }
            else
            {
                GetRightForward(upwards, out right, out forward);
            }


			Vector3 offset = brushSettings.positionOffset - brushSettings.multibrushSlots[prefabSlot].pivotOffset;

            if(brushSettings.multibrushEnabled)
                offset += brushSettings.multibrushSlots[prefabSlot].position;


            offset *= brushSettings.brushOverallScale;


            transform.position = raycastInfo.point
                + right * offset.x
                + upwards * offset.y
                + forward * offset.z;

        }
            


        void ScaleObject(RaycastInfo raycastInfo, GameObject gameObject, BrushSettings brushSettings, int prefabSlot)
        {            
            Vector3 randomVector = UnityEngine.Random.insideUnitSphere;
            Vector3 scale;

            randomVector = new Vector3(Mathf.Abs(randomVector.x), Mathf.Abs(randomVector.y), Mathf.Abs(randomVector.z));

            if (brushSettings.scaleMode == AxisMode.Uniform)
            {
                float scaleValue = brushSettings.scaleUniformMin + randomVector.x * (brushSettings.scaleUniformMax - brushSettings.scaleUniformMin);
                scale = new Vector3(scaleValue, scaleValue, scaleValue);
            }
            else
            {                
                scale = new Vector3(brushSettings.scalePerAxisMin.x + randomVector.x * (brushSettings.scalePerAxisMax.x - brushSettings.scalePerAxisMin.x),
                    brushSettings.scalePerAxisMin.y + randomVector.y * (brushSettings.scalePerAxisMax.y - brushSettings.scalePerAxisMin.y),
                    brushSettings.scalePerAxisMin.z + randomVector.z * (brushSettings.scalePerAxisMax.z - brushSettings.scalePerAxisMin.z));
            }

            if (brushSettings.multibrushEnabled) {
				scale.x = brushSettings.multibrushSlots [prefabSlot].scale.x * scale.x;
				scale.y = brushSettings.multibrushSlots [prefabSlot].scale.y * scale.y;
				scale.z = brushSettings.multibrushSlots [prefabSlot].scale.z * scale.z;
			}
			

			if (brushSettings.scaleTransformMode == TransformMode.Relative)
                gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x * scale.x * brushSettings.scaleAux,
                                                              gameObject.transform.localScale.y * scale.y * brushSettings.scaleAux,
                                                              gameObject.transform.localScale.z * scale.z * brushSettings.scaleAux);
            else
                gameObject.transform.localScale = scale * brushSettings.scaleAux;

            gameObject.transform.localScale = gameObject.transform.localScale * brushSettings.brushOverallScale;

            gameObject.transform.localScale = Utility.RoundVector(gameObject.transform.localScale, 3);
        }



        float SlopeAngle(Brush brush, Vector3 surfaceNormal)
        {
            Vector3 refVector = Vector3.up;
            switch(brush.settings.slopeVector)
            {
            case SlopeVector.X:
                refVector = new Vector3(1, 0, 0);
                break;
            case SlopeVector.Y:
                refVector = new Vector3(0, 1, 0);
                break;
            case SlopeVector.Z:
                refVector = new Vector3(0, 0, 1);
                break;
            case SlopeVector.View:
                if(Camera.current != null)
                    refVector = -Camera.current.transform.forward;
                break;
            case SlopeVector.FirstNormal:
                refVector = brushTool.firstNormal;
                break;
            case SlopeVector.Custom:
                refVector = brush.settings.slopeVectorCustom.normalized;
                break;
            }

            if(brush.settings.slopeVectorFlip)
                refVector = -refVector;
            
            return Mathf.Acos(Mathf.Clamp01(Vector3.Dot(surfaceNormal, refVector))) * Mathf.Rad2Deg;
        }


        bool SlopeFilter(Brush brush, Vector3 surfaceNormal)
        {
            // Only in brush tool
            if(currentTool != PaintTool.Brush)
                return true;
            
            if(!brush.settings.slopeEnabled)
                return true;            

            float angle = SlopeAngle(brush, surfaceNormal);

            return angle >= brush.settings.slopeAngleMin && angle <= brush.settings.slopeAngleMax;
        }



        bool CheckOverlap(Brush brush, BrushSettings brushSettings, Bounds bounds)
        {            
            bool overlaps = false;
            float distance = brushSettings.brushOverlapDistance * brushSettings.brushOverallScale;
            int checkLayers = brushSettings.brushOverlapCheckLayers.value;
            List<GameObject> prefabList = null;


            if(brushSettings.brushOverlapCheckObjects == OverlapCheckObjects.SameObjects)
            {
                prefabList = new List<GameObject>(BrushSettings.kNumMultibrushSlots);

                if(brushSettings.multibrushEnabled)
                {
                    for(int i = 0; i < brush.prefabSlots.Length; i++)
                    {
                        if(brush.prefabSlots[i].gameObject != null)
                        {
                            prefabList.Add(brush.prefabSlots[i].gameObject);
                        }
                    }
                }
                else
                {
                    GameObject prefab = brush.GetFirstAssociatedPrefab();
                    if(prefab != null)
                        prefabList.Add(prefab);
                }
            }


            if(brushSettings.brushOverlapCheckMode == OverlapCheckMode.Distance)
            {
                octree.IntersectSphere(bounds.center, distance, (go) =>
                    {
                        if(go == null)
                        {
                            return true;
                        }

                        switch(brushSettings.brushOverlapCheckObjects)
                        {
                        case OverlapCheckObjects.SameObjects:
                            {
                                GameObject prefabRoot = PrefabUtility.FindPrefabRoot(go);
                                if(prefabRoot == null)
                                    return true;

                                if(!prefabList.Contains(PrefabUtility.GetPrefabParent(prefabRoot) as GameObject))
                                    return true;
                            }
                            break;
                        case OverlapCheckObjects.SamePlaceLayer:
                            if(go.layer != settings.prefabPlaceLayer)
                                return true;
                            break;
                        case OverlapCheckObjects.OtherLayers:
                            if(((1 << go.layer) & checkLayers) == 0)
                                return true;
                            break;
                        }

                        if((go.transform.position - bounds.center).magnitude < distance)
                        {
                            overlaps = true;
                            return false;
                        }

                        return true;
                    }
                );
            }
            else
            {
                octree.IntersectBounds(bounds,
                    (go) =>
                    {
                        if(go == null)
                        {
                            return true;
                        }

                        switch(brushSettings.brushOverlapCheckObjects)
                        {
                        case OverlapCheckObjects.SameObjects:
                            {
                                GameObject prefabRoot = PrefabUtility.FindPrefabRoot(go);
                                if(prefabRoot == null)
                                    return true;

                                if(!prefabList.Contains(PrefabUtility.GetPrefabParent(prefabRoot) as GameObject))
                                    return true;
                            }
                            break;
                        case OverlapCheckObjects.SamePlaceLayer:
                            if(go.layer != settings.prefabPlaceLayer)
                                return true;
                            break;
                        case OverlapCheckObjects.OtherLayers:
                            if(((1 << go.layer) & checkLayers) == 0)
                                return true;
                            break;
                        }
                            
                        overlaps = true;
                        return false;
                    });
            }
                
            return overlaps;
        }


        PlacedObjectInfo PlaceObject(RaycastInfo raycastInfo, GameObject hitObject, Brush brush)
        {
            if(!SlopeFilter(brush, raycastInfo.normal))
                return null;
            
            int prefabSlot = brush.GetPrefabSlotForPlace();
            if(prefabSlot == -1 || brush.prefabSlots[prefabSlot].gameObject == null)
                return null;
            
            
            GameObject gameObject = PrefabUtility.InstantiatePrefab(brush.prefabSlots[prefabSlot].gameObject) as GameObject;
            if(gameObject == null)
                return null;
            gameObject.AddComponent<PrefabPainterGrassInfo>();

            if (settings.overwritePrefabLayer)
            {
                Utility.ForAllInHierarchy(gameObject, go => { go.layer = settings.prefabPlaceLayer; });
            }

            PlacedObjectInfo placedObjectInfo = new PlacedObjectInfo(raycastInfo, gameObject, brush, prefabSlot);

            OrientObject(placedObjectInfo);
            PositionObject(raycastInfo, gameObject, brush.settings, prefabSlot);
            ScaleObject(raycastInfo, gameObject, brush.settings, prefabSlot);


            if(currentTool == PaintTool.Brush && brush.settings.brushOverlapCheck)
            {
                if(CheckOverlap(brush, brush.settings, GetObjectBounds(gameObject)))
                {
                    GameObject.DestroyImmediate(gameObject);
                    return null;
                }
            }


            brush.PrepareNextPrefabForPlace();

            octree.AddObjectFast(gameObject, settings.useAdditionalVertexStreams);

            Undo.RegisterCreatedObjectUndo(gameObject, "PP: Paint Prefabs");
            Utility.MarkActiveSceneDirty();

            return placedObjectInfo;
        }



		#if UNITY_5_4_OR_NEWER
		#else
        static IEnumerable<GameObject> SceneRoots()
        {            
            var prop = new HierarchyProperty(HierarchyType.GameObjects);
            var expanded = new int[0];
            while (prop.Next(expanded)) {
                yield return prop.pptrValue as GameObject;
            }
        }
		#endif

        void ParentObject(PlacedObjectInfo placedObjectInfo)
        {
            GameObject parentObject = null;


            switch(settings.placeUnder)
            {
            case Placement.HitObject:
                parentObject = currentHitObject;
                break;
            case Placement.CustomObject:
                parentObject = sceneSettings.parentForPrefabs;
                break;
            default: case Placement.World:
                break;
            }


            // Group Prefabs
            // find group object by name
            if(settings.groupPrefabs)
            {
                Transform group = null;
                string groupName = placedObjectInfo.brush.name + settings.groupName;

                if (parentObject != null)
                    group = parentObject.transform.Find(groupName);
                else {
		#if UNITY_5_4_OR_NEWER
					GameObject[] sceneRoots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
					foreach(GameObject root in sceneRoots)
					{
						if(root.name == groupName) {
							group = root.transform;
							break;
						}
					}

		#else
                    foreach(GameObject root in SceneRoots())
					{
                        if(root.name == groupName) {
                            group = root.transform;
                            break;
                        }
                    }
		#endif
                }            
                if (group == null)
                {
                    GameObject childObject = new GameObject(groupName);
                    if (parentObject != null)
                        childObject.transform.parent = parentObject.transform;
                    group = childObject.transform;
                }

				if(group.gameObject.layer != settings.prefabPlaceLayer)
					group.gameObject.layer = settings.prefabPlaceLayer;

                parentObject = group.gameObject;
            }


            if (placedObjectInfo.gameObject != null && parentObject != null && parentObject.transform != null)
                placedObjectInfo.gameObject.transform.SetParent(parentObject.transform, true);
        }


        Vector3 GetRightVector(Vector3 up, Vector3 forward)
        {
            return Vector3.Cross(forward, up).normalized;
        }

        void GetRightForward(Vector3 up, out Vector3 right, out Vector3 forward)
        {
            switch(settings.surfaceCoords)
			{
			default: case SurfaceCoords.AroundX:
				forward = Vector3.Cross(Vector3.right, up).normalized;
				if (forward.magnitude < 0.001f)
					forward = Vector3.forward;

				right = Vector3.Cross(up, forward).normalized;
				break;
			case SurfaceCoords.AroundY:
				right = Vector3.Cross(up, Vector3.up).normalized;
				if (right.magnitude < 0.001f)
					right = Vector3.right;

				forward = Vector3.Cross(right, up).normalized;
				break;
			case SurfaceCoords.AroundZ:
				right = Vector3.Cross(up, Vector3.forward).normalized;
				if (right.magnitude < 0.001f)
					right = Vector3.right;

				forward = Vector3.Cross(right, up).normalized;
				break;
			}
        }

		Bounds GetObjectBounds(GameObject gameObject)
		{
			Bounds bounds = new Bounds();
			bool found = false;

            Utility.ForAllInHierarchy(gameObject, (go) =>
				{
					Renderer renderer = go.GetComponent<Renderer>();
					Collider collider;
					RectTransform rectTransform;

					if (renderer != null)
					{
						if (!found) {
							bounds = renderer.bounds;
							found = true;
						} else {
							bounds.Encapsulate(renderer.bounds);
						}
					}
					else if ((collider = go.GetComponent<Collider>()) != null)
					{
						if (!found) {
							bounds = collider.bounds;
							found = true;
						} else {
							bounds.Encapsulate(collider.bounds);
						}                        
					}
					else if ((rectTransform = go.GetComponent<RectTransform>()) != null)
					{
						Vector3[] fourCorners = new Vector3[4];
						rectTransform.GetWorldCorners(fourCorners);
						Bounds rectBounds = new Bounds();

						rectBounds.center = fourCorners[0];
						rectBounds.Encapsulate(fourCorners[1]);
						rectBounds.Encapsulate(fourCorners[2]);
						rectBounds.Encapsulate(fourCorners[3]);

						if (!found) {
							bounds = rectBounds;
							found = true;
						} else {
							bounds.Encapsulate(rectBounds);
						}                        
					}
				});

			if (!found)
				return new Bounds(gameObject.transform.position, gameObject.transform.lossyScale);

			return bounds;
		}


		Vector3 GetObjectScaleFactor(GameObject gameObject)
		{
			Bounds bounds = GetObjectBounds(gameObject);
			Vector3 localScale = gameObject.transform.localScale;


			float size = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
			//float size = bounds.extents.magnitude * 2.0f;

			if (size != 0.0f)
				size = 1.0f / size;
			else
				size = 1.0f;

			return new Vector3(localScale.x * size, localScale.y * size, localScale.z * size);
		}
#endregion // Object Placement




#region Scene UI

        void GetHanlesOrientation(Vector3 surfaceNormal, OrientationMode mode, bool flip, out Vector3 upwards, out Vector3 right, out Vector3 forward)
        {
            switch(mode)
            {
            default:
            case OrientationMode.None:
            case OrientationMode.AlongBrushStroke:                
            case OrientationMode.AlongSurfaceNormal:
                upwards = surfaceNormal;
                break;
            case OrientationMode.X:
                upwards = new Vector3(1, 0, 0);
                break;
            case OrientationMode.Y:
                upwards = new Vector3(0, 1, 0);
                break;
            case OrientationMode.Z:
                upwards = new Vector3(0, 0, 1);
                break;
            }

            if (flip && mode != OrientationMode.AlongBrushStroke)
                upwards = -upwards;

            GetRightForward(upwards, out right, out forward);

        }

		void ArrowCap(int controlID, Vector3 position, Quaternion rotation, float size)
		{
			#if UNITY_5_6_OR_NEWER
			if(Event.current != null && (Event.current.type == EventType.Layout || Event.current.type == EventType.Repaint))
				Handles.ArrowHandleCap(controlID, position, rotation, size, Event.current.type);
			#else
			Handles.ArrowCap(controlID, position, rotation, size);
			#endif
		}

        void CircleCap(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            #if UNITY_5_6_OR_NEWER
            if(Event.current != null && (Event.current.type == EventType.Layout || Event.current.type == EventType.Repaint))
                Handles.CircleHandleCap(controlID, position, rotation, size, Event.current.type);
            #else
            Handles.CircleCap(controlID, position, rotation, size);
            #endif
        }

        void DotCap(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            #if UNITY_5_6_OR_NEWER
            if(Event.current != null && (Event.current.type == EventType.Layout || Event.current.type == EventType.Repaint))
                Handles.DotHandleCap(controlID, position, rotation, size, Event.current.type);
            #else
            Handles.DotCap(controlID, position, rotation, size);
            #endif
        } 

        void DrawBrushHandles(RaycastInfo hit, Brush brush)
        {
            if(!brush.settings.gridEnabled)
            {
                Handles.color = settings.uiColor;
                CircleCap (1, hit.point, Quaternion.LookRotation (hit.normal), brush.settings.brushRadius * brush.settings.brushOverallScale);

                Handles.color = new Color(0.5f, 0.5f, 0.5f, 0.1f);
                Handles.DrawSolidDisc(hit.point, hit.normal, brush.settings.brushRadius * brush.settings.brushOverallScale);
            }

            Vector3 upwards, forward, right;
            GetHanlesOrientation(hit.normal, brush.settings.orientationMode, brush.settings.flipOrientation, out upwards, out right, out forward);

            Handles.color = settings.uiColor;
            Handles.Label(hit.point, "    Br", Styles.precisePlaceTextStyle);

            DrawXYZCross(hit, upwards, right, forward);
        }

        void DrawEraseHandles(RaycastInfo hit, Brush brush)
        {
            Handles.color = new Color(0.5f, 0.5f, 0.5f, 0.1f);
            Handles.DrawSolidDisc(hit.point, hit.normal, settings.eraseBrushRadius);
            Handles.color = settings.uiColor;
            CircleCap (1, hit.point, Quaternion.LookRotation (hit.normal), settings.eraseBrushRadius);

            Handles.color = settings.uiColor;
            Handles.Label(hit.point, "    Er", Styles.precisePlaceTextStyle);

            Vector3 forward, right;
            GetRightForward(hit.normal, out right, out forward);

            float handleSize = HandleUtility.GetHandleSize (hit.point) * 0.4f;
            Handles.color = settings.uiColor;
            Handles.DrawLine(hit.point + right * handleSize, hit.point + right * -handleSize);
            Handles.DrawLine(hit.point + forward * handleSize, hit.point + forward * -handleSize);
        }

        void DrawSelectHandles(RaycastInfo hit, Brush brush)
        {
            Handles.color = new Color(0.5f, 0.5f, 0.5f, 0.1f);
            Handles.DrawSolidDisc(hit.point, hit.normal, settings.selectBrushRadius);
            Handles.color = settings.uiColor;
            CircleCap (1, hit.point, Quaternion.LookRotation (hit.normal), settings.selectBrushRadius);

            Handles.color = settings.uiColor;
            Handles.Label(hit.point, "    Sl", Styles.precisePlaceTextStyle);

            Vector3 forward, right;
            GetRightForward(hit.normal, out right, out forward);

            float handleSize = HandleUtility.GetHandleSize (hit.point) * 0.4f;
            Handles.color = settings.uiColor;
            Handles.DrawLine(hit.point + right * handleSize, hit.point + right * -handleSize);
            Handles.DrawLine(hit.point + forward * handleSize, hit.point + forward * -handleSize);
        }

        void DrawMaskedHandles(RaycastInfo hit, Brush brush)
        {
            float handleSize = HandleUtility.GetHandleSize (hit.point) * 0.2f;

            Vector3 forward, right;
            GetRightForward(hit.normal, out right, out forward);

            Handles.color = settings.uiColor;
            CircleCap (1, hit.point, Quaternion.LookRotation (hit.normal), handleSize * 0.5f);
            Handles.DrawLine(hit.point + right * handleSize, hit.point + right * -handleSize);
            Handles.DrawLine(hit.point + forward * handleSize, hit.point + forward * -handleSize);
            if (currentHitObject != null)
                Handles.Label(hit.point, "      Layer: " + LayerMask.LayerToName(currentHitObject.layer), Styles.handlesTextStyle);
        }

        void DrawErrorHandles(RaycastInfo hit, string message)
        {
            float handleSize = HandleUtility.GetHandleSize (hit.point) * 0.2f;

            Vector3 forward, right;
            GetRightForward(hit.normal, out right, out forward);

            Handles.color = settings.uiColor;
            CircleCap (1, hit.point, Quaternion.LookRotation (hit.normal), handleSize * 0.5f);
            Handles.DrawLine(hit.point + right * handleSize, hit.point + right * -handleSize);
            Handles.DrawLine(hit.point + forward * handleSize, hit.point + forward * -handleSize);
            Handles.Label(hit.point, "      " +  message, Styles.handlesTextStyle);
        }


		void DrawPickObjectHandles(RaycastInfo hit)
		{
			Vector3 forward, right;
			GetRightForward(hit.normal, out right, out forward);

			float handleSize = HandleUtility.GetHandleSize (hit.point) * 0.3f;

			Handles.color = Color.green;
			ArrowCap(0, hit.point, Quaternion.LookRotation(hit.normal, right), handleSize * 2f);
			Handles.color = Color.red;
			Handles.DrawAAPolyLine(3, hit.point + right * handleSize, hit.point + right * -handleSize);
			Handles.color = Color.blue;
			Handles.DrawAAPolyLine(3, hit.point + forward * handleSize, hit.point + forward * -handleSize);
			Handles.color = Color.red;
			Handles.Label(hit.point, "      " +  onPickObjectMessage, Styles.handlesTextStyle);
		}


        void DrawHandles(RaycastInfo hit, Brush brush, bool hasMultipleSelectedBrushes, bool hotControl)
        {
            Color handlesColor = Handles.color;
            Matrix4x4 handlesMatrix = Handles.matrix;

			if(onPickObject && hit.isHit)
			{
				DrawPickObjectHandles(hit);
				return;
			}

            switch(currentTool)
            {
            case PaintTool.Brush:
                {
                    if(brush == null)
                    {
                        DrawErrorHandles(hit, Strings.selectBrush);
                        return;
                    }

                    if(hasMultipleSelectedBrushes)
                    {
                        DrawErrorHandles(hit, Strings.multiSelBrush);
                        return;
                    }


                    if(brush.settings.gridEnabled)
                        DrawGrid(brush);

                    if(hit.isHitMaskedLayer && !hotControl)
                    {
                        DrawMaskedHandles(hit, brush);
                        return;
                    }

//                    if(brush.settings.slopeEnabled)
//                    {
//                        if (hit.isHitTargetLayer && !SlopeFilter(brush, hit.normal)) {
//                            DrawErrorHandles(hit, "Slope Filter Angle: " + SlopeAngle(brush, hit.normal).ToString("F2") + "\u00b0");
//                            return;
//                        }
//                    }

                    
                    DrawBrushHandles(hit, brush);
                }
                break;
            case PaintTool.PrecisePlace:
                {
                    if(brush == null)
                    {
                        DrawErrorHandles(hit, Strings.selectBrush);
                        return;
                    }

                    if(hasMultipleSelectedBrushes)
                    {
                        DrawErrorHandles(hit, Strings.multiSelBrush);
                        return;
                    }

                    if(hit.isHitMaskedLayer)
                    {
                        DrawMaskedHandles(hit, brush);
                        return;
                    }

                    if(brush.settings.gridEnabled)
                        DrawGrid(brush);
                    
                    DrawPrecisePlaceHandles(brush);
                }
                break;
            case PaintTool.Erase:
                {
                    if(!settings.eraseByLayer)
                    {
                        if(brush == null)
                        {
                            DrawErrorHandles(hit, Strings.selectBrush);
                            return;
                        }
                    }

                    DrawEraseHandles(hit, brush);
                }
                break;
            case PaintTool.Select:
                {                    
                    if(!settings.selectByLayer)
                    {
                        if(brush == null)
                        {
                            DrawErrorHandles(hit, Strings.selectBrush);
                            return;
                        }
                    }

                    DrawSelectHandles(hit, brush);
                }
                break;
            }

            Handles.color = handlesColor;
            Handles.matrix = handlesMatrix;
        }

        void DrawPrecisePlaceHandles(Brush brush)
        {
            Vector3 upwards, forward, right;

            if(precisePlaceTool.placedObjectInfo == null)
            {                
                Handles.color = settings.uiColor;
                Handles.Label(currentRaycast.point, "    Pp", Styles.precisePlaceTextStyle);

                GetRightForward(currentRaycast.normal, out right, out forward);
                DrawXYZCross(currentRaycast, currentRaycast.normal, right, forward);
                return;
            }

            GetRightForward(currentRaycast.normal, out right, out forward);

            Handles.color = settings.uiColor;
            CircleCap (1, precisePlaceTool.placedObjectInfo.raycastInfo.point, Quaternion.LookRotation (precisePlaceTool.placedObjectInfo.raycastInfo.normal), precisePlaceTool.radius);

            Handles.color = settings.uiColor;
            Handles.DrawDottedLine(precisePlaceTool.placedObjectInfo.raycastInfo.point, precisePlaceTool.point, 4.0f);
            Handles.DrawDottedLine(precisePlaceTool.placedObjectInfo.raycastInfo.point, precisePlaceTool.placedObjectInfo.raycastInfo.point + forward * precisePlaceTool.radius, 4.0f);


            
			Handles.color = new Color(0, 0, 0, 0.2f);
            Handles.DrawSolidArc(precisePlaceTool.placedObjectInfo.raycastInfo.point, precisePlaceTool.placedObjectInfo.raycastInfo.normal, forward, precisePlaceTool.angle, precisePlaceTool.radius);


            Handles.color = settings.uiColor;
            Handles.Label(precisePlaceTool.point, "    Angle: " + (precisePlaceTool.angle).ToString("F2"), Styles.precisePlaceTextStyle);
            Handles.Label(precisePlaceTool.point, "\n    Radius: " + (precisePlaceTool.radius).ToString("F2"), Styles.precisePlaceTextStyle);


            GetHanlesOrientation(currentRaycast.normal, brush.settings.orientationMode, brush.settings.flipOrientation, out upwards, out right, out forward);
            DrawXYZCross(currentRaycast, upwards, right, forward);
        }



        void DrawXYZCross(RaycastInfo hit, Vector3 upwards, Vector3 right, Vector3 forward)
		{
			float handleSize = HandleUtility.GetHandleSize (hit.point) * 0.5f;

			Handles.color = Color.green;
            Handles.DrawAAPolyLine(3, hit.point + upwards * handleSize, hit.point + upwards * -handleSize * 0.2f);
			Handles.color = Color.red;
            Handles.DrawAAPolyLine(3, hit.point + right * handleSize, hit.point + right * -handleSize * 0.2f);
			Handles.color = Color.blue;
            Handles.DrawAAPolyLine(3, hit.point + forward * handleSize, hit.point + forward * -handleSize * 0.2f);
		}


        Vector3 GetGridNormalVector(Brush brush)
        {
            switch(brush.settings.gridPlane)
            {
            case GridPlane.XY:
                return new Vector3(0, 0, 1);
            case GridPlane.XZ:
                return new Vector3(0, 1, 0);
            case GridPlane.YZ:
                return new Vector3(1, 0, 0);
            }

            if(brush.settings.gridNormal.magnitude < 0.001f)
                return new Vector3(0, 1, 0);

            return brush.settings.gridNormal.normalized;
        }

        void DrawGrid(Brush brush)
        {
            if(!grid.originRaycastInfo.isHit)
                return;
            

            int kSize           = 10;
            int halfSize        = kSize / 2;
            float halfSizeInv   = 1.0f / halfSize;
            Vector3 gridStep    = new Vector3(brush.settings.gridStep.x, brush.settings.gridStep.y, 1);
            Vector3 gridNormal  = GetGridNormalVector(brush);


            Matrix4x4 gridMatrix = Matrix4x4.TRS(grid.visualOrigin, Quaternion.AngleAxis(brush.settings.gridAngle, gridNormal) * Quaternion.LookRotation(gridNormal), gridStep);

            Vector3 localHitPoint = gridMatrix.inverse.MultiplyPoint(currentRaycast.point);

            for(int x = -halfSize; x <= halfSize; x++)
            {
                for(int y = -halfSize; y <= halfSize; y++)
                {
                    Vector3 localPoint = new Vector3(x, y, 0);

                    Vector3 point = gridMatrix.MultiplyPoint(localPoint);

                    float alpha = Mathf.Clamp01((1.0f - (localPoint - localHitPoint).magnitude * halfSizeInv)) * 1.0f;

                    Handles.color = new Color(1, 1, 1, alpha) * settings.uiColor;
                    DotCap(0, point, Quaternion.identity, HandleUtility.GetHandleSize (point) * 0.03f);

                }
            }

            float gridBaseAlpha = 0.5f;
            halfSize += 1;
            for(int x = -(halfSize-1); x < halfSize; x++)
            {
                Vector3 point1 = new Vector3(x, -halfSize, 0);
                Vector3 point2 = new Vector3(x, 0, 0);
                Vector3 point3 = new Vector3(x, halfSize, 0);

                Vector3 point4 = new Vector3(-halfSize, x, 0);
                Vector3 point5 = new Vector3(0, x, 0);
                Vector3 point6 = new Vector3(halfSize, x, 0);

                GL.PushMatrix();
                GL.MultMatrix(gridMatrix);
                GL.Begin(GL.LINES);

                GL.Color(new Color(0, 0, 0, gridBaseAlpha * Mathf.Clamp01(1.0f - (point1 - localHitPoint).magnitude * halfSizeInv)));
                GL.Vertex(point1);
                GL.Color(new Color(0, 0, 0, gridBaseAlpha * Mathf.Clamp01(1.0f - (point2 - localHitPoint).magnitude * halfSizeInv)));
                GL.Vertex(point2);
                GL.Vertex(point2);
                GL.Color(new Color(0, 0, 0, gridBaseAlpha * Mathf.Clamp01(1.0f - (point3 - localHitPoint).magnitude * halfSizeInv)));
                GL.Vertex(point3);


                GL.Color(new Color(0, 0, 0, gridBaseAlpha * Mathf.Clamp01(1.0f - (point4 - localHitPoint).magnitude * halfSizeInv)));
                GL.Vertex(point4);
                GL.Color(new Color(0, 0, 0, gridBaseAlpha * Mathf.Clamp01(1.0f - (point5 - localHitPoint).magnitude * halfSizeInv)));
                GL.Vertex(point5);
                GL.Vertex(point5);
                GL.Color(new Color(0, 0, 0, gridBaseAlpha * Mathf.Clamp01(1.0f - (point6 - localHitPoint).magnitude * halfSizeInv)));
                GL.Vertex(point6);


                GL.End();
                GL.PopMatrix();
            }

            if(!grid.inDeadZone)
            {
                // Draw origin point
                Handles.color = settings.uiColor;
                DotCap(0, grid.originRaycastInfo.point, Quaternion.identity, HandleUtility.GetHandleSize (grid.originRaycastInfo.point) * 0.05f);
                Handles.color = Color.white;
                DotCap(0, grid.originRaycastInfo.point, Quaternion.identity, HandleUtility.GetHandleSize (grid.originRaycastInfo.point) * 0.03f);

                Handles.Label(currentRaycast.point, "\n\n     P: " + grid.originRaycastInfo.point, Styles.handlesTextStyle);
            }


//            Vector2 guiPoint = HandleUtility.WorldToGUIPoint(currentRaycast.point);//
//            Handles.BeginGUI();
//            Rect rect  = HandleUtility.WorldPointToSizedRect(currentRaycast.point, new GUIContent("1!!!!!!!!!!!!"), Styles.handlesTextStyle);
//            GUI.Label(rect, new GUIContent("1!!!!!!!!!!"), Styles.handlesTextStyle);
//            Handles.EndGUI();


        }




        void UpdateGrid(Brush brush)
        {
            Vector3 gridOrigin      = new Vector3(brush.settings.gridOrigin.x, brush.settings.gridOrigin.y, 0);
            Vector3 gridStep        = new Vector3(brush.settings.gridStep.x, brush.settings.gridStep.y, 1);
            Vector3 gridNormal      = GetGridNormalVector(brush);
            float   gridAngle       = brush.settings.gridAngle;
            Vector3 hitPoint        = currentRaycast.point;
            float   raycastHeight   = settings.gridRaycastHeight;


            if(!currentRaycast.isHit)
            {
                grid.originHitObject = null;
                grid.originRaycastInfo.isHitTargetLayer = false;
                grid.originRaycastInfo.isHitMaskedLayer = false;
                return;
            }


            Matrix4x4 gridMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(gridAngle, gridNormal) * Quaternion.LookRotation(gridNormal), Vector3.one)
                                 * Matrix4x4.TRS(gridOrigin, Quaternion.identity, gridStep);

            // transform point to grid space
            Vector3 gridSpacePoint = gridMatrix.inverse.MultiplyPoint(hitPoint);


            grid.inDeadZone = (new Vector2(Mathf.Round(gridSpacePoint.x), Mathf.Round(gridSpacePoint.y)) - new Vector2(gridSpacePoint.x, gridSpacePoint.y)).magnitude > Grid.kDeadZoneSize;
            

            // round point values
            gridSpacePoint = new Vector3(Mathf.Round(gridSpacePoint.x), Mathf.Round(gridSpacePoint.y), gridSpacePoint.z);

            // transform point back to world space
            Vector3 snappedHitPoint = gridMatrix.MultiplyPoint(gridSpacePoint);


            Ray ray;

            // offset raycast 
            if(Vector3.Dot(currentRaycast.normal, gridNormal) > 0f)
            {
                snappedHitPoint += gridNormal * raycastHeight;
                ray = new Ray(snappedHitPoint, -gridNormal);
            }
            else 
            {
                snappedHitPoint -= gridNormal * raycastHeight;
                ray = new Ray(snappedHitPoint, gridNormal);
            }

            grid.originHitObject = Raycast(ray, out grid.originRaycastInfo, settings.paintLayers.value, settings.ignoreLayers.value);

            // if no hit - try slightly shift the ray
            // fixes situations where ray just touch objects
            if(!grid.originRaycastInfo.isHit)
            {
                for(int i = 0; i < 3; i++)
                {
                    Ray shiftedRay = new Ray(ray.origin + UnityEngine.Random.onUnitSphere * 0.001f, ray.direction);
                    grid.originHitObject = Raycast(shiftedRay, out grid.originRaycastInfo, settings.paintLayers.value, settings.ignoreLayers.value);
                    if(grid.originRaycastInfo.isHit)
                        break;
                }
            }

            // make nice values
            grid.originRaycastInfo.point = Utility.RoundVector(grid.originRaycastInfo.point, 3);


            RaycastInfo visualRaycast = new RaycastInfo();
			Raycast(grid.originRaycastInfo.ray, out visualRaycast, settings.paintLayers.value, ~settings.paintLayers.value);
			grid.visualOrigin = visualRaycast.point;
        }



        void OnToolEnabled(PaintTool tool)
        {
            switch(tool)
            {
            case PaintTool.PrecisePlace:
                break;
            }
        }

        void OnToolDisabled(PaintTool tool)
        {
            switch(tool)
            {
            case PaintTool.PrecisePlace:
                break;
            }
        }


		void PickObject(string message, Action<GameObject, RaycastInfo> action)
		{
			onPickObject = true;
			onPickObjectAction = action;
			onPickObjectMessage = message;

            UnityEditor.Tools.current = Tool.None;

			octree.Populate(GameObject.FindObjectsOfType<GameObject>(), settings.useAdditionalVertexStreams);
		}


        void OnSceneGUI(SceneView sceneView)
        {            
            // if any object selected - abort paint
            if (Selection.objects.Length > 0 && IsPaintToolEnabled() && currentTool != PaintTool.Select)
            {
                currentTool = PaintTool.None;
                selectedObjects = null;
                Repaint();
            }


            int layersMask = settings.paintLayers.value;

            switch(currentTool)
            {
            case PaintTool.Erase:
            case PaintTool.Select:
                layersMask = ~0;
                break;
            default:
                break;
            }

            bool hasMultipleSelectedBrushes = settings.GetActiveTab().HasMultipleSelectedBrushes();
            Brush brush = settings.GetActiveTab().GetFirstSelectedBrush();


            int controlID = GUIUtility.GetControlID(s_PrefabPainterEditorHash, FocusType.Passive);
            Event e = Event.current;
            EventType eventType = e.GetTypeForControl(controlID);


            if(GUIUtility.hotControl != controlID)
            {
                precisePlaceTool.placedObjectInfo = null;
                eraseTool.prefabList.Clear();
            }


            HandleKeyboardEvents();



            switch (eventType)
            {
            case EventType.MouseDown:
            case EventType.MouseDrag:
                {
                    if (GUIUtility.hotControl != 0 && GUIUtility.hotControl != controlID)
                        return;
                    
                    if (eventType == EventType.MouseDrag && GUIUtility.hotControl != controlID)
                        return;
                    
                    if (e.button != 0 || Event.current.alt)
                        return;

					if(onPickObject)
					{
						currentHitObject = Raycast(HandleUtility.GUIPointToWorldRay (e.mousePosition), out currentRaycast, layersMask, settings.ignoreLayers.value);
						if(onPickObjectAction != null)
						{
							onPickObjectAction(currentHitObject, currentRaycast);
							onPickObjectAction = null;
						}
						onPickObject = false;
						return;
					}

                    if (!IsPaintToolEnabled())
                        return;
                    
                    if (eventType == EventType.MouseDown)
                        GUIUtility.hotControl = controlID;

                    HandleUtility.Repaint();

                    switch(currentTool)
                    {
                    case PaintTool.Brush:
                        {
                            currentHitObject = Raycast(HandleUtility.GUIPointToWorldRay (e.mousePosition), out currentRaycast, layersMask, settings.ignoreLayers.value);

                            if (brush == null || hasMultipleSelectedBrushes)
                                return;



                            if(brush.settings.gridEnabled)
                            {
                                UpdateGrid(brush);

                                brushTool.raycast = grid.originRaycastInfo;
                                currentHitObject = grid.originHitObject;
                            }
                            else
                            {
                                brushTool.raycast = currentRaycast;
                            }



                            if (eventType == EventType.MouseDown)
                            {
                                brushTool.dragDistance = 0;
                                brushTool.strokeDirection = new Vector3(1, 0, 0);
                                brushTool.lastPlacedObjectInfo = null;
                                brushTool.firstNormal = new Vector3(0, 0, 0);

                                brush.BeginStroke();

                                if (brushTool.raycast.isHitTargetLayer)
                                {                                    
                                    PlacedObjectInfo placedObjectInfo = PlaceObject(brushTool.raycast, currentHitObject, brush);
                                    if(placedObjectInfo != null)
                                    {
                                        ParentObject(placedObjectInfo);
                                        brushTool.lastPlacedObjectInfo = placedObjectInfo;
                                        brushTool.strokeDirectionRefPoint = brushTool.raycast.point;
                                    }
                                }

                                brushTool.prevRaycast = brushTool.raycast;
                            }

                            // loose surface - break stroke - don't orient last object 
                            if(!brushTool.raycast.isHit || !brushTool.prevRaycast.isHit)
                            {
                                brushTool.lastPlacedObjectInfo = null;
                            }

                            if(brushTool.firstNormal.magnitude < 0.1f && brushTool.raycast.isHit)
                            {
                                brushTool.firstNormal = brushTool.raycast.normal;
                            }


                            if (eventType == EventType.MouseDrag && (brushTool.raycast.isHit || brushTool.prevRaycast.isHit))
                            {
                                Vector3 hitPoint     = brushTool.raycast.point;
                                Vector3 lastHitPoint = brushTool.prevRaycast.point;
                                Vector3 hitNormal    = brushTool.raycast.isHit ? brushTool.raycast.normal : brushTool.prevRaycast.normal;

                                bool isTwoPoints = true;

                                // predict point
                                if (!brushTool.raycast.isHit)
                                {
                                    if (!brushTool.prevRaycast.IntersectsHitPlane(brushTool.raycast.ray, out hitPoint))
                                        isTwoPoints = false;
                                }

                                // predict point
                                if (!brushTool.prevRaycast.isHit)
                                {
                                    if (!brushTool.raycast.IntersectsHitPlane(brushTool.prevRaycast.ray, out lastHitPoint))
                                        isTwoPoints = false;
                                }


                                if(brush.settings.gridEnabled)
                                {
                                    if(brushTool.raycast.isHitTargetLayer && !grid.inDeadZone &&
                                        (brushTool.lastPlacedObjectInfo == null || !Utility.IsVector3Equal(hitPoint, brushTool.lastPlacedObjectInfo.raycastInfo.point)))
                                    {
                                        PlacedObjectInfo placedObjectInfo = PlaceObject(brushTool.raycast, currentHitObject, brush);
                                        if(placedObjectInfo != null)
                                        {
                                            if(brushTool.lastPlacedObjectInfo != null)
                                                brushTool.strokeDirection = (brushTool.lastPlacedObjectInfo.raycastInfo.point - placedObjectInfo.raycastInfo.point).normalized;

                                            // re-orient last object along stroke
                                            if (brush.settings.orientationMode == OrientationMode.AlongBrushStroke && brushTool.lastPlacedObjectInfo != null)
                                            {
                                                brushTool.lastPlacedObjectInfo.gameObject.transform.eulerAngles = Vector3.zero;
                                                PositionObject(brushTool.lastPlacedObjectInfo.raycastInfo, brushTool.lastPlacedObjectInfo.gameObject, brushTool.lastPlacedObjectInfo.brush.settings, brushTool.lastPlacedObjectInfo.prefabSlot);
                                                OrientObject(brushTool.lastPlacedObjectInfo);
                                            }

                                            ParentObject(placedObjectInfo);
                                            brushTool.lastPlacedObjectInfo = placedObjectInfo;
                                            brushTool.strokeDirectionRefPoint = hitPoint;
                                        }
                                    }
                                }
                                else
                                {
                                    if(isTwoPoints && !Utility.IsVector3Equal(hitPoint, lastHitPoint))
                                    { 
                                        float   brushSpacing    = Mathf.Max(0.01f, brush.settings.brushSpacing * brush.settings.brushOverallScale);
                                        float   brushRadius     = brush.settings.brushRadius * brush.settings.brushOverallScale;
                                        Vector3 moveVector      = (hitPoint - lastHitPoint);
                                        float   moveLenght      = moveVector.magnitude;
                                        Vector3 moveDirection   = moveVector.normalized;


                                        brushTool.strokeDirection = (hitPoint - brushTool.strokeDirectionRefPoint).normalized;

                                        // re-orient last object along stroke
                                        if (brush.settings.orientationMode == OrientationMode.AlongBrushStroke && brushTool.lastPlacedObjectInfo != null)
                                        {
                                            brushTool.lastPlacedObjectInfo.gameObject.transform.eulerAngles = Vector3.zero;
                                            PositionObject(brushTool.lastPlacedObjectInfo.raycastInfo, brushTool.lastPlacedObjectInfo.gameObject, brushTool.lastPlacedObjectInfo.brush.settings, brushTool.lastPlacedObjectInfo.prefabSlot);
                                            OrientObject(brushTool.lastPlacedObjectInfo);
                                        }


                                        if (brushTool.dragDistance + moveLenght >= brushSpacing)
                                        {
                                            float d = brushSpacing - brushTool.dragDistance;
                                            Vector3 drawPoint = lastHitPoint + moveDirection * d;
                                            brushTool.dragDistance = 0;
                                            moveLenght -= d;

                                            Vector3 randomPoint = drawPoint + Vector3.ProjectOnPlane(UnityEngine.Random.onUnitSphere, hitNormal) * brushRadius * 0.5f;

                                            RaycastInfo raycastInfo = new RaycastInfo();
                                            currentHitObject = Raycast (WorldPointToRay(randomPoint), out raycastInfo, layersMask, settings.ignoreLayers.value);

                                            if (raycastInfo.isHitTargetLayer)
                                            {
                                                PlacedObjectInfo placedObjectInfo = PlaceObject(raycastInfo, currentHitObject, brush);
                                                if(placedObjectInfo != null)
                                                {
                                                    ParentObject(placedObjectInfo);
                                                    brushTool.lastPlacedObjectInfo = placedObjectInfo;
                                                    brushTool.strokeDirectionRefPoint = hitPoint;
                                                }
                                            }

                                            while (moveLenght >= brushSpacing)
                                            {
                                                moveLenght -= brushSpacing;
                                                drawPoint += moveDirection * brushSpacing;

                                                randomPoint = drawPoint + Vector3.ProjectOnPlane(UnityEngine.Random.onUnitSphere, hitNormal) * brushRadius * 0.5f;

                                                currentHitObject = Raycast (WorldPointToRay(randomPoint), out raycastInfo, layersMask, settings.ignoreLayers.value);

                                                if (raycastInfo.isHitTargetLayer)
                                                {
                                                    PlacedObjectInfo placedObjectInfo = PlaceObject(raycastInfo, currentHitObject, brush);
                                                    if(placedObjectInfo != null)
                                                    {
                                                        ParentObject(placedObjectInfo);
                                                        brushTool.lastPlacedObjectInfo = placedObjectInfo;
                                                        brushTool.strokeDirectionRefPoint = hitPoint;
                                                    }
                                                }
                                            }
                                        }

                                        brushTool.dragDistance += moveLenght;
                                    }
                                }
                            }
                            brushTool.prevRaycast = brushTool.raycast;
                            e.Use();
                        }
                        break;
                    case PaintTool.PrecisePlace:
                        {
                            if(precisePlaceTool.placedObjectInfo == null)
                                currentHitObject = Raycast(HandleUtility.GUIPointToWorldRay (e.mousePosition), out currentRaycast, layersMask, settings.ignoreLayers.value);

                            if (brush == null || hasMultipleSelectedBrushes)
                                return;

                            bool snapScale    = e.shift ? !settings.ppSnapScale : settings.ppSnapScale;
                            bool snapRotation = e.control ? !settings.ppSnapRotation : settings.ppSnapRotation;


                            if (eventType == EventType.MouseDown)
                            {
                                if(brush.settings.gridEnabled)
                                {
                                    currentRaycast = grid.originRaycastInfo;
                                    currentHitObject = grid.originHitObject;
                                }
                                
                                if (currentRaycast.isHitTargetLayer)
                                {
                                    brush.BeginStroke();

                                    precisePlaceTool.placedObjectInfo = PlaceObject(currentRaycast, currentHitObject, brush);
                                    if(precisePlaceTool.placedObjectInfo == null)
                                        return;
                                    
                                    precisePlaceTool.orienation = precisePlaceTool.placedObjectInfo.gameObject.transform.rotation;

                                    if(brush.settings.ppFixedScale)
                                    {
                                        GameObject gameObject = precisePlaceTool.placedObjectInfo.gameObject;

                                        if (brush.settings.scaleTransformMode == TransformMode.Relative)
                                            gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x * brush.settings.ppFixedScaleValue.x,
                                                gameObject.transform.localScale.y * brush.settings.ppFixedScaleValue.y,
                                                gameObject.transform.localScale.z * brush.settings.ppFixedScaleValue.z);
                                        else
                                            gameObject.transform.localScale = brush.settings.ppFixedScaleValue;
                                    }

                                    ParentObject(precisePlaceTool.placedObjectInfo);

                                    precisePlaceTool.scaleFactor = GetObjectScaleFactor(precisePlaceTool.placedObjectInfo.gameObject);

                                    if(!brush.settings.ppFixedScale)
                                        precisePlaceTool.placedObjectInfo.gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

                                    brush.EndStroke();
                                }
                            }


                            if (precisePlaceTool.placedObjectInfo != null)
                            {
                                if(precisePlaceTool.placedObjectInfo.raycastInfo.IntersectsHitPlane(HandleUtility.GUIPointToWorldRay (e.mousePosition), out precisePlaceTool.point))
                                {
                                    Vector3 vector = precisePlaceTool.point - precisePlaceTool.placedObjectInfo.raycastInfo.point;
                                    float vectorLength = vector.magnitude;

                                    if (vectorLength < 0.01f)
                                    {
                                        vector = Vector3.up * 0.01f;
                                        vectorLength = 0.01f;
                                    }

                                    precisePlaceTool.radius = vectorLength;

                                    Vector3 forward, right;
                                    GetRightForward(precisePlaceTool.placedObjectInfo.raycastInfo.normal, out right, out forward);

                                    precisePlaceTool.angle = Vector3.Angle(forward, vector.normalized);
                                    if (Vector3.Dot(vector.normalized, right) < 0.0f)
                                        precisePlaceTool.angle = -precisePlaceTool.angle;

                                    // snap angle
                                    if (snapRotation && settings.ppSnapRotationValue != 0)
                                        precisePlaceTool.angle = Mathf.Round(precisePlaceTool.angle / settings.ppSnapRotationValue) * settings.ppSnapRotationValue;
                                    // snap scale
                                    if (snapScale && settings.ppSnapScaleValue != 0) {
                                        precisePlaceTool.radius = Mathf.Round(precisePlaceTool.radius / settings.ppSnapScaleValue) * settings.ppSnapScaleValue;
                                        precisePlaceTool.radius = Mathf.Max(precisePlaceTool.radius, 0.01f);
                                    }

                                    Vector3 orientationVector = Vector3.up;
                                    switch(brush.settings.ppOrientationMode)
                                    {
                                    case PrecisePlaceOrientationMode.AlongSurfaceNormal:
                                        orientationVector = precisePlaceTool.placedObjectInfo.raycastInfo.normal;
                                        break;                                    
                                    case PrecisePlaceOrientationMode.X:
                                        orientationVector = new Vector3(1, 0, 0);
                                        break;                                    
                                    case PrecisePlaceOrientationMode.Y:
                                        orientationVector = new Vector3(0, 1, 0);
                                        break;
                                    case PrecisePlaceOrientationMode.Z:
                                        orientationVector = new Vector3(0, 0, 1);
                                        break;
                                    }
                                    if(brush.settings.flipOrientation)
                                        orientationVector = -orientationVector;

                                    if(!brush.settings.ppFixedRotation)
                                    {
                                        precisePlaceTool.placedObjectInfo.gameObject.transform.eulerAngles = Vector3.zero;
                                        precisePlaceTool.placedObjectInfo.gameObject.transform.localEulerAngles = Vector3.zero;
                                        precisePlaceTool.placedObjectInfo.gameObject.transform.rotation =  Quaternion.AngleAxis(precisePlaceTool.angle, orientationVector) * precisePlaceTool.orienation;
                                    }

                                    if(!brush.settings.ppFixedScale)
                                    {
                                        precisePlaceTool.placedObjectInfo.gameObject.transform.localScale = new Vector3(precisePlaceTool.radius * 2.0f * precisePlaceTool.scaleFactor.x,
                                            precisePlaceTool.radius * 2.0f * precisePlaceTool.scaleFactor.y, precisePlaceTool.radius * 2.0f * precisePlaceTool.scaleFactor.z);
                                    }
                                }
                            }

                            e.Use();
                        }
                        break;
                    case PaintTool.Erase:
                        {
                            if(settings.eraseByLayer)
                            {
                                currentHitObject = Raycast(HandleUtility.GUIPointToWorldRay (e.mousePosition), out currentRaycast, layersMask, settings.ignoreLayers.value);
                                if (!currentRaycast.isHit)
                                    return;
                                
                                octree.IntersectSphere(currentRaycast.point, settings.eraseBrushRadius, (go) =>
                                {
                                    if(go == null)
                                        return true;

                                    GameObject prefabRoot = PrefabUtility.FindPrefabRoot(go);
                                    if(prefabRoot == null)
                                        return true;

                                    if(((1 << prefabRoot.layer) & settings.eraseLayers.value) == 0)
                                        return true;
                                        
                                    Undo.DestroyObjectImmediate(prefabRoot);
                                    return true;
                                });
                            }
                            else
                            {
                                if (brush == null)
                                    return;

                                if (eventType == EventType.MouseDown)
                                {
                                    eraseTool.prefabList.Clear();

                                    // on MouseDown make list of selected prefabs
                                    settings.GetActiveTab().brushes.ForEach((b) => {
                                        if(b.selected)
                                        {
                                            if(b.settings.multibrushEnabled)
                                            {
                                                for(int i = 0; i < b.prefabSlots.Length; i++)
                                                {
                                                    if(b.prefabSlots[i].gameObject != null && b.settings.multibrushSlots[i].enabled)
                                                    {
                                                        eraseTool.prefabList.Add(b.prefabSlots[i].gameObject);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                GameObject prefab = b.GetFirstAssociatedPrefab();
                                                if(prefab != null)
                                                    eraseTool.prefabList.Add(prefab);
                                            }
                                        }
                                    });
                                }

                                currentHitObject = Raycast(HandleUtility.GUIPointToWorldRay (e.mousePosition), out currentRaycast, layersMask, settings.ignoreLayers.value);
                                if (!currentRaycast.isHit)
                                    return;
                                
                                octree.IntersectSphere(currentRaycast.point, settings.eraseBrushRadius, (go) =>
                                {
                                        if(go == null)
                                            return true;
                                        
                                        GameObject prefabRoot = PrefabUtility.FindPrefabRoot(go);
                                        if(prefabRoot == null)
                                            return true;

                                        if(eraseTool.prefabList.Contains(PrefabUtility.GetPrefabParent(prefabRoot) as GameObject))
                                        {
                                            Undo.DestroyObjectImmediate(prefabRoot);
                                        }
                                        return true;
                                });
                            }
                            
                            e.Use();
                        }
                        break;
                    case PaintTool.Select:
                        {
                            bool selectionChanged = false;

                            if (eventType == EventType.MouseDown)
                            {
                                switch(settings.selectMode)
                                {
                                case SelectMode.Replace:
                                    selectionTool.selectedObjects.Clear();
                                    selectionChanged = true;
                                    break;
								default:
									selectionTool.selectedObjects.Clear();
									selectionTool.selectedObjects.AddRange(Selection.gameObjects);
									break;
                                }
                            }

                            if(settings.selectByLayer)
                            {
                                currentHitObject = Raycast(HandleUtility.GUIPointToWorldRay (e.mousePosition), out currentRaycast, layersMask, settings.ignoreLayers.value);
                                if (!currentRaycast.isHit)
                                    return;

                                octree.IntersectSphere(currentRaycast.point, settings.selectBrushRadius, (go) =>
                                    {
                                        if(go == null)
                                            return true;

                                        GameObject prefabRoot = PrefabUtility.FindPrefabRoot(go);
                                        if(prefabRoot == null)
                                            return true;
                                        
                                        if(((1 << prefabRoot.layer) & settings.selectLayers.value) == 0)
                                            return true;

                                        switch(settings.selectMode)
                                        {
                                        case SelectMode.Replace:
                                        case SelectMode.Add:
                                            selectionTool.selectedObjects.Add(prefabRoot);
                                            selectionChanged = true;
                                            break;
                                        case SelectMode.Substract:
                                            if(selectionTool.selectedObjects.Contains(prefabRoot))
                                            {
                                                selectionTool.selectedObjects.Remove(prefabRoot);
                                                selectionChanged = true;
                                            }
                                            break;
                                        }
                                        return true;
                                    });
                            }
                            else
                            {
                                if (brush == null)
                                    return;

                                if (eventType == EventType.MouseDown)
                                {
                                    selectionTool.prefabList.Clear();

                                    // on MouseDown make list of selected prefabs
                                    settings.GetActiveTab().brushes.ForEach((b) => {
                                        if(b.selected)
                                        {
                                            if(b.settings.multibrushEnabled)
                                            {
                                                for(int i = 0; i < b.prefabSlots.Length; i++)
                                                {
                                                    if(b.prefabSlots[i].gameObject != null && b.settings.multibrushSlots[i].enabled)
                                                    {
                                                        selectionTool.prefabList.Add(b.prefabSlots[i].gameObject);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                GameObject prefab = b.GetFirstAssociatedPrefab();
                                                if(prefab != null)
                                                    selectionTool.prefabList.Add(prefab);
                                            }
                                        }
                                    });
                                }

                                currentHitObject = Raycast(HandleUtility.GUIPointToWorldRay (e.mousePosition), out currentRaycast, layersMask, settings.ignoreLayers.value);
                                if (!currentRaycast.isHit)
                                    return;

                                octree.IntersectSphere(currentRaycast.point, settings.selectBrushRadius, (go) =>
                                    {
                                        if(go == null)
                                            return true;

                                        GameObject prefabRoot = PrefabUtility.FindPrefabRoot(go);
                                        if(prefabRoot == null)
                                            return true;
                                        
                                        if(selectionTool.prefabList.Contains(PrefabUtility.GetPrefabParent(prefabRoot) as GameObject))
                                        {
                                            switch(settings.selectMode)
                                            {
                                            case SelectMode.Replace:
                                            case SelectMode.Add:
                                                selectionTool.selectedObjects.Add(prefabRoot);
                                                selectionChanged = true;
                                                break;
                                            case SelectMode.Substract:
                                                if(selectionTool.selectedObjects.Contains(prefabRoot))
                                                {
                                                    selectionTool.selectedObjects.Remove(prefabRoot);
                                                    selectionChanged = true;
                                                }
                                                break;
                                            }
                                        }
                                        return true;
                                    });
                            }

                            if(selectionChanged)
                                Selection.objects = selectionTool.selectedObjects.ToArray();

                            e.Use();
                        }
                        break;
                    }
                }
                break;
            case EventType.MouseUp:
                
                if (GUIUtility.hotControl != controlID)
                    return;
                
                GUIUtility.hotControl = 0;

                if (!IsPaintToolEnabled())
                    return;

                if(brush != null)
                    brush.EndStroke();
                
                if(currentTool == PaintTool.PrecisePlace && brush.settings.ppFixedScale == false && precisePlaceTool.placedObjectInfo != null)
                {
                    Vector2 placeSrceenPoint = HandleUtility.WorldToGUIPoint(precisePlaceTool.placedObjectInfo.raycastInfo.point);

                    if((e.mousePosition - placeSrceenPoint).magnitude < 10f)
                    {
                        GameObject.DestroyImmediate(precisePlaceTool.placedObjectInfo.gameObject);
                        precisePlaceTool.placedObjectInfo = null;
                    }
                }
                
                e.Use();
                break;
            case EventType.MouseMove:

				if(onPickObject)
				{
					currentHitObject = Raycast(HandleUtility.GUIPointToWorldRay (e.mousePosition), out currentRaycast, layersMask, settings.ignoreLayers.value);

					HandleUtility.Repaint();
					e.Use();
				}
				else                
                if (IsPaintToolEnabled())
				{
	                currentHitObject = Raycast(HandleUtility.GUIPointToWorldRay (e.mousePosition), out currentRaycast, layersMask, settings.ignoreLayers.value);
	                
					// for slope handles info
	                brushTool.firstNormal = currentRaycast.normal;

	                if(brush != null && !hasMultipleSelectedBrushes && brush.settings.gridEnabled && (currentTool == PaintTool.Brush || currentTool == PaintTool.PrecisePlace))
	                    UpdateGrid(brush);

	                HandleUtility.Repaint();
	                e.Use();
				}
                break;
            case EventType.KeyDown:
                
                if (IsPaintToolEnabled())
				{                
	                // F key - Frame camera on brush hit point
	                if (e.keyCode == KeyCode.F && e.modifiers == 0 && currentRaycast.isHit)
	                {
	                    SceneView.lastActiveSceneView.LookAt(currentRaycast.point);
	                    e.Use();
	                }
				}

                break;
            case EventType.Repaint:
				
				if (IsPaintToolEnabled() || onPickObject)
				{               
	                if(!currentRaycast.isHit)
	                    return;
					
	                DrawHandles(currentRaycast, brush, hasMultipleSelectedBrushes, GUIUtility.hotControl == controlID);
				}

                break;
            case EventType.Layout:
                
				if (IsPaintToolEnabled() || onPickObject)
				{                
                	HandleUtility.AddDefaultControl(controlID);
				}
                break;
            }
        }

        Ray WorldPointToRay(Vector3 worldSpacePoint)
        {
            if(Camera.current == null)
                return new Ray();
            return new Ray(Camera.current.transform.position, (worldSpacePoint - Camera.current.transform.position).normalized);
        }
		

#endregion // Scene UI

	} // class PrefabPainter
        

} // namespace nTools.PrefabPainter


