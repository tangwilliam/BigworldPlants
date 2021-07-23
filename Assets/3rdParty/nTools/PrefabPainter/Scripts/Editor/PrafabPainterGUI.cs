
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
    // class Strings
    //
    static public class Strings 
    {
        public static GUIContent    windowTitle                 = new GUIContent("Prefab Painter", "Prefab placement tool");


        public static GUIContent    brushName                   = new GUIContent("Brush Name", "Brush name");
        public static GUIContent    brushRadius                 = new GUIContent("Radius", "Brush radius in world units");
        public static GUIContent    brushSpacing                = new GUIContent("Spacing", "");

        public static GUIContent    brushPositionOffset         = new GUIContent("Surface Offset", "");

        public static GUIContent    brushOrientationTransform   = new GUIContent("Transform", "");
        public static GUIContent    brushOrientationMode        = new GUIContent("Orientation", "");
        public static GUIContent    brushFlipOrientation        = new GUIContent("Flip Orientation", "");
        public static GUIContent    brushRotation               = new GUIContent("Aux Rotation", "");
        public static GUIContent    brushRandomizeOrientationX  = new GUIContent("Randomize X %", "");
        public static GUIContent    brushRandomizeOrientationY  = new GUIContent("Randomize Y %", "");
        public static GUIContent    brushRandomizeOrientationZ  = new GUIContent("Randomize Z %", "");

        public static GUIContent    brushScaleTransformMode     = new GUIContent("Transform", "");
        public static GUIContent    brushScaleMode              = new GUIContent("Mode", "");
        public static GUIContent    brushScaleUniformMin        = new GUIContent("Min", "");
        public static GUIContent    brushScaleUniformMax        = new GUIContent("Max", "");
        public static GUIContent    brushScalePerAxisMin        = new GUIContent("Min", "");
        public static GUIContent    brushScalePerAxisMax        = new GUIContent("Max", "");

        public static GUIContent    brushPaintOnSelected        = new GUIContent("Paint On Selected Only", "");
        public static GUIContent    brushPaintOnLayers          = new GUIContent("Paint On Layers", "");
        public static GUIContent    brushIgnoreLayers           = new GUIContent("Ignore Layers", "");
        public static GUIContent    brushPlaceUnder             = new GUIContent("Place Under", "");
        public static GUIContent    brushCustomSceneObject      = new GUIContent("Custom Scene Object", "");
        public static GUIContent    brushGroupPrefabs           = new GUIContent("Group Prefabs", "");
        public static GUIContent    brushOverwritePrefabLayer   = new GUIContent("Overwrite Prefab Layer", "");
        public static GUIContent    brushPrefabPlaceLayer       = new GUIContent("Prefab Place Layer", "");


        public static GUIContent    settingsMaxBrushRadius      = new GUIContent("Max Brush Size", "");
        public static GUIContent    settingsMaxBrushSpacing     = new GUIContent("Max Brush Spacing", "");
        public static GUIContent    settingsPrecisePlaceSnapAngle = new GUIContent("Precise Place Snap Angle", "");
        public static GUIContent    settingsSurfaceCoords       = new GUIContent("Surface Coords", "");
        public static GUIContent    settingsHideSceneSettingsObject = new GUIContent("Hide Scene Settings Object", "");
        public static GUIContent    settingsGroupName           = new GUIContent("Group Name Prefix", "");

        public static string[]      colorTagNames               = Enum.GetNames (typeof (ColorTag));


        public static string        brushSettingsFoldout        = "Brush Settings";
        public static string        positionSettingsFoldout     = "Position";
        public static string        orientationSettingsFoldout  = "Orientation";
        public static string        scaleSettingsFoldout        = "Scale";
        public static string        commonSettingsFoldout       = "Common Settings";

        public static string        settingsLabel               = "Settings";
        public static string        helpLabel                   = "Help";


        public static string[]      selectModeNames             = Enum.GetNames (typeof (SelectMode));

        public static string        undoAddBrush                = "PP: Add Brush";
        public static string        undoRelinkPrefab            = "PP: Relink Brush Prefab";
        public static string        undoPaintPrefabs            = "PP: Paint Prefabs";
        public static string        undoMoveBrushes             = "PP: Move Brush(es)";
        public static string        undoDuplicateBrushes        = "PP: Duplicate Brush(es)";
        public static string        undoDeleteBrushes           = "PP: Delete Brush(es)";
        public static string        undoResetBrushes            = "PP: Reset Brush(es)";
        public static string        undoPasteBrushSettings      = "PP: Paste Brush Settings";
        public static string        undoAddTab                  = "PP: Add Tab";
        public static string        undoDeleteTab               = "PP: Delete Tab";

        public static string        dragNDropHere               = "Drag & Drop Prefab Here";
        public static string        missingPrefab               = "Missing";
        public static string        shiftDragRelink             = "Shift+Drag\nRelink";
        public static string        selectBrush                 = "Select Brush";
        public static string        multiSelBrush               = "Multiple Selected Brushes";
        public static string        newSettingsName             = "Untitled Settings";
        public static string        newTabName                  = "New Tab";


        public static string helpText =            
            "Shortcuts:\n" +
            "    Shift+drag\t- Relink prefab\n" +
            "    []\t\t- Change brush size\n" +
            "    ESC\t\t- Abort paint\n" +
            "    F\t\t- Frame camera on brush\n" +
            "    ; '\t\t- Change grid offet (in Grid mode)\n" +
            "    , .\t\t- Change grid size (in Grid mode)\n";

    }


    //
    // class Styles
    //
    static public class Styles
    {
		public static GUIStyle		logoFont;

        public static GUIStyle      iconLabelText;

        public static GUIStyle      tabLabelText;
        public static GUIStyle      tabButton;
        public static Color         tabTintColor;

        public static GUIStyle      multibrushIconText;

        public static GUIStyle      buttonLeft  = "buttonleft";
        public static GUIStyle      buttonMid   = "buttonmid";
        public static GUIStyle      buttonRight = "buttonright";

        public static GUIStyle      boldFoldout;
        public static GUIStyle      precisePlaceTextStyle;
        public static GUIStyle      handlesTextStyle;

        public static Color         colorBlue = new Color32 (62, 125, 231, 255);
        public static Color         backgroundColor;

        public static GUIStyle      leftGreyMiniLabel;

        public static Color         colorTagRed     = new Color32 (232, 19,  19, 255);
        public static Color         colorTagOrange  = new Color32 (251, 138,  0, 255);
        public static Color         colorTagYellow  = new Color32 (255, 223,  0, 255);
        public static Color         colorTagGreen   = new Color32 (61,  200, 53, 255);
        public static Color         colorTagBlue    = new Color32 (20,  130, 224, 255); 
        public static Color         colorTagViolet  = new Color32 (155, 86,  251, 255);

        public static Color         foldoutTintColor;

        public static GUIStyle      miniHorizontalScrollbar;
        public static GUIStyle      miniHorizontalScrollbarThumb;

        static Styles()
        {
			logoFont = new GUIStyle(EditorStyles.label);
			logoFont.alignment = TextAnchor.MiddleCenter;
			logoFont.fontSize = 20;

            backgroundColor = EditorGUIUtility.isProSkin 
                ? new Color32(41, 41, 41, 255)
                : new Color32(162, 162, 162, 255);


            iconLabelText = new GUIStyle(EditorStyles.miniLabel);
            iconLabelText.alignment = TextAnchor.LowerCenter;
            iconLabelText.clipping  = TextClipping.Clip;


            tabLabelText = new GUIStyle(EditorStyles.label);
            tabLabelText.alignment = TextAnchor.MiddleCenter;
            tabLabelText.clipping  = TextClipping.Clip;
            tabLabelText.fontStyle = FontStyle.Bold;

            multibrushIconText = new GUIStyle(EditorStyles.miniLabel);

            tabButton = new GUIStyle(EditorStyles.toolbarButton);
            tabButton.fixedHeight = 0;
            tabButton.fixedWidth = 0;

            tabTintColor = EditorGUIUtility.isProSkin 
                ? new Color (1f, 1f, 1f, 0.1f)
                : new Color (0f, 0f, 0f, 0.1f);



            if(buttonLeft == null || buttonMid == null || buttonRight == null)
            {
                buttonLeft  = new GUIStyle(EditorStyles.miniButtonLeft);
                buttonMid   = new GUIStyle(EditorStyles.miniButtonMid);
                buttonRight = new GUIStyle(EditorStyles.miniButtonRight);
            }


            boldFoldout = new GUIStyle(EditorStyles.foldout);
            boldFoldout.fontStyle = FontStyle.Bold;

            precisePlaceTextStyle = new GUIStyle(EditorStyles.largeLabel);
            precisePlaceTextStyle.normal.textColor = Color.red;
            precisePlaceTextStyle.fontStyle = FontStyle.Bold;
            precisePlaceTextStyle.fontSize = 20;

            handlesTextStyle = new GUIStyle(EditorStyles.largeLabel);
            handlesTextStyle.normal.textColor = Color.red;
            //handlesTextStyle.fontStyle = FontStyle.Bold;

            #if UNITY_5_5_OR_NEWER
            leftGreyMiniLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
            #else
            leftGreyMiniLabel = new GUIStyle(EditorStyles.miniLabel);
            #endif
            leftGreyMiniLabel.alignment = TextAnchor.MiddleLeft;


            foldoutTintColor = EditorGUIUtility.isProSkin 
                ? new Color (1f, 1f, 1f, 0.1f)
                : new Color (0f, 0f, 0f, 0.1f);





            miniHorizontalScrollbar = new GUIStyle(GUI.skin.horizontalScrollbar);
            miniHorizontalScrollbar.fixedWidth = 0f;
            miniHorizontalScrollbar.fixedHeight = 0f;
            miniHorizontalScrollbarThumb = new GUIStyle(GUI.skin.horizontalScrollbarThumb);
            miniHorizontalScrollbarThumb.fixedWidth = 0f;
            miniHorizontalScrollbarThumb.fixedHeight = 0f;
        }
    }




	//
	// class BrushPresetDatabase
	//
	public class BrushPresetDatabase
	{
		public string 		   presetsDirectory = "";
		public List<string>    presets = new List<string>();
		static string 		   kPresetFileExtension = ".xml";

		public BrushPresetDatabase(string presetsDirectory)
		{
			this.presetsDirectory = presetsDirectory;
			Refresh ();
		}

		public void Refresh()
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(presetsDirectory);
			
			presets.Clear();
			
			foreach(FileInfo fileInfo in directoryInfo.GetFiles())
			{
				if(String.Compare(fileInfo.Extension, kPresetFileExtension, true) == 0)
				{
					presets.Add(Path.GetFileNameWithoutExtension(fileInfo.FullName));
				}
			}
		}
		
		public void SavePreset(string name, BrushSettings settings)
		{
			string filePath = Path.Combine(presetsDirectory, name + kPresetFileExtension);
			
			string xml = XmlUtility.ToXmlString(settings);
			if(xml != null)
			{
				File.WriteAllText(filePath, xml);
				AssetDatabase.Refresh();
				Refresh();
			}
		}
		
		public BrushSettings LoadPreset(string name)
		{
			string text = File.ReadAllText (Path.Combine(presetsDirectory, name + kPresetFileExtension));
			return XmlUtility.FromXmlString<BrushSettings>(text);
		}
		
		public void DeletePreset(string name)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(presetsDirectory);
			
			foreach(FileInfo fileInfo in directoryInfo.GetFiles())
			{
				if(String.Compare(fileInfo.Extension, kPresetFileExtension, true) == 0)
				{
					if(Path.GetFileNameWithoutExtension(fileInfo.FullName) == name)
					{
						string filePath = fileInfo.FullName.Replace(Application.dataPath, "Assets");

						File.Delete(filePath);
						AssetDatabase.Refresh();
						Refresh();
					}
				}
			}
		}
		
		public void DeleteAll()
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(presetsDirectory);
			
			foreach(FileInfo fileInfo in directoryInfo.GetFiles())
			{
				if(String.Compare(fileInfo.Extension, kPresetFileExtension, true) == 0)
				{
					string filePath = fileInfo.FullName.Replace(Application.dataPath, "Assets");
					
					File.Delete(filePath);
					AssetDatabase.Refresh();
					Refresh();
				}
			}
		}
	}



	//
	// class PrefabPainter
	//
	// GUI implementation
	//  
	//
	public partial class PrefabPainter : EditorWindow
	{        
		
		//
		// class InternalDragAndDrop
		//
		static class InternalDragAndDrop
		{
			enum State
			{
				None,
				DragPrepare,
				DragReady,
				Dragging,
				DragPerform
			}
			
			static object          dragData = null;
			static Vector2         mouseDownPosition;
			static State           state = State.None;
			const float            kDragStartDistance = 7.0f;
			
			
			public static void OnBeginGUI()
            {
                Event e = Event.current;

                switch(state)
                {
                case State.None:
                    {
                        if (e.type == EventType.MouseDown && e.button == 0)
                        {
                            mouseDownPosition = e.mousePosition;
                            state = State.DragPrepare;
                        }
                    }
                    break;
                case State.DragPrepare:
                    {
                        if (e.type == EventType.MouseUp && e.button == 0)
                        {                        
                            state = State.None;
                        }
                    }
                    break;
                case State.DragReady:
                    {
                        if (e.type == EventType.MouseUp && e.button == 0)
                        {                        
                            state = State.None;
                        }
                    }
                    break;
                case State.Dragging:
                    {
                        if (e.type == EventType.MouseUp && e.button == 0)
                        {                        
                            state = State.DragPerform;
                            e.Use();
                        }

                        if (e.type == EventType.MouseDrag)
                        {
                            e.Use();
                        }                       
                    }
                    break;
                }
            }


            public static void OnEndGUI()
            {
                Event e = Event.current;

                switch(state)
                {
                case State.DragReady:
                    if (e.type == EventType.Repaint)
                    {
                        state = State.None;
                    }
                    break;
                case State.DragPrepare:                
                    if (e.type == EventType.MouseDrag &&
                        ((mouseDownPosition - e.mousePosition).magnitude > kDragStartDistance))
                    {                    
                        state = State.DragReady;
                    }
                    break;
                case State.DragPerform:
                    {
                        if (e.type == EventType.Repaint)
                        {
                            dragData = null;
                            state = State.None;
                        }
                    }
                    break;
                }
            }



            public static bool IsDragReady()
            {
                return state == State.DragReady;
            }

            public static void StartDrag(object data)
            {
                if (data == null || state != State.DragReady)
                    return;

                dragData = data;
                state = State.Dragging;
            }

            public static bool IsDragging()
            {
                return state == State.Dragging;
            }

            public static bool IsDragPerform()
            {
                return state == State.DragPerform;
            }

            public static object GetData()
            {
                return dragData;
            }

            public static Vector2 DragStartPosition()
            {
                return mouseDownPosition;
            }

        }


        //
        // class AssetPreviewCacheController
        //
        public static class AssetPreviewCacheController
        {
            const int kMinCacheSize = 50;
            const int kMaxCacheSize = 1000;
            static int currentCacheSize = 50;
            static Dictionary<string, int> requests = new Dictionary<string, int>();

            public static void AddCacheRequest(string id, int count)
            {
                if(count < 0)
                    return;

                requests[id] = count;

                int requestCache = Mathf.Clamp(requests.Sum(x => x.Value), kMinCacheSize, kMaxCacheSize) + 10;

                if(requestCache > currentCacheSize)
                {                    
                    AssetPreview.SetPreviewTextureCacheSize(requestCache);
                    currentCacheSize = requestCache;
                }
            }
        }


		/*public class PreviewRender2
		{
			PreviewRenderUtility previewRenderUtility;

			public Camera camera { get; }

			public bool isInitialized {
				get { return previewRenderUtility != null; }
			}

			public PreviewRender2()
			{
				try {
					previewRenderUtility = new PreviewRenderUtility (true);
					#if UNITY_2017_1_OR_NEWER
					camera = previewRenderUtility.camera;
					#else
					camera = previewRenderUtility.m_Camera;
					#endif

				} catch(Exception)
				{
					previewRenderUtility = null;
				}
			}

			public void Cleanup ()
			{
				if(previewRenderUtility != null)
					previewRenderUtility.Cleanup();
			}

			public void BeginPreview(Rect rect)
			{
				if(previewRenderUtility != null)
					previewRenderUtility.BeginPreview(rect, null);
			}

			public void EndAndDrawPreview(Rect rect)
			{
				if(previewRenderUtility != null)
					previewRenderUtility.EndAndDrawPreview(rect);
			}

			public void Render()
			{
				if(previewRenderUtility != null)
					#if UNITY_2017_1_OR_NEWER
					previewRenderUtility.Render();
					#else
					previewRenderUtility.m_Camera.Render();
					#endif
			}

			public void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int subMeshIndex)
			{
				Graphics.DrawMesh(mesh, matrix, material, 1, camera, subMeshIndex);
			}
		}*/


		public class PreviewRender
		{			
			public Camera camera;
			public RenderTexture renderTexture;
			public int previewLayer = 7;

			public PreviewRender()
			{
//				try {
//					BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic;
//					PropertyInfo propertyInfo = typeof(Camera).GetProperty ("PreviewCullingLayer", flags);
//					previewLayer = (int)propertyInfo.GetValue (null, new object[0]);
//				} catch(Exception)
//				{
//				}

				GameObject gameObject = EditorUtility.CreateGameObjectWithHideFlags("PreRenderCamera", HideFlags.HideAndDontSave, new Type[] { typeof(Camera) });
				camera = gameObject.GetComponent<Camera>();
				#if (UNITY_5_2 || UNITY_5_3 || UNITY_5_4_OR_NEWER) 
				camera.cameraType = CameraType.Preview;
				#endif
				camera.fieldOfView = 35f;
				camera.enabled = false;
				camera.clearFlags = CameraClearFlags.Depth;
				camera.farClipPlane = 10f;
				camera.nearClipPlane = 2f;
				camera.backgroundColor = new Color(0.192156866f, 0.192156866f, 0.192156866f, 1f);
				camera.renderingPath = RenderingPath.Forward;
				camera.useOcclusionCulling = false;
				camera.cullingMask = 1 << previewLayer;
			}

			public void Cleanup ()
			{
				if (camera)
				{
					UnityEngine.Object.DestroyImmediate(camera.gameObject, true);
				}
				if (renderTexture)
				{
					UnityEngine.Object.DestroyImmediate(renderTexture);
					renderTexture = null;
				}
			}

			public void BeginPreview(Rect rect)
			{
				#if (UNITY_5_4_OR_NEWER)
				int width = (int)(rect.width * EditorGUIUtility.pixelsPerPoint);
				int height = (int)(rect.height * EditorGUIUtility.pixelsPerPoint);
				#else	
				int width = (int)rect.width;
				int height = (int)rect.height;
				#endif


				if (!renderTexture || renderTexture.width != width || renderTexture.height != height)
				{
					if (renderTexture)
					{
						UnityEngine.Object.DestroyImmediate(renderTexture);
						renderTexture = null;
					}

					renderTexture = new RenderTexture(width, height, 16, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
					renderTexture.hideFlags = HideFlags.HideAndDontSave;
					camera.targetTexture = renderTexture;
				}			
			}

			public void EndAndDrawPreview(Rect rect)
			{
				if(camera && renderTexture)
					GUI.DrawTexture(rect, renderTexture, ScaleMode.ScaleAndCrop, false);
			}

			public void Render()
			{
				if(camera)
					camera.Render();
			}

			public void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int subMeshIndex)
			{
				if(camera)
					Graphics.DrawMesh(mesh, matrix, material, previewLayer, camera, subMeshIndex);
			}
		}

        const float                 kUIRepaintInterval = 0.5f;
        float                       lastUIRepaintTime = 0;

		BrushPresetDatabase			presetDatabase = null;

        Vector2                     windowScrollPos;
        bool                        onBeginTabRename = false;
        int                         multibrushItemDoubleClicked = -1;

		bool						onPickSlopeNormal = false;
		bool						onPickGridNormal = false;

        Dictionary<string, string>  brushShortNamesDictionary = new Dictionary<string, string>();
        Dictionary<string, string>  tabShortNamesDictionary = new Dictionary<string, string>();
        Dictionary<int, Vector2>    brushWindowsScrollDictionary = new Dictionary<int, Vector2>();
        Dictionary<int, float>      multibrushScrollDictionary = new Dictionary<int, float>();

		#pragma warning disable 0414  
		enum PreviewSide { Right, Top, Front };
		static string[]				s_PreviewSideNames = Enum.GetNames(typeof(PreviewSide));
		PreviewRender 				previewRender;
		PreviewSide					previewSide = PreviewSide.Front;
		GameObject					previewObject;
		Bounds						previewObjectBounds;
		Material					previewWireMaterial;
		int							previewSelectedSlot;
		Vector2                 	previewWindowPosition = new Vector2(-0.5f, -0.5f);
		float                   	previewWindowScale = 1.8f;
		Mesh						previewGridMesh;
		Mesh						previewPivotMesh;
		Mesh						previewQuadMesh;
		Vector3						dragOffset;
 
        // Textures
        Texture2D                   closeIconTexture = null;
        Texture2D                   tagTexture = null;
        Texture2D                   resetIconTexture = null;
        Texture2D[]                 toolbarTextures = null;



        static int                  s_TabsWindowHash = "PPGUI.TabsWindow".GetHashCode();
        static int                  s_BrushWindowHash = "PPGUI.BrushWindow".GetHashCode();
        static int                  s_BrushWindowResizeBarHash = "PPGUI.BrushWindowResize".GetHashCode();
        static int                  s_RaitingBarHash = "PPGUI.RatingBar".GetHashCode();
        static int                  s_MultibrushHash = "PPGUI.Multibrush".GetHashCode();
        static int                  s_BrushWindowObjectPickerHash = "PPGUI.BrushWindow.ObjectPicker".GetHashCode();
        static int                  s_MultibrushObjectPickerHash = "PPGUI.Multibrush.ObjectPicker".GetHashCode();
		static int                  s_PivotEditorHash = "PPGUI.PivotEditor".GetHashCode();
		static int                  s_PivotEditorPivotHash = "PPGUI.PivotEditorPivot".GetHashCode();
        static int                  s_FlatButtonHash = "PPGUI.FlatButton".GetHashCode();
        static string               s_tabTextFieldName = "TabNameTextField";
        static float                s_HorizontalMiniScrollBar_GrabPosition;
        static int                  s_HorizontalMiniScrollBarHash = "HorizontalMiniScrollBar".GetHashCode();

        static GUIContent[]         s_XYLabels = { new GUIContent("X"), new GUIContent("Y") };
        static GUIContent[]         s_XYZLabels = { new GUIContent("X"), new GUIContent("Y"), new GUIContent("Z") };
//        static GUIContent[]         s_MinMaxLabels = { new GUIContent(""), new GUIContent("") };
        static float[]              s_Vector2Floats = new float[2];
        static float[]              s_Vector3Floats = new float[3];
		#pragma warning restore 0414 


        // UI Settings
        const int                   kBrushWindowResizeBarHeight = 8;

        const int                   kBrushIconWidth  = 60;
        const int                   kBrushIconHeight = 72;

        const int                   kTabWidth = 70;
        const int                   kTabHeight = 20;

        const int                   kMultibrushIconSize = 70;
        const int                   kMultibrushIconBorderSize = 2;
        const float                 kMultibrushRaitingBarHeightPercent = 0.1f;


        bool                        brushSettingsFoldout       = true;
        bool                        positionSettingsFoldout    = true;
        bool                        orientationSettingsFoldout = true;
        bool                        scaleSettingsFoldout       = true;
        bool                        commonSettingsFoldout      = true;
        bool                        multibrushSettingsFoldout  = true;
        bool                        multibrushSlotSettingsFoldout  = false;
		bool                        pivotEditorFoldout         = false;
        bool                        gridFoldout                = true;
        bool                        slopeFilterFoldout         = true;
        bool                        precisePlaceFoldout        = true;
        bool                        precisePlaceSnapFoldout    = true;
        bool                        eraseSettingsFoldout       = true;
        bool                        selectSettingsFoldout      = true;
        bool                        toolSettingsFoldout        = true;
        bool                        helpFoldout                = true;
        float                       brushWindowHeight          = 225.0f;




        Texture2D LoadGUITexture(string filename)
        {
			Texture2D texture = AssetDatabase.LoadAssetAtPath(Path.Combine(GetGUIDirectory(), filename), typeof(Texture2D)) as Texture2D;
            if(texture != null)
                return texture;
            return Texture2D.whiteTexture;
        }

        void OnInitGUI()
        {
			// Init pivot editor stuff
			{
				previewRender = new PreviewRender ();

				Vector3[] vertices = new Vector3[]
				{
					new Vector3(0,0,0), new Vector3(1,0,0),
					new Vector3(0,0,0), new Vector3(0,1,0),
					new Vector3(0,0,0), new Vector3(0,0,1),

					new Vector3(0,0,0),	new Vector3(0,.25f,0), new Vector3(.25f,0,0), new Vector3(.25f,.25f,0),
					new Vector3(0,0,0),	new Vector3(0,.25f,0), new Vector3(0,0,.25f), new Vector3(0,.25f,.25f),
					new Vector3(0,0,0),	new Vector3(0,0,.25f), new Vector3(.25f,0,0), new Vector3(.25f,0,.25f),
				};

				Color r = Color.red;
				Color g = Color.green;
				Color b = Color.blue;

				Color[] colors = new Color[]
				{
					r, r, g, g, b, b,
					b, b, b, b,
					r, r, r, r,
					g, g, g, g
				};

				int[] indices1 = new int[]
				{
					0, 1, 2, 3, 4, 5
				};

				int[] indices2 = new int[]
				{
					6, 7, 8, 7, 9, 8,
					10, 11, 12, 11, 13, 12,
					14, 15, 16, 15, 17, 16
				};

				int[] indices3 = new int[]
				{
					6,7, 6,8, 7,9, 8,9,
					10,11, 10,12, 11,13, 12,13,
					14,15, 14,16, 15,17, 16,17
				};

				previewPivotMesh = new Mesh();
				previewPivotMesh.hideFlags = HideFlags.HideAndDontSave;
				previewPivotMesh.subMeshCount = 3;
				previewPivotMesh.vertices = vertices;
				previewPivotMesh.colors = colors;
				previewPivotMesh.SetIndices(indices1, MeshTopology.Lines, 0);
				previewPivotMesh.SetIndices(indices2, MeshTopology.Triangles, 1);
				previewPivotMesh.SetIndices(indices3, MeshTopology.Lines, 2);


				previewGridMesh = new Mesh();
				previewGridMesh.hideFlags = HideFlags.HideAndDontSave;

				Vector3[] gridVertices = new Vector3[41 * 2 * 2];
				int[] gridIndices = new int[41 * 2 * 2];

				for(int i = 0; i < gridVertices.Length; i+=4)
				{
					float pos = (((float)i / (gridVertices.Length-4)) - 0.5f) * 2f;

					gridVertices[i]   = new Vector3(pos, -1, 0);
					gridVertices[i+1] = new Vector3(pos, 1, 0);

					gridVertices[i+2] = new Vector3(-1, pos, 0);
					gridVertices[i+3] = new Vector3(1, pos, 0);
				}

				for(int i = 0; i < gridIndices.Length; i++)
				{
					gridIndices[i] = i;
				}

				previewGridMesh.vertices = gridVertices;
				previewGridMesh.SetIndices(gridIndices, MeshTopology.Lines, 0);


				previewQuadMesh = new Mesh();
				previewQuadMesh.hideFlags = HideFlags.HideAndDontSave;

				Vector3[] quadVertices = new Vector3[]
				{
					new Vector3(-1,-1,0), new Vector3(-1,1,0), new Vector3(1,1,0), new Vector3(1,-1,0)
				};

				int[] quadIndices = new int[]
				{
					0, 1, 3, 3, 1, 2
				};

				previewQuadMesh.subMeshCount = 1;
				previewQuadMesh.vertices = quadVertices;
				previewQuadMesh.SetIndices(quadIndices, MeshTopology.Triangles, 0);
			}

			// Load presets
			presetDatabase = new BrushPresetDatabase (GetPresetsDirectory ());


            // Load Textures
            if (EditorGUIUtility.isProSkin)
            {
                Utility.SetWindowTitle(this, new GUIContent(Strings.windowTitle.text, LoadGUITexture("pp_icon_w.png"), ""));
                closeIconTexture = LoadGUITexture("close_icon.png");
                tagTexture = LoadGUITexture("tag.png");
                resetIconTexture = LoadGUITexture("reset_icon_w.png");

                toolbarTextures = new Texture2D[] {
                    LoadGUITexture("paint_icon_w.png"),
                    LoadGUITexture("precise_icon_w.png"),
                    LoadGUITexture("erase_icon_w.png"),
                    LoadGUITexture("select_icon_w.png"),
                    LoadGUITexture("settings_icon_w.png")
                };
            }
            else
            {
                Utility.SetWindowTitle(this, new GUIContent(Strings.windowTitle.text, LoadGUITexture("pp_icon.png"), ""));

                closeIconTexture = LoadGUITexture("close_icon.png");
                tagTexture = LoadGUITexture("tag.png");
                resetIconTexture = LoadGUITexture("reset_icon.png");

                toolbarTextures = new Texture2D[] {
                    LoadGUITexture("paint_icon.png"),
                    LoadGUITexture("precise_icon.png"),
                    LoadGUITexture("erase_icon.png"),
                    LoadGUITexture("select_icon.png"),
                    LoadGUITexture("settings_icon.png")
                };
            }


            brushSettingsFoldout            = EditorPrefs.GetBool("nTools.PrefabPainter.brushSettingsFoldout", brushSettingsFoldout);
            positionSettingsFoldout         = EditorPrefs.GetBool("nTools.PrefabPainter.positionSettingsFoldout", positionSettingsFoldout);
            orientationSettingsFoldout      = EditorPrefs.GetBool("nTools.PrefabPainter.orientationSettingsFoldout", orientationSettingsFoldout);
            scaleSettingsFoldout            = EditorPrefs.GetBool("nTools.PrefabPainter.scaleSettingsFoldout", scaleSettingsFoldout);
            commonSettingsFoldout           = EditorPrefs.GetBool("nTools.PrefabPainter.commonSettingsFoldout", commonSettingsFoldout);
            multibrushSettingsFoldout       = EditorPrefs.GetBool("nTools.PrefabPainter.multibrushSettingsFoldout", multibrushSettingsFoldout);
			multibrushSlotSettingsFoldout   = EditorPrefs.GetBool("nTools.PrefabPainter.multibrushSlotSettingsFoldout", multibrushSlotSettingsFoldout);
			pivotEditorFoldout   			= EditorPrefs.GetBool("nTools.PrefabPainter.pivotEditorFoldout", pivotEditorFoldout);
            gridFoldout                     = EditorPrefs.GetBool("nTools.PrefabPainter.gridFoldout", gridFoldout);
            slopeFilterFoldout              = EditorPrefs.GetBool("nTools.PrefabPainter.slopeFilterFoldout", slopeFilterFoldout);
            precisePlaceFoldout             = EditorPrefs.GetBool("nTools.PrefabPainter.precisePlaceFoldout", precisePlaceFoldout);
            precisePlaceSnapFoldout         = EditorPrefs.GetBool("nTools.PrefabPainter.precisePlaceSnapFoldout", precisePlaceSnapFoldout);
            eraseSettingsFoldout            = EditorPrefs.GetBool("nTools.PrefabPainter.eraseSettingsFoldout", eraseSettingsFoldout);
            selectSettingsFoldout           = EditorPrefs.GetBool("nTools.PrefabPainter.selectSettingsFoldout", selectSettingsFoldout);
            toolSettingsFoldout             = EditorPrefs.GetBool("nTools.PrefabPainter.toolSettingsFoldout", toolSettingsFoldout);
            helpFoldout                     = EditorPrefs.GetBool("nTools.PrefabPainter.helpFoldout", helpFoldout);
            brushWindowHeight               = EditorPrefs.GetFloat("nTools.PrefabPainter.brushWindowHeight", brushWindowHeight);
        }

        void OnCleanupGUI()
        {
			previewRender.Cleanup ();
			previewRender = null;

            EditorPrefs.SetBool("nTools.PrefabPainter.brushSettingsFoldout", brushSettingsFoldout);
            EditorPrefs.SetBool("nTools.PrefabPainter.positionSettingsFoldout", positionSettingsFoldout);
            EditorPrefs.SetBool("nTools.PrefabPainter.orientationSettingsFoldout", orientationSettingsFoldout);
            EditorPrefs.SetBool("nTools.PrefabPainter.scaleSettingsFoldout", scaleSettingsFoldout);
            EditorPrefs.SetBool("nTools.PrefabPainter.commonSettingsFoldout", commonSettingsFoldout);
            EditorPrefs.SetBool("nTools.PrefabPainter.multibrushSettingsFoldout", multibrushSettingsFoldout);
            EditorPrefs.SetBool("nTools.PrefabPainter.multibrushSlotSettingsFoldout", multibrushSlotSettingsFoldout);
			EditorPrefs.SetBool("nTools.PrefabPainter.pivotEditorFoldout", pivotEditorFoldout);
            EditorPrefs.SetBool("nTools.PrefabPainter.gridFoldout", gridFoldout);
            EditorPrefs.SetBool("nTools.PrefabPainter.slopeFilterFoldout", slopeFilterFoldout);
            EditorPrefs.SetBool("nTools.PrefabPainter.precisePlaceFoldout", precisePlaceFoldout);
            EditorPrefs.SetBool("nTools.PrefabPainter.precisePlaceSnapFoldout", precisePlaceSnapFoldout);
            EditorPrefs.SetBool("nTools.PrefabPainter.eraseSettingsFoldout", eraseSettingsFoldout);
            EditorPrefs.SetBool("nTools.PrefabPainter.selectSettingsFoldout", selectSettingsFoldout);
            EditorPrefs.SetBool("nTools.PrefabPainter.toolSettingsFoldout", toolSettingsFoldout);
            EditorPrefs.SetBool("nTools.PrefabPainter.helpFoldout", helpFoldout);
            EditorPrefs.SetFloat("nTools.PrefabPainter.brushWindowHeight", brushWindowHeight);
        }



#region Context Menus

        GenericMenu PresetMenu()
        {
            GenericMenu menu = new GenericMenu();

            Tab activeTab = settings.GetActiveTab();
            bool hasSelectedBrushes = activeTab.HasSelectedBrushes();
            bool hasMultipleSelectedBrushes = activeTab.HasMultipleSelectedBrushes();
            Brush brush = activeTab.GetFirstSelectedBrush();

            if(!hasSelectedBrushes)
                return menu;




            // Reveal in Project
            if(!hasMultipleSelectedBrushes && brush != null && !brush.settings.multibrushEnabled)
            {
                GameObject prefab = brush.GetFirstAssociatedPrefab();

                if(prefab != null)
                {
                    menu.AddItem(new GUIContent("Reveal in Project"), false, ContextMenuCallback, new Action(() => EditorGUIUtility.PingObject(prefab)));
                    menu.AddSeparator ("");
                }
            }



            // Copy/Paste/Delete/Duplicate
            menu.AddItem(new GUIContent("Cut %x"), false, ContextMenuCallback, new Action(() => { settings.ClipboardCutBrushes(); }));
            menu.AddItem(new GUIContent("Copy %c"), false, ContextMenuCallback, new Action(() => { settings.ClipboardCopyBrushes(); }));
            if(settings.ClipboardIsCanPasteBrushes())
                menu.AddItem(new GUIContent("Paste %v"), false, ContextMenuCallback, new Action(() => { settings.ClipboardPasteBrushes(); }));
            else
                menu.AddDisabledItem(new GUIContent("Paste %v"));
            menu.AddItem(new GUIContent("Duplicate %d"), false, ContextMenuCallback, new Action(() => activeTab.DuplicateSelectedBrushes()));
        #if UNITY_EDITOR_OSX
            menu.AddItem(new GUIContent("Delete %\b"), false, ContextMenuCallback, new Action(() => activeTab.DeleteSelectedBrushes()));
        #else
            menu.AddItem(new GUIContent("Delete #DEL"), false, ContextMenuCallback, new Action(() => activeTab.DeleteSelectedBrushes()));
        #endif




            // Selection
            menu.AddSeparator ("");
            menu.AddItem(new GUIContent("Select All %a"), false, ContextMenuCallback, new Action(() => { settings.GetActiveTab().SelectAllBrushes(); }));
            menu.AddItem(new GUIContent("Invert Selection"), false, ContextMenuCallback, new Action(() => { settings.GetActiveTab().InvertSelection(); }));



            // Settings
            menu.AddSeparator ("");

            if(!hasMultipleSelectedBrushes)
                menu.AddItem(new GUIContent("Copy Settings"), false, ContextMenuCallback, new Action(() => settings.ClipboardCopySettings()));
            if(settings.ClipboardIsCanPasteSettings())
                menu.AddItem(new GUIContent("Paste Settings"), false, ContextMenuCallback, new Action(() => settings.ClipboardPasteSettings()));
            else
                menu.AddDisabledItem(new GUIContent("Paste Settings"));



            // Apply Saved Presets

            presetDatabase.Refresh();

			for (int i = 0; i < presetDatabase.presets.Count; i++)
            {
				string name = presetDatabase.presets[i];

                menu.AddItem(new GUIContent("Apply Settings/" + name), false, ContextMenuCallback, new Action(() => {
                    Undo.RegisterCompleteObjectUndo(settings, "PP: Apply Preset Settings");
                    activeTab.brushes.ForEach((b) =>    {                        
                        if(b.selected)
							b.settings.CopyFrom(presetDatabase.LoadPreset(name));
					});
                }));
            }


            // Save/Delete Presets
            menu.AddSeparator ("Apply Settings/");
            if (!hasMultipleSelectedBrushes)
				menu.AddItem(new GUIContent("Apply Settings/Save Settings..."), false, ContextMenuCallback, new Action(() => SaveSettingsDialog.ShowDialog(presetDatabase, settings)));
            else 
                menu.AddDisabledItem(new GUIContent("Apply Settings/Save Settings..."));            
			menu.AddItem(new GUIContent("Apply Settings/Delete Settings..."), false, ContextMenuCallback, new Action(() => DeleteSettingsDialog.ShowDialog(presetDatabase)));
			
			menu.AddItem(new GUIContent("Reset Settings"), false, ContextMenuCallback, new Action(() => activeTab.ResetSelectedBrushes()));




            // Extensions

            bool isAllMutibrush = true;
            //bool isAllAdvancedSettings = true;
			//bool isAllPivotEditor = true;
            bool isAllSlopeFilter = true;
            bool isAllGrid = true;

            activeTab.brushes.ForEach((b) => {
                if (b.selected) {
                    if (!b.settings.multibrushEnabled) isAllMutibrush = false;
                    //if (!b.settings.advancedSettingsEnabled) isAllAdvancedSettings = false;
					//if (!b.settings.pivotEditorEnabled) isAllPivotEditor = false;
                    if (!b.settings.slopeEnabled) isAllSlopeFilter = false;
                    if (!b.settings.gridEnabled) isAllGrid = false;
                }
            });

            menu.AddSeparator ("");
            menu.AddItem(new GUIContent("Multibrush"), isAllMutibrush, ContextMenuCallback, new Action(() => {
                Undo.RegisterCompleteObjectUndo(settings, "PP: Change Brush Settings");
                activeTab.brushes.ForEach((b) => { if (b.selected) b.settings.multibrushEnabled = !isAllMutibrush; });                
                    
            }));
//            menu.AddItem(new GUIContent("Advanced Settings"), isAllAdvancedSettings, ContextMenuCallback, new Action(() => {
//                Undo.RegisterCompleteObjectUndo(settings, "PP: Change Brush Settings");
//                activeTab.brushes.ForEach((b) => { if (b.selected) b.settings.advancedSettingsEnabled = !isAllAdvancedSettings; });                
//
//            }));
//			menu.AddItem(new GUIContent("Pivot Editor"), isAllPivotEditor, ContextMenuCallback, new Action(() => {
//				Undo.RegisterCompleteObjectUndo(settings, "PP: Change Brush Settings");
//				activeTab.brushes.ForEach((b) => { if (b.selected) b.settings.pivotEditorEnabled = !isAllPivotEditor; });                
//
//			}));
            menu.AddItem(new GUIContent("Slope Filter"), isAllSlopeFilter, ContextMenuCallback, new Action(() => {
                Undo.RegisterCompleteObjectUndo(settings, "PP: Change Brush Settings");
                activeTab.brushes.ForEach((b) => { if (b.selected) b.settings.slopeEnabled = !isAllSlopeFilter; });                

            }));
            menu.AddItem(new GUIContent("Grid"), isAllGrid, ContextMenuCallback, new Action(() => {
                Undo.RegisterCompleteObjectUndo(settings, "PP: Change Brush Settings");
                activeTab.brushes.ForEach((b) => { if (b.selected) b.settings.gridEnabled = !isAllGrid; });                

            }));



            // Tags
            menu.AddSeparator ("");
            menu.AddItem(new GUIContent("Tag/None"),   !hasMultipleSelectedBrushes && brush.colorTag == ColorTag.None,   ContextMenuCallback, new Action(() => activeTab.SetSelectedBrushesTag(ColorTag.None)));
            menu.AddItem(new GUIContent("Tag/Red"),    !hasMultipleSelectedBrushes && brush.colorTag == ColorTag.Red,    ContextMenuCallback, new Action(() => activeTab.SetSelectedBrushesTag(ColorTag.Red)));
            menu.AddItem(new GUIContent("Tag/Orange"), !hasMultipleSelectedBrushes && brush.colorTag == ColorTag.Orange, ContextMenuCallback, new Action(() => activeTab.SetSelectedBrushesTag(ColorTag.Orange)));
            menu.AddItem(new GUIContent("Tag/Yellow"), !hasMultipleSelectedBrushes && brush.colorTag == ColorTag.Yellow, ContextMenuCallback, new Action(() => activeTab.SetSelectedBrushesTag(ColorTag.Yellow)));
            menu.AddItem(new GUIContent("Tag/Green"),  !hasMultipleSelectedBrushes && brush.colorTag == ColorTag.Green,  ContextMenuCallback, new Action(() => activeTab.SetSelectedBrushesTag(ColorTag.Green)));
            menu.AddItem(new GUIContent("Tag/Blue"),   !hasMultipleSelectedBrushes && brush.colorTag == ColorTag.Blue,   ContextMenuCallback, new Action(() => activeTab.SetSelectedBrushesTag(ColorTag.Blue)));
            menu.AddItem(new GUIContent("Tag/Violet"), !hasMultipleSelectedBrushes && brush.colorTag == ColorTag.Violet, ContextMenuCallback, new Action(() => activeTab.SetSelectedBrushesTag(ColorTag.Violet)));


            return menu;
        }


        GenericMenu BrushWindowMenu()
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Add Prefab"), false, ContextMenuCallback, new Action(() => { this.SendEvent(EditorGUIUtility.CommandEvent("BrushWindowShowObjectSelector")); }));

            menu.AddSeparator ("");

            if(settings.ClipboardIsCanPasteBrushes())
                menu.AddItem(new GUIContent("Paste %v"), false, ContextMenuCallback, new Action(() => { settings.ClipboardPasteBrushes(); }));
            else
                menu.AddDisabledItem(new GUIContent("Paste %v"));

            menu.AddSeparator ("");

            if(settings.GetActiveTab().GetBrushCount() > 0)
            {
                menu.AddItem(new GUIContent("Select All %a"), false, ContextMenuCallback, new Action(() => { settings.GetActiveTab().SelectAllBrushes(); }));
                menu.AddItem(new GUIContent("Invert Selection"), false, ContextMenuCallback, new Action(() => { settings.GetActiveTab().InvertSelection(); }));
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Select All %a"));
                menu.AddDisabledItem(new GUIContent("Invert Selection"));
            }

			menu.AddSeparator ("");

			menu.AddItem(new GUIContent("Arrange By Name"), false, ContextMenuCallback, new Action(() => { settings.GetActiveTab().ArrangeBrushesByName(); }));

            return menu;
        }


        GenericMenu TabMenu()
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Add Tab"), false, ContextMenuCallback, new Action(() => { settings.SetActiveTabIndex(settings.AddNewTab(Strings.newTabName, settings.GetActiveTabIndex())); }));

            menu.AddSeparator ("");
            menu.AddItem(new GUIContent("Rename"), false, ContextMenuCallback, new Action(() => { onBeginTabRename = true; }));
            menu.AddItem(new GUIContent("Duplicate"), false, ContextMenuCallback, new Action(() => { settings.DuplicateTab(settings.GetActiveTabIndex()); }));
            if(settings.GetTabCount() > 1)
                menu.AddItem(new GUIContent("Delete"), false, ContextMenuCallback, new Action(() => { settings.DeleteTab(settings.GetActiveTabIndex()); }));
            else
                menu.AddDisabledItem(new GUIContent("Delete"));    

            return menu;
        }


        GenericMenu MultibrushPrefabMenu(Brush brush, int prefabSlot)
        {
            GenericMenu menu = new GenericMenu();

            if (brush.settings.multibrushSlots[prefabSlot].enabled)
                menu.AddItem(new GUIContent("Disable"), false, ContextMenuCallback, new Action(() => { Undo.RegisterCompleteObjectUndo(settings, "PP: Disable Slot"); brush.settings.multibrushSlots[prefabSlot].enabled = false; }));
            else
                menu.AddItem(new GUIContent("Enable"), false, ContextMenuCallback, new Action(() => { Undo.RegisterCompleteObjectUndo(settings, "PP: Enable Slot"); brush.settings.multibrushSlots[prefabSlot].enabled = true; }));

            if(brush.prefabSlots[prefabSlot].gameObject != null)
                menu.AddItem(new GUIContent("Clear"), false, ContextMenuCallback, new Action(() => { brush.ClearPrefab(prefabSlot); }));
            else 
                menu.AddDisabledItem(new GUIContent("Clear"));


            menu.AddSeparator ("");
            if(brush.prefabSlots[prefabSlot].gameObject != null)
                menu.AddItem(new GUIContent("Reveal in Project"), false, ContextMenuCallback, new Action(() => EditorGUIUtility.PingObject(brush.prefabSlots[prefabSlot].gameObject)));
            else 
                menu.AddDisabledItem(new GUIContent("Reveal in Project"));
            
            menu.AddSeparator ("");
            int rating = Mathf.RoundToInt(brush.settings.multibrushSlots[prefabSlot].raiting * 100.0f);
            menu.AddDisabledItem(new GUIContent("Rating " + rating + "%"));
            return menu;
        }

        void ContextMenuCallback(object obj)
        {
            if (obj != null && obj is Action)
                (obj as Action).Invoke();
        }

#endregion // Context Menus



#region GUI
        void HandleGridShortcuts()
        {
        }

        void HandleKeyboardEvents()
        {
            Event e = Event.current;
            Tab currentTab = settings.GetActiveTab();
            Brush selectedBrush = currentTab.GetFirstSelectedBrush ();
            BrushSettings brushSettings = selectedBrush != null ? selectedBrush.settings : null;


            switch(e.type)
            {
            case EventType.KeyDown:
                switch(currentTool)
                {
                case PaintTool.Brush:
                    if(e.type == EventType.KeyDown && selectedBrush != null && !currentTab.HasMultipleSelectedBrushes())
                    {
                        switch(e.keyCode) {
                        case KeyCode.LeftBracket:
                            brushSettings.brushRadius = Mathf.Max (0.05f, brushSettings.brushRadius - brushSettings.brushRadius * 0.1f);
                            HandleUtility.Repaint ();
                            e.Use();
                            break;
                        case KeyCode.RightBracket:
                            brushSettings.brushRadius = Mathf.Min (brushSettings.brushRadius + brushSettings.brushRadius * 0.1f, settings.maxBrushRadius);
                            HandleUtility.Repaint ();
                            e.Use();
                            break;
                        case KeyCode.Comma:
                            if(brushSettings.gridEnabled)
                            {
                                brushSettings.gridStep -= Vector2.one;
                                UpdateGrid(selectedBrush);
                                HandleUtility.Repaint ();
                                e.Use();
                            }
                            break;
                        case KeyCode.Period:
                            if(brushSettings.gridEnabled)
                            {
                                brushSettings.gridStep += Vector2.one;
                                UpdateGrid(selectedBrush);
                                HandleUtility.Repaint ();
                                e.Use();
                            }
                            break;
                        case KeyCode.Semicolon:
                            if(brushSettings.gridEnabled)
                            {
                                brushSettings.gridOrigin -= new Vector2(0.5f, 0.5f);
                                UpdateGrid(selectedBrush);
                                HandleUtility.Repaint ();
                                e.Use();
                            }
                            break;
                        case KeyCode.Quote:
                            if(brushSettings.gridEnabled)
                            {
                                brushSettings.gridOrigin += new Vector2(0.5f, 0.5f);
                                UpdateGrid(selectedBrush);
                                HandleUtility.Repaint ();
                                e.Use();
                            }
                            break;
                        }
                    }
                    break;
                case PaintTool.PrecisePlace:
                    if(e.type == EventType.KeyDown && selectedBrush != null && !currentTab.HasMultipleSelectedBrushes())
                    {
                        switch(e.keyCode) {
                        case KeyCode.Comma:
                            if(brushSettings.gridEnabled)
                            {
                                brushSettings.gridStep -= Vector2.one;
                                UpdateGrid(selectedBrush);
                                HandleUtility.Repaint ();
                                e.Use();
                            }
                            break;
                        case KeyCode.Period:
                            if(brushSettings.gridEnabled)
                            {
                                brushSettings.gridStep += Vector2.one;
                                UpdateGrid(selectedBrush);
                                HandleUtility.Repaint ();
                                e.Use();
                            }
                            break;
                        case KeyCode.Semicolon:
                            if(brushSettings.gridEnabled)
                            {
                                brushSettings.gridOrigin -= new Vector2(0.5f, 0.5f);
                                UpdateGrid(selectedBrush);
                                HandleUtility.Repaint ();
                                e.Use();
                            }
                            break;
                        case KeyCode.Quote:
                            if(brushSettings.gridEnabled)
                            {
                                brushSettings.gridOrigin += new Vector2(0.5f, 0.5f);
                                UpdateGrid(selectedBrush);
                                HandleUtility.Repaint ();
                                e.Use();
                            }
                            break;
                        }
                    }
                    break;
                case PaintTool.Erase:
                    switch(e.keyCode) {
                    case KeyCode.LeftBracket:
                        settings.eraseBrushRadius = Mathf.Max (0.05f, settings.eraseBrushRadius - settings.eraseBrushRadius * 0.1f);
                        HandleUtility.Repaint ();
                        e.Use();
                        break;
                    case KeyCode.RightBracket:                    
                        settings.eraseBrushRadius = Mathf.Min (settings.eraseBrushRadius + settings.eraseBrushRadius * 0.1f, settings.maxBrushRadius);
                        HandleUtility.Repaint ();
                        e.Use();
                        break;
                    }
                    break;
                case PaintTool.Select:
                    switch(e.keyCode) {
                    case KeyCode.LeftBracket:
                        settings.selectBrushRadius = Mathf.Max (0.05f, settings.selectBrushRadius - settings.selectBrushRadius * 0.1f);
                        HandleUtility.Repaint ();
                        e.Use();
                        break;
                    case KeyCode.RightBracket:                    
                        settings.selectBrushRadius = Mathf.Min (settings.selectBrushRadius + settings.selectBrushRadius * 0.1f, settings.maxBrushRadius);
                        HandleUtility.Repaint ();
                        e.Use();
                        break;
                    }
                    break;
                }

                // ESC
                if(e.keyCode == KeyCode.Escape && e.modifiers == 0)
                {
					if(onPickObject)
					{
						onPickObject = false;
					}
					else
					if(currentTool != PaintTool.None)
					{
                        UnityEditor.Tools.current = Tool.Move;
						currentTool = PaintTool.None;
					}
                    
                    Repaint();
                    e.Use ();
                }
                break;
            }
        }



        static string DelayedTextField(GUIContent label, string text)
        {
            #if (UNITY_5_4_OR_NEWER)
            return EditorGUILayout.DelayedTextField (label, text);
            #else
			return EditorGUILayout.TextField(label, text);
            #endif
        }

		static string DelayedTextField(Rect position, string text)
		{
			#if (UNITY_5_4_OR_NEWER)
            return EditorGUI.DelayedTextField (position, text);
			#else
			return EditorGUI.TextField(position, text);
			#endif
		}
		

        string GetShortNameForBrush(string name)
        {
            if(name == null || name.Length == 0)
                return "";

            string shortString;

            if(brushShortNamesDictionary.TryGetValue(name, out shortString))
                return shortString;

            return brushShortNamesDictionary[name] = Utility.TruncateString(name, Styles.iconLabelText, kBrushIconWidth);
        }



        string GetShortNameForTab(string name)
        {
            if(name == null || name.Length == 0)
                return "";

            string shortString;

            if(tabShortNamesDictionary.TryGetValue(name, out shortString))
                return shortString;

            return tabShortNamesDictionary[name] = Utility.TruncateString(name, Styles.tabLabelText, kTabWidth);
        }


        Vector2 GetBrushWindowScrollPosition(Tab tab)
        {
            Vector2 scrollPosition;

            if(brushWindowsScrollDictionary.TryGetValue(tab.id, out scrollPosition))
                return scrollPosition;

            return Vector2.zero;
        }

        void SetBrushWindowScrollPosition(Tab tab, Vector2 scrollPosition)
        {
            brushWindowsScrollDictionary[tab.id] = scrollPosition;
        }


        float GetMultibrushScrollPosition(Brush brush)
        {
            float scrollPosition;

            if(multibrushScrollDictionary.TryGetValue(brush.id, out scrollPosition))
                return scrollPosition;

            return 0f;
        }

        void SetMultibrushScrollPosition(Brush brush, float scrollPosition)
        {
            multibrushScrollDictionary[brush.id] = scrollPosition;
        }






        void TabsGUI()
        {
            Event e = Event.current;

            int tabCount = settings.GetTabCount();
            if(tabCount == 0)
                return;

           
            int tabWidth = kTabWidth;
            int tabHeight = kTabHeight;


            float windowWidth = EditorGUIUtility.currentViewWidth;

            tabCount = tabCount + 1;
            int tabsPerLine = Mathf.Max(1, Mathf.FloorToInt(windowWidth / tabWidth));
            int tabLines = Mathf.CeilToInt((float)tabCount / tabsPerLine);
            int tabIndex = 0;
            int tabUnderCursor = -1;


            // Get dragging data
            Brush[] draggingBrushes = null;
            Tab draggingTab = null;
            Rect dragRect = new Rect();
            if (InternalDragAndDrop.IsDragging() || InternalDragAndDrop.IsDragPerform())
            {
                if(InternalDragAndDrop.GetData() is Brush[])
                    draggingBrushes = (Brush[])InternalDragAndDrop.GetData();
                else
                    if(InternalDragAndDrop.GetData() is Tab)
                        draggingTab = (Tab)InternalDragAndDrop.GetData();
            }


            int tabWindowControlID = GUIUtility.GetControlID(s_TabsWindowHash, FocusType.Passive);


            for(int line = 0; line < tabLines; line++)
            {
                Rect lineRect = EditorGUILayout.GetControlRect(GUILayout.Height(tabHeight));

                for(int tab = 0; tab < tabsPerLine && tabIndex < tabCount; tab++, tabIndex++)
                {
                    // '+' Tab
                    if(tabIndex == tabCount-1)
                    {
                        Rect tabPlusRect = new Rect(lineRect.x + tabWidth * tab, lineRect.y, tabWidth/2, tabHeight);
                        EditorGUI.LabelField(tabPlusRect, "", Styles.tabButton);
                        EditorGUI.LabelField(tabPlusRect, "+", Styles.tabLabelText);
                        if(tabPlusRect.Contains(e.mousePosition) && e.type == EventType.MouseDown && e.button == 0)
                        {
                            AddDelayedAction(() => {
                                settings.SetActiveTabIndex(settings.AddNewTab(Strings.newTabName));
                                Repaint();
                            });
                        }
                        break;
                    }



                    Rect tabRect = new Rect(lineRect.x + tabWidth * tab, lineRect.y, tabWidth, tabHeight);


                    // Tab under cursor
                    if(tabRect.Contains(e.mousePosition))
                    {
                        tabUnderCursor = tabIndex;

                        // Drop brushes
                        if (draggingBrushes != null)
                        {
                            EditorGUIUtility.AddCursorRect (tabRect, UnityEditor.MouseCursor.MoveArrow);

                            if(InternalDragAndDrop.IsDragPerform())
                            {
                                AddDelayedAction(() => { settings.MoveBrushes(draggingBrushes, settings.GetTab(tabUnderCursor)); Repaint(); });
                            }
                        }

                        if (draggingTab != null)
                        {
							EditorGUIUtility.AddCursorRect (tabRect, UnityEditor.MouseCursor.MoveArrow);

                            bool isAfter = (e.mousePosition.x - tabRect.xMin) > tabRect.width/2;

                            dragRect = new Rect(tabRect);

                            if(isAfter)
                            {
                                dragRect.xMin = dragRect.xMax - 2;
                                dragRect.xMax = dragRect.xMax + 2;
                            }
                            else
                            {
                                dragRect.xMax = dragRect.xMin + 2;
                                dragRect.xMin = dragRect.xMin - 2;
                            }

                            if(InternalDragAndDrop.IsDragPerform())
                            {
                                settings.InsertSelectedTab(tabIndex, isAfter);
                            }
                        }
                    }


                    EditorGUI.LabelField(tabRect, "", Styles.tabButton);                    
                    if(tabIndex == settings.GetActiveTabIndex())
                        EditorGUI.DrawRect(EditorGUI.IndentedRect(tabRect), Styles.tabTintColor);



                    // Tab Rename
                    if (tabIndex == settings.GetActiveTabIndex())
                    {
                        Tab activeTab = settings.GetTab(tabIndex);

                        // make TextField and set focus to it
                        if (onBeginTabRename)
                        {
                            GUI.SetNextControlName(s_tabTextFieldName);
							activeTab.name = DelayedTextField(tabRect, activeTab.name);

                            TextEditor textEditor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                            if (textEditor != null) {
                                textEditor.SelectAll();
                            }

                            GUI.FocusControl(s_tabTextFieldName);
                            onBeginTabRename = false;
                        }
                        else
                        {
                            // if TextField still in focus - continue rename 
                            if (GUI.GetNameOfFocusedControl() == s_tabTextFieldName)
                            {
                                GUI.SetNextControlName(s_tabTextFieldName);
                                EditorGUI.BeginChangeCheck();
								string newTabName = DelayedTextField(tabRect, activeTab.name);
                                if (EditorGUI.EndChangeCheck())
                                {
                                    Undo.RegisterCompleteObjectUndo(settings, "PP: Tab Rename");
                                    activeTab.name = newTabName;
                                }


                                // Unfocus TextField - finish rename
                                bool onFinishRenameKeyDown = (e.isKey && (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.Escape));
                                if(onFinishRenameKeyDown) {
                                    GUIUtility.keyboardControl = 0;
                                    GUIUtility.hotControl = 0;
                                    e.Use();
                                }

                            } else {
                                // TextField lost focus - finish rename
                                EditorGUI.LabelField(tabRect, GetShortNameForTab(activeTab.name), Styles.tabLabelText);
                            }
                        }
                    }
                    else {
                        EditorGUI.LabelField(tabRect, GetShortNameForTab(settings.GetTab(tabIndex).name), Styles.tabLabelText);
                    }


                }
            }

            // Dragging cursor
            if(draggingTab != null)
                EditorGUI.DrawRect(dragRect, Color.white);



            switch(e.type)
            {
            case EventType.MouseDown:
                if(tabUnderCursor != -1 && e.button == 0)
                {
                    GUIUtility.keyboardControl = tabWindowControlID;
                    GUIUtility.hotControl = 0;

                    settings.SetActiveTabIndex(tabUnderCursor);

                    e.Use();
                }
                break;
            case EventType.MouseUp:
                break;
            case EventType.ContextClick:
                if(tabUnderCursor != -1)
                {
                    GUIUtility.keyboardControl = tabWindowControlID;
                    GUIUtility.hotControl = 0;

                    if(settings.GetActiveTabIndex() != tabUnderCursor)
                    {
                        settings.SetActiveTabIndex(tabUnderCursor);
                    } else {
                        TabMenu().ShowAsContext();
                    }

                    e.Use();
                }
                break;
            case EventType.ValidateCommand:
                if(GUIUtility.keyboardControl == tabWindowControlID)
                {
                    switch(e.commandName)
                    {
                    case "SelectAll":
                    case "Delete":
                    case "Duplicate":                    
                    case "Cut":
                    case "Copy":
                    case "Paste":
                        e.Use();
                        break;
                    }
                }
                break;
            case EventType.ExecuteCommand:
                if(GUIUtility.keyboardControl == tabWindowControlID)
                {                
                    switch(e.commandName)
                    {
                    case "SelectAll":                        
                        AddDelayedAction(() => { settings.GetActiveTab().SelectAllBrushes(); Repaint(); } );
                        e.Use();
                        break;
                    case "Delete":
                        AddDelayedAction(() => { settings.GetActiveTab().DeleteSelectedBrushes(); Repaint(); } );
                        e.Use();
                        break;
                    case "Duplicate":                    
                        AddDelayedAction(() => { settings.GetActiveTab().DuplicateSelectedBrushes(); Repaint(); } );
                        e.Use();
                        break;
                    case "Cut":
                        AddDelayedAction(() => { settings.ClipboardCutBrushes(); Repaint(); } );
                        e.Use();
                        break;
                    case "Copy":
                        settings.ClipboardCopyBrushes();
                        e.Use();
                        break;
                    case "Paste":
                        AddDelayedAction(() => { settings.ClipboardPasteBrushes(); Repaint(); } );    
                        e.Use();
                        break;
                    }
                }
                break;
            }


            // Start drag tab
            //
            if (InternalDragAndDrop.IsDragReady() && tabUnderCursor != -1)
            {
                InternalDragAndDrop.StartDrag(settings.GetTab(tabUnderCursor));
            }
        }



        Texture2D GetPrefabPreviewTexture(GameObject prefab)
        {
            Texture2D previewTexture;

            if((previewTexture = AssetPreview.GetAssetPreview(prefab)) != null)
                return previewTexture;
            
            if ((previewTexture = AssetPreview.GetMiniThumbnail(prefab)) != null)
                return previewTexture;

            return AssetPreview.GetMiniTypeThumbnail(typeof(GameObject));
        }

        void BrushWindowGUI()
        {
            Event e = Event.current;

            Tab currentTab = settings.GetActiveTab();
            int brushCount = currentTab.GetBrushCount();

            int brushIconWidth = kBrushIconWidth;
            int brushIconHeight = kBrushIconHeight;


            Rect brushWindowRect = EditorGUILayout.GetControlRect(GUILayout.Height(Mathf.Max(0.0f, brushWindowHeight)) );
            int brushWindowControlID = GUIUtility.GetControlID(s_BrushWindowHash, FocusType.Passive, brushWindowRect);

            Rect virtualRect = new Rect(brushWindowRect);
            {
                virtualRect.width = Mathf.Max(virtualRect.width - 20, 1); // space for scroll 

                int presetColumns = Mathf.FloorToInt(Mathf.Max(1, (virtualRect.width) / brushIconWidth));
                int virtualRows   = Mathf.CeilToInt((float)brushCount / presetColumns);

                virtualRect.height = Mathf.Max(virtualRect.height, brushIconHeight * virtualRows);
            }



            // draw brushes window background
            GUI.Label(brushWindowRect, "", EditorStyles.helpBox);

            Vector2 brushWindowScrollPos = GetBrushWindowScrollPosition(currentTab);
            brushWindowScrollPos = GUI.BeginScrollView(brushWindowRect, brushWindowScrollPos, virtualRect, false, true);



            // Empty preset list - show Drag&Drop Info
            if (brushCount == 0)
            {
                GUIStyle labelStyle = new GUIStyle(EditorStyles.boldLabel);
                labelStyle.fontStyle = FontStyle.Bold;
                labelStyle.alignment = TextAnchor.MiddleCenter;
                EditorGUI.LabelField(brushWindowRect, "Drag & Drop Prefab Here", labelStyle);
            }
           

            Rect dragRect = new Rect(0, 0, 0, 0);
            int brushIndex = 0;
            int brushUnderCursor = -1;
            int iconDrawCount = 0;


            // if we dragging brushes get it
            Brush[] draggingBrushes = null;
            if ((InternalDragAndDrop.IsDragging() || InternalDragAndDrop.IsDragPerform())
                && InternalDragAndDrop.GetData() is Brush[] && virtualRect.Contains (e.mousePosition))
            {
                draggingBrushes = (Brush[])InternalDragAndDrop.GetData();
				EditorGUIUtility.AddCursorRect (brushWindowRect, UnityEditor.MouseCursor.MoveArrow);
            }



            for (int y = (int)virtualRect.yMin; y < (int)virtualRect.yMax; y += brushIconHeight)
            {
                if (brushIndex >= brushCount)
                    break;

                for (int x = (int)virtualRect.xMin; (x+brushIconWidth) < (int)(virtualRect.xMax); x += brushIconWidth)
                {
                    if (brushIndex >= brushCount)
                        break;


                    Rect brushIconRect = new Rect(x, y, brushIconWidth, brushIconHeight);

                    Rect brushIconRectScrolled = new Rect(brushIconRect);
                    brushIconRectScrolled.position -= brushWindowScrollPos;

                    // only visible incons
                    if(brushIconRectScrolled.Overlaps(brushWindowRect))
                    {


                        if(brushIconRect.Contains(e.mousePosition))
                            brushUnderCursor = brushIndex;



                        Brush brush = currentTab.brushes[brushIndex];



                        // Draw selected Prefab preview blue rect
                        if(brush.selected)
                            EditorGUI.DrawRect(brushIconRect, Styles.colorBlue);




						if (draggingBrushes != null && e.type == EventType.Repaint)
                        {
                            if (brushIconRect.Contains(e.mousePosition))
                            {
                                bool isAfter = (e.mousePosition.x - brushIconRect.xMin) > brushIconRect.width/2;

                                dragRect = new Rect(brushIconRect);

                                if(isAfter)
                                {
                                    dragRect.xMin = dragRect.xMax - 2;
                                    dragRect.xMax = dragRect.xMax + 2;
                                }
                                else
                                {
                                    dragRect.xMax = dragRect.xMin + 2;
                                    dragRect.xMin = dragRect.xMin - 2;
                                }

                                if(InternalDragAndDrop.IsDragPerform())
                                {
                                    currentTab.InsertSelectedBrushes(brushIndex, isAfter);
                                }
                            }  

//							if(brushWindowRect.Contains(e.mousePosition))
//							{
//								if(brushCount-1 == brushIndex)
//								{
//									dragRect = new Rect(brushIconRect);
//									dragRect.xMin = dragRect.xMax - 2;
//									dragRect.xMax = dragRect.xMax + 2;
//								}
//
//								if(InternalDragAndDrop.IsDragPerform())
//									AddDelayedAction(() => currentTab.InsertSelectedBrushes(brushCount-1, true));
//							}    
                        }


                        // Prefab preview 
                        if(e.type == EventType.Repaint)
                        {

                            Rect previewRect = new Rect(brushIconRect.x+2, brushIconRect.y+2, brushIconRect.width-4, brushIconRect.width-4);
                            Color dimmedColor = new Color(0.4f, 0.4f, 0.4f, 1.0f);

                            if(brush.settings.multibrushEnabled)
                            {
                                iconDrawCount += 4;

                                Rect[] icons =
                                {   new Rect(previewRect.x, previewRect.y, previewRect.width/2-1, previewRect.height/2-1),
                                    new Rect(previewRect.x+previewRect.width/2, previewRect.y, previewRect.width/2, previewRect.height/2-1),
                                    new Rect(previewRect.x, previewRect.y+previewRect.height/2, previewRect.width/2-1, previewRect.height/2),
                                    new Rect(previewRect.x+previewRect.width/2, previewRect.y+previewRect.height/2, previewRect.width/2, previewRect.height/2)
                                };

                                GameObject[] prefabs = new GameObject[4];

                                for(int i = 0, j = 0; i < brush.prefabSlots.Length && j < 4; i++)
                                {
                                    if(brush.prefabSlots[i].gameObject != null)
                                    {
                                        prefabs[j] = brush.prefabSlots[i].gameObject;
                                        j++;
                                    }
                                }

                                for(int i = 0; i < 4; i++)
                                {
                                    if(prefabs[i] != null)
                                    {
                                        Texture2D preview = GetPrefabPreviewTexture(prefabs[i]);
                                        if (preview != null)
                                            GUI.DrawTexture(icons[i], preview);
                                    }
                                    else
                                        EditorGUI.DrawRect(icons[i], dimmedColor);
                                }
                            }
                            else // !Multibrush
                            {
                                iconDrawCount += 1;

                                GameObject prefab = brush.GetFirstAssociatedPrefab();

                                if (prefab != null)
                                {
                                    Texture2D preview = GetPrefabPreviewTexture(prefab);
                                    if (preview != null)               
                                        GUI.DrawTexture(previewRect, preview);
                                }
                                else
                                {
                                    EditorGUI.DrawRect(previewRect, dimmedColor);
                                }
                            }

                            // Prefab name
                            Styles.iconLabelText.Draw(brushIconRect, GetShortNameForBrush(brush.name), false, false, false, false);

                            // Color tag
                            if(brush.colorTag != ColorTag.None)
                            {
                                float size = previewRect.width * 0.3f;
                                Rect tagRect = new Rect(previewRect.x + previewRect.width - size, previewRect.y, size, size);

                                Color guiColor = GUI.color;
                                switch(brush.colorTag)
                                {
                                case ColorTag.Red:      GUI.color = Styles.colorTagRed; break;
                                case ColorTag.Orange:   GUI.color = Styles.colorTagOrange; break;
                                case ColorTag.Yellow:   GUI.color = Styles.colorTagYellow; break;
                                case ColorTag.Green:    GUI.color = Styles.colorTagGreen; break;
                                case ColorTag.Blue:     GUI.color = Styles.colorTagBlue; break;
                                case ColorTag.Violet:   GUI.color = Styles.colorTagViolet; break;
                                }
                                GUI.DrawTexture(tagRect, tagTexture, ScaleMode.ScaleToFit, true, 0);
                                GUI.color = guiColor;
                            }
                        }

                    }

                    brushIndex++;                         
                } // x
            } // y


            // Dragging cursor
            if(draggingBrushes != null)
                EditorGUI.DrawRect(dragRect, Color.white);


            // increase preview cache size if needed
            AssetPreviewCacheController.AddCacheRequest("BrushWindow", iconDrawCount * 2);


            switch(e.type)
            {
            case EventType.MouseDown:

                if(virtualRect.Contains(e.mousePosition) && e.button == 0)
                {
                    GUIUtility.keyboardControl = brushWindowControlID;
                    GUIUtility.hotControl = brushWindowControlID;

                    // Double click on background
                    if(e.clickCount == 2 && brushUnderCursor == -1)
                    {
                        this.SendEvent(EditorGUIUtility.CommandEvent("BrushWindowShowObjectSelector"));
                    }


                    if(brushUnderCursor != -1)
                    {
                        #if UNITY_EDITOR_OSX
                        if (e.command)
                        #else
                        if (e.control)
                        #endif
                        {                        
                            currentTab.SelectBrushAdditive(brushUnderCursor);
                        }
                        else if (e.shift)
                        {                        
                            currentTab.SelectBrushRange(brushUnderCursor);
                        }
                        else {
                            if(currentTab.IsBrushSelected(brushUnderCursor) == false) // Deselect other on mouse up for drag operation 
                                currentTab.SelectBrush(brushUnderCursor);
                        }
                    } else {                    
                        currentTab.DeselectAllBrushes();
                    }

                    e.Use();
                }
                break;
            case EventType.MouseUp:
                if (e.button == 0 && virtualRect.Contains(e.mousePosition))
                {
                    GUIUtility.hotControl = 0;

                    #if UNITY_EDITOR_OSX
                    if (brushUnderCursor != -1 && !e.command && !e.shift)
                    #else
                    if (brushUnderCursor != -1 && !e.control && !e.shift)
                    #endif
                    {
                        AddDelayedAction(() => { currentTab.SelectBrush(brushUnderCursor); Repaint(); });
                    }


                    e.Use();
                }
                break;
            case EventType.ContextClick:
                if(brushUnderCursor != -1)
                {
                    if (currentTab.IsBrushSelected(brushUnderCursor))
                    {
                        PresetMenu().ShowAsContext();
                    }
                    else
                    {
                        currentTab.SelectBrush(brushUnderCursor);
                    }
                    e.Use();
                }
                if(virtualRect.Contains (e.mousePosition) && brushUnderCursor == -1)
                {
                    GUIUtility.keyboardControl = brushWindowControlID;

                    BrushWindowMenu().ShowAsContext();
                    e.Use();
                }
                break;
            case EventType.ValidateCommand:
                if(GUIUtility.keyboardControl == brushWindowControlID)
                {
                    switch(e.commandName)
                    {
                    case "SelectAll":
                    case "Delete":
                    case "Duplicate":                    
                    case "Cut":
                    case "Copy":
                    case "Paste":
                    case "BrushWindowShowObjectSelector":
                    case "ObjectSelectorClosed":
                        e.Use();
                        break;
                    }
                }
                break;
            case EventType.ExecuteCommand:
                if(GUIUtility.keyboardControl == brushWindowControlID)
                {                
                    switch(e.commandName)
                    {
                    case "SelectAll":
                        AddDelayedAction(() => { currentTab.SelectAllBrushes(); Repaint(); } );
                        e.Use();
                        break;
                    case "Delete":
                        AddDelayedAction(() => { currentTab.DeleteSelectedBrushes(); Repaint(); } );
                        e.Use();
                        break;
                    case "Duplicate":                    
                        AddDelayedAction(() => { currentTab.DuplicateSelectedBrushes(); Repaint(); } );
                        e.Use();
                        break;
                    case "Cut":
                        AddDelayedAction(() => { settings.ClipboardCutBrushes(); Repaint(); } );
                        e.Use();
                        break;
                    case "Copy":
                        settings.ClipboardCopyBrushes();
                        e.Use();
                        break;
                    case "Paste":
                        AddDelayedAction(() => { settings.ClipboardPasteBrushes(); Repaint(); } );    
                        e.Use();
                        break;
                    case "BrushWindowShowObjectSelector":
                        EditorGUIUtility.ShowObjectPicker<GameObject>(null, false, "", s_BrushWindowObjectPickerHash);
                        e.Use();
                        break;
                    case "ObjectSelectorClosed":
                        if(EditorGUIUtility.GetObjectPickerControlID() == s_BrushWindowObjectPickerHash)
                        {
                            UnityEngine.Object pickedObject = EditorGUIUtility.GetObjectPickerObject();

                            if (pickedObject != null && 
                                pickedObject is GameObject && 
                                PrefabUtility.GetPrefabType(pickedObject as GameObject) != PrefabType.None &&
                                AssetDatabase.Contains(pickedObject))
                            {
								GameObject prefabRoot = PrefabUtility.FindPrefabRoot(pickedObject as GameObject);
								if(prefabRoot)
								{
	                                Undo.RegisterCompleteObjectUndo(settings, "PP: Add Prefab");
									currentTab.brushes.Add(new Brush(prefabRoot));
								}
                            }

                            e.Use();
                        }
                        break;
                    }
                }
                break;
            }



            // Drag brushes
            if (InternalDragAndDrop.IsDragReady() && currentTab.HasSelectedBrushes() && virtualRect.Contains (InternalDragAndDrop.DragStartPosition()) && GUIUtility.hotControl == brushWindowControlID)
            {
                InternalDragAndDrop.StartDrag(currentTab.brushes.FindAll((b) => b.selected).ToArray());
            }



            // Drop operation
            //
            if (e.type == EventType.DragUpdated || e.type == EventType.DragPerform)
            {
                // Relink Prefab
                if (e.shift && brushUnderCursor != -1)
                {
                    if(currentTab.brushes[brushUnderCursor].settings.multibrushEnabled)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                    }
                    else
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Link;

                        if (e.type == EventType.DragPerform) {
                            DragAndDrop.AcceptDrag ();

                            foreach (UnityEngine.Object draggedObject in DragAndDrop.objectReferences)
                            {
                                if (draggedObject is GameObject &&
                                    PrefabUtility.GetPrefabType(draggedObject as GameObject) != PrefabType.None &&
                                    AssetDatabase.Contains(draggedObject))
                                {
									GameObject prefabRoot = PrefabUtility.FindPrefabRoot(draggedObject as GameObject);
									if(prefabRoot)
									{
                                    	Undo.RegisterCompleteObjectUndo(settings, "PP: Relink Prefab");
										currentTab.brushes[brushUnderCursor].AssignPrefab(prefabRoot, 0);                                                        
									}
                                }
                            }
                        }
                    }

                    e.Use();
                }
                else
				{
                    // Add Prefab
                    if(virtualRect.Contains (e.mousePosition))
                    {
                        if (DragAndDrop.objectReferences.Length > 0)
                            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                        if (e.type == EventType.DragPerform)
						{
                            DragAndDrop.AcceptDrag ();

							List<GameObject> draggedGameObjects = new List<GameObject>();

                            foreach (UnityEngine.Object draggedObject in DragAndDrop.objectReferences)
                            {
                                if (draggedObject is GameObject && 
                                    PrefabUtility.GetPrefabType(draggedObject as GameObject) != PrefabType.None &&
                                    AssetDatabase.Contains(draggedObject))
                                {
									GameObject prefabRoot = PrefabUtility.FindPrefabRoot(draggedObject as GameObject);
									if(prefabRoot)
										draggedGameObjects.Add (prefabRoot);               
                                }
                            }

							if(draggedGameObjects.Count > 0)
							{
								Undo.RegisterCompleteObjectUndo(settings, "PP: Add Prefab(s)");

								draggedGameObjects.Sort (delegate(GameObject x, GameObject y) {
									return EditorUtility.NaturalCompare(x.name, y.name);
								});

								foreach(GameObject go in draggedGameObjects)
									currentTab.brushes.Add(new Brush(go));
							}
                        }
                        e.Use();
                    } 
				}
            }




            GUI.EndScrollView();
            SetBrushWindowScrollPosition(currentTab, brushWindowScrollPos);



            // Scroll window while dragging up/down
            if(InternalDragAndDrop.IsDragging() && e.type == EventType.Repaint)
            {
                float scrollSpeed = 300.0f;
                float factor = ((e.mousePosition.y - brushWindowRect.y) - brushWindowRect.height/2.0f) / brushWindowRect.height * 2.0f;
                //factor = Mathf.Abs(factor) < 0.5f ? 0.0f : factor; // dead zone
                factor = factor * factor * Mathf.Sign(factor); // non linear speed 
                brushWindowScrollPos.y += factor * scrollSpeed * Time.deltaTime;
            }



            //            // Status Bar
            //            {
            //                int nameWidth = 150;
            //                int buttonWidth = 70;
            //                Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(16));
            //                EditorGUI.LabelField(new Rect(rect.x, rect.y, nameWidth, rect.height), "", EditorStyles.);
            //                EditorGUI.LabelField(new Rect(rect.x + nameWidth, rect.y, buttonWidth, rect.height), "", Styles.tabButton);
            //            }



            //
            // Brush window resize bar
            {
                Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(kBrushWindowResizeBarHeight));
                int controlID = GUIUtility.GetControlID(s_BrushWindowResizeBarHash, FocusType.Passive, rect);

				EditorGUIUtility.AddCursorRect (rect, UnityEditor.MouseCursor.SplitResizeUpDown);

                switch(e.type)
                {
                case EventType.MouseDown:
                    if(rect.Contains(e.mousePosition) && e.button == 0)
                    {
                        GUIUtility.keyboardControl = controlID;
                        GUIUtility.hotControl = controlID;
                        e.Use();
                    }
                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID && e.button == 0)
                    {
                        GUIUtility.hotControl = 0;
                        e.Use();
                    }
                    break;
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        Rect windowRect = EditorGUIUtility.ScreenToGUIRect(this.position);

                        // Clamp brush window size to min/max values
                        brushWindowHeight = Mathf.Clamp(brushWindowHeight + e.delta.y,
                            kBrushIconHeight, brushWindowHeight + (windowRect.yMax - rect.yMax)); 

                        e.Use();
                    }
                    break;
                case EventType.Repaint:
                    {
                        Rect drawRect = rect;
                        drawRect.yMax -= 2; drawRect.yMin += 2;
                        EditorGUI.DrawRect(drawRect, Styles.backgroundColor);
                    }   
                    break;
                }
            }


        }



        static Rect TransformRect(Rect rect, float xOffset, float yOffset, float xScale, float yScale)
        {
            return new Rect(rect.x + rect.width * xOffset, rect.y + rect.height * yOffset, rect.width * xScale, rect.height * yScale);
        }



        static float RaitingBar(Rect rect, float value)
        {
            Event e = Event.current;
            int controlID = GUIUtility.GetControlID(s_RaitingBarHash, FocusType.Passive, rect);

            switch(e.type)
            {
            case EventType.MouseDown:
                if(rect.Contains(e.mousePosition) && e.button == 0)
                {
                    value = (e.mousePosition.x - rect.x) / rect.width;
                    GUI.changed = true;

                    GUIUtility.keyboardControl = controlID;
                    GUIUtility.hotControl = controlID;
                    e.Use();
                }
                break;
            case EventType.MouseUp:
                if (GUIUtility.hotControl == controlID && e.button == 0)
                {
                    GUIUtility.hotControl = 0;
                    e.Use();
                }
                break;
            case EventType.MouseDrag:
                if (GUIUtility.hotControl == controlID)
                {
                    value = (e.mousePosition.x - rect.x) / rect.width;
                    GUI.changed = true;

                    e.Use();
                }
                break;
            case EventType.Repaint:
                {
                    Rect drawRect = rect;
                    drawRect.width *= value;
                    Color  colorBlue = new Color32 (30, 80, 180, 255);
                    EditorGUI.DrawRect(drawRect, colorBlue);
                }
                break;
            }

            return Mathf.Clamp01(value);
        }



        static void DrawBoxGUI(Rect rect, int lineWidth, Color color)
        {
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width,                       lineWidth), color); // top 
            EditorGUI.DrawRect(new Rect(rect.x, rect.y+rect.height-lineWidth, rect.width, lineWidth), color); // bottom

            EditorGUI.DrawRect(new Rect(rect.x, rect.y, lineWidth,                        rect.height), color); // left
            EditorGUI.DrawRect(new Rect(rect.x+rect.width-lineWidth, rect.y, lineWidth,   rect.height), color); // right
        }



        static float HorizontalScrollBar(Rect windowRect, float position, Rect viewRect)
        {
            Event e = Event.current;


            float scrollbarScale;

            if(viewRect.width != 0f)
                scrollbarScale = Mathf.Clamp01(windowRect.width / viewRect.width);
            else
                scrollbarScale = 1.0f;

            // FIXME: auto hide scroll bar
            //if(scrollbarScale > 0.999f)
            //    return 0f;


            Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(GUI.skin.horizontalScrollbar.fixedHeight));
            rect = EditorGUI.IndentedRect(rect);

            int controlID = GUIUtility.GetControlID(s_HorizontalMiniScrollBarHash, FocusType.Passive, rect);

            Rect scrollBarRect = rect;
            scrollBarRect.x += position * scrollbarScale;
            scrollBarRect.width = rect.width * scrollbarScale;

            switch(e.type)
            {
            case EventType.MouseDown:
                if(rect.Contains(e.mousePosition) && e.button == 0)
                {
                    if(scrollBarRect.Contains(e.mousePosition))
                    {
                        s_HorizontalMiniScrollBar_GrabPosition = e.mousePosition.x;
                        GUIUtility.hotControl = controlID;
                    }
                    else
                    {
                        if(e.mousePosition.x < scrollBarRect.x)
                        {
                            position -= scrollbarScale * viewRect.width * 0.5f;
                        }
                        else
                        {
                            position += scrollbarScale * viewRect.width * 0.5f;
                        }

                        GUI.changed = true;
                    }

                    GUIUtility.keyboardControl = controlID;
                    e.Use();
                }
                break;
            case EventType.MouseUp:
                if (GUIUtility.hotControl == controlID && e.button == 0)
                {
                    GUIUtility.hotControl = 0;
                    e.Use();
                }
                break;
            case EventType.MouseDrag:
                if (GUIUtility.hotControl == controlID)
                {
                    if(rect.width != 0f)
                    {
                        position += (e.mousePosition.x - s_HorizontalMiniScrollBar_GrabPosition) * (viewRect.width / rect.width);
                        s_HorizontalMiniScrollBar_GrabPosition = e.mousePosition.x;
                        GUI.changed = true;
                    }

                    e.Use();
                }
                break;
            case EventType.Repaint:
                {
                    GUI.skin.horizontalScrollbar.Draw(rect, false, false, false, false);
                    GUI.skin.horizontalScrollbarThumb.Draw(scrollBarRect, false, false, false, false);
                }
                break;
            }

            return Mathf.Clamp(position, 0, Math.Max(0, viewRect.width - windowRect.width));
        }



        void MultibrushGUI(Brush brush, bool showSettings)
        {
            Event e = Event.current;

            if(!brush.settings.multibrushEnabled)
                return;



            if ((multibrushSettingsFoldout = FoldoutReset(multibrushSettingsFoldout,
                () => { Undo.RegisterCompleteObjectUndo(settings, "PP: Reset Multibrush Settings"); brush.settings.ResetMultibrush(); },
                "Multibrush")) == false)
            {
                return;
            }


            int iconSize = kMultibrushIconSize;
            int borderSize = kMultibrushIconBorderSize;
            int raitingBarHeight = Mathf.RoundToInt(iconSize * kMultibrushRaitingBarHeightPercent);
            int prefabCount = brush.prefabSlots.Length;





            // increase preview cache size if needed
            AssetPreviewCacheController.AddCacheRequest("MultibrushWindow", prefabCount);

            int raitigBarBorder = 1;
            int totalWindowWitdh = (borderSize + iconSize + borderSize)  * prefabCount;

            ++EditorGUI.indentLevel;

            // Get rect for prefab icons
            Rect windowRect = EditorGUILayout.GetControlRect(GUILayout.Height(borderSize + iconSize + raitigBarBorder + raitingBarHeight + borderSize));            


            // center rect
            if(windowRect.width > totalWindowWitdh)
            {
                float size = windowRect.width - totalWindowWitdh;
                windowRect.width -= size;
                windowRect.x += size/2;
            }
            else
            {
                windowRect = EditorGUI.IndentedRect(windowRect);
            }

            Rect viewRect = windowRect;
            viewRect.width = totalWindowWitdh;




            int controlID = GUIUtility.GetControlID(s_MultibrushHash, FocusType.Passive, windowRect);

            // unfocus other elements
            if(e.type == EventType.MouseDown && windowRect.Contains(e.mousePosition))
            {
                GUIUtility.keyboardControl = controlID;
                GUIUtility.hotControl = 0;
            }



            float scrollPosition = GetMultibrushScrollPosition(brush);


            GUI.BeginGroup(windowRect, EditorStyles.textArea); 

            Rect itemRect = new Rect(borderSize - scrollPosition, borderSize, iconSize, iconSize);
            int itemUnderCursor = -1;

            for(int i = 0; i < prefabCount; i++)
            {
                if(e.type == EventType.Repaint)
                {
                    if(brush.selectedSlot == i)
                    {
                        Rect selectedRect = itemRect;
                        selectedRect.x -= borderSize;
                        selectedRect.y -= borderSize;
                        selectedRect.width += borderSize*2;
                        selectedRect.height += borderSize*2 + raitingBarHeight + 1;

                        EditorGUI.DrawRect(selectedRect, Styles.colorBlue);
                    }
                    
                    if(brush.prefabSlots[i].gameObject != null)
                    {
                        Texture2D preview = GetPrefabPreviewTexture(brush.prefabSlots[i].gameObject);
                        if (preview != null)
                            GUI.DrawTexture(itemRect, preview);

                        // fade disabled prefabs 
                        if(!brush.settings.multibrushSlots[i].enabled)
                            EditorGUI.DrawRect(itemRect, new Color(0.5f, 0.5f, 0.5f, 0.8f));

                        // Prefab name
                        Styles.multibrushIconText.alignment = TextAnchor.LowerCenter;
                        Styles.multibrushIconText.normal.textColor = Color.white;
                        Styles.multibrushIconText.Draw(itemRect, brush.prefabSlots[i].gameObject.name, false, false, false, false);

                        // Remove button
                        Rect closeButtonRect = TransformRect(itemRect, 0.75f, 0.05f, 0.2f, 0.2f);
                        GUI.DrawTexture(closeButtonRect, closeIconTexture);
                    }
                    else
                    {
                        Styles.multibrushIconText.alignment = TextAnchor.MiddleCenter;
                        Styles.multibrushIconText.normal.textColor = Color.white;
                        Styles.multibrushIconText.Draw(itemRect, "Drag\nprefab\nhere", false, false, false, false);

                        EditorGUI.DrawRect(itemRect, new Color(0, 0, 0, 0.15f));
                    }

                    // prefab number    
                    Styles.multibrushIconText.alignment = TextAnchor.UpperLeft;
                    Styles.multibrushIconText.normal.textColor = Color.white;
                    Styles.multibrushIconText.Draw(itemRect, "#" + i, false, false, false, false);
                }


                if(itemRect.Contains(e.mousePosition))
                {
                    itemUnderCursor = i;

                    if (e.type == EventType.MouseDown && e.button == 0)
                    {
                        // Clear button
                        Rect buttonRect = TransformRect(itemRect, 0.75f, 0.05f, 0.2f, 0.2f);
                        if (buttonRect.Contains(e.mousePosition))
                        {
                            int index = i;
                            AddDelayedAction(() => { brush.ClearPrefab(index); });
                            e.Use();
                        }

                        if(e.clickCount == 2) // Double click
                        {
                            multibrushItemDoubleClicked = i;
                            this.SendEvent(EditorGUIUtility.CommandEvent("MultibrushShowObjectSelector"));
                            e.Use();
                        }
                        else // single click - select
                        {
                            brush.SelectPrefab(i);
                            e.Use();
                        }
                    }

                    // Context Menu
                    if (e.type == EventType.ContextClick)
                    {
                        MultibrushPrefabMenu(brush, i).ShowAsContext();
                        e.Use();
                    }
                }

                // Rating Bar
                float raiting = RaitingBar(new Rect(itemRect.x, itemRect.y + itemRect.height + raitigBarBorder, itemRect.width, raitingBarHeight), brush.settings.multibrushSlots[i].raiting);
                if(raiting != brush.settings.multibrushSlots[i].raiting)
                {
                    brush.settings.multibrushSlots[i].raiting = raiting;
                }


                itemRect.x += iconSize + borderSize*2;
            }

            GUI.EndGroup();



            // Scroll Bar
            scrollPosition = HorizontalScrollBar(windowRect, scrollPosition, viewRect);
            SetMultibrushScrollPosition(brush, scrollPosition);




            // Object Picker Window
            if (e.type == EventType.ValidateCommand)
            {
                switch(e.commandName)
                {
                case "MultibrushShowObjectSelector":
                case "ObjectSelectorClosed":
                    e.Use();
                    break;
                }
            }
            if (e.type == EventType.ExecuteCommand)
            {                
                switch(e.commandName)
                {
                case "MultibrushShowObjectSelector":
                    EditorGUIUtility.ShowObjectPicker<GameObject>(null, false, "", s_MultibrushObjectPickerHash);
                    e.Use();
                    break;
                case "ObjectSelectorClosed":
                    if(EditorGUIUtility.GetObjectPickerControlID() == s_MultibrushObjectPickerHash)
                    {
                        UnityEngine.Object pickedObject = EditorGUIUtility.GetObjectPickerObject();

                        if (pickedObject != null && 
                            pickedObject is GameObject && 
                            PrefabUtility.GetPrefabType(pickedObject as GameObject) != PrefabType.None &&
                            AssetDatabase.Contains(pickedObject))
                        {
							GameObject prefabRoot = PrefabUtility.FindPrefabRoot(pickedObject as GameObject);
							if(prefabRoot)
							{
								brush.AssignPrefab(prefabRoot, multibrushItemDoubleClicked);                                     
							}
                        }

                        e.Use();
                    }
                    break;
                }
            }




            // Drag & Drop
            if((e.type == EventType.DragUpdated || e.type == EventType.DragPerform) && itemUnderCursor != -1)
            {
                if(DragAndDrop.objectReferences.Length > 1)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (e.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag ();

                        foreach (UnityEngine.Object draggedObject in DragAndDrop.objectReferences)
                        {
                            if (draggedObject is GameObject &&
                                PrefabUtility.GetPrefabType(draggedObject as GameObject) != PrefabType.None &&
                                AssetDatabase.Contains(draggedObject))
                            {
								GameObject prefabRoot = PrefabUtility.FindPrefabRoot(draggedObject as GameObject);
								if(prefabRoot)
								{
									brush.AssignPrefabToEmptySlot(prefabRoot);
								}
                            }
                        }
                    }
                }
                else
                {
                    if(brush.prefabSlots[itemUnderCursor].gameObject != null)
                        DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                    else
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    

                    if (e.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag ();

                        foreach (UnityEngine.Object draggedObject in DragAndDrop.objectReferences)
                        {
                            if (draggedObject is GameObject &&
                                PrefabUtility.GetPrefabType(draggedObject as GameObject) != PrefabType.None &&
                                AssetDatabase.Contains(draggedObject))
                            {
								GameObject prefabRoot = PrefabUtility.FindPrefabRoot(draggedObject as GameObject);
								if(prefabRoot)
								{
									brush.AssignPrefab(prefabRoot, itemUnderCursor);
								}
                            }
                        }
                    }
                }

                e.Use();
            }






            if(showSettings)
            {
                int selectedSlot = brush.selectedSlot;
                BrushSettings brushSettings = brush.settings;
                
                multibrushSlotSettingsFoldout = FoldoutReset(multibrushSlotSettingsFoldout,
                    () => { Undo.RegisterCompleteObjectUndo(settings, "PP: Reset Slot Settings"); brush.settings.ResetMultibrushSlot(brush.selectedSlot); },
                    "Slot #" + brush.selectedSlot + " Settings");

                if(multibrushSlotSettingsFoldout)
                {
                    ++EditorGUI.indentLevel;
                    MakeUndoOnChange(ref brushSettings.multibrushSlots[selectedSlot].position, Vector3Field(new GUIContent("Position"), brushSettings.multibrushSlots[selectedSlot].position));
                    MakeUndoOnChange(ref brushSettings.multibrushSlots[selectedSlot].rotation, Vector3Field(new GUIContent("Rotation"), brushSettings.multibrushSlots[selectedSlot].rotation));
                    MakeUndoOnChange(ref brushSettings.multibrushSlots[selectedSlot].scale, Vector3Field(new GUIContent("Scale"), brushSettings.multibrushSlots[selectedSlot].scale));
                    --EditorGUI.indentLevel;
                }

                MakeUndoOnChange(ref brushSettings.multibrushPaintSelectedSlot, EditorGUILayout.Toggle (new GUIContent("Paint Selected Slot"), brushSettings.multibrushPaintSelectedSlot));
                GUI.enabled = !brushSettings.multibrushPaintSelectedSlot;
                MakeUndoOnChange(ref brushSettings.multibrushMode, (MultibrushMode)EditorGUILayout.EnumPopup (new GUIContent("Mode"), brushSettings.multibrushMode));
                if(brushSettings.multibrushMode == MultibrushMode.Pattern)
                {
                    MakeUndoOnChange(ref brushSettings.multibrushPattern, EditorGUILayout.TextField (new GUIContent("Pattern"), brushSettings.multibrushPattern));
					MakeUndoOnChange(ref brushSettings.multibrushPatternContinue, EditorGUILayout.Toggle (new GUIContent("Continue Pattern", "Continue pattern from last position in new stroke"), brushSettings.multibrushPatternContinue));
                }
                GUI.enabled = true;
            }



            --EditorGUI.indentLevel;

            EditorGUILayout.Space();
        }


        static Vector2 Vector2Field(GUIContent label, Vector2 value)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label);
            Rect position = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, EditorStyles.numberField);
            s_Vector2Floats[0] = value.x;
            s_Vector2Floats[1] = value.y;
            EditorGUI.MultiFloatField(position, s_XYLabels, s_Vector2Floats);
            EditorGUILayout.EndHorizontal();
            return new Vector2(s_Vector2Floats[0], s_Vector2Floats[1]);
        }

        static Vector3 Vector3Field(GUIContent label, Vector3 value)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label);
            Rect position = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, EditorStyles.numberField);
            s_Vector3Floats[0] = value.x;
            s_Vector3Floats[1] = value.y;
            s_Vector3Floats[2] = value.z;
            EditorGUI.MultiFloatField(position, s_XYZLabels, s_Vector3Floats);
            EditorGUILayout.EndHorizontal();
            return new Vector3(s_Vector3Floats[0], s_Vector3Floats[1], s_Vector3Floats[2]);
        }


//        static void Vector2MinMaxField(GUIContent label, SerializedProperty minProperty, SerializedProperty maxProperty)
//        {
//            EditorGUILayout.BeginHorizontal();
//            EditorGUILayout.PrefixLabel(label);
//            Rect position = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, EditorStyles.numberField);
//            s_Vector2Floats[0] = minProperty.floatValue;
//            s_Vector2Floats[1] = maxProperty.floatValue;
//            EditorGUI.BeginChangeCheck();
//            EditorGUI.MultiFloatField(position, s_MinMaxLabels, s_Vector2Floats);
//            if (EditorGUI.EndChangeCheck())
//            {
//                minProperty.floatValue = s_Vector2Floats[0];
//                maxProperty.floatValue = s_Vector2Floats[1];
//            }
//
//            EditorGUILayout.EndHorizontal();
//        }

        public bool FlatTexturedButton(Rect rect, GUIContent content)
        {
            Event e = Event.current;
            int controlID = GUIUtility.GetControlID(s_FlatButtonHash, FocusType.Passive, rect);
            bool clicked = false;

            switch(e.type)
            {
            case EventType.MouseDown:
                if(rect.Contains(e.mousePosition) && e.button == 0)
                {
                    GUIUtility.keyboardControl = controlID;
                    GUIUtility.hotControl = controlID;
                    e.Use();
                }
                break;
            case EventType.MouseUp:
                if (GUIUtility.hotControl == controlID && e.button == 0)
                {
                    GUI.changed = clicked = true;

                    GUIUtility.hotControl = 0;
                    e.Use();
                }
                break;
            case EventType.Repaint:
                {
                    if(GUIUtility.hotControl == controlID)
                        EditorGUI.DrawRect(rect, Styles.colorBlue);

                    GUI.DrawTexture(new Rect(rect.x+2, rect.y+2, rect.width-4, rect.height-4), content.image);
                }
                break;
            }

            return clicked;
        }



        public bool Foldout(bool foldout, string content)
        {
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, EditorStyles.foldout);

            EditorGUI.DrawRect(EditorGUI.IndentedRect(rect), Styles.foldoutTintColor);

            Rect foldoutRect = rect;
            foldoutRect.width = EditorGUIUtility.singleLineHeight;
            foldout = EditorGUI.Foldout(rect, foldout, "", true);

            rect.x += EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(rect, content, EditorStyles.boldLabel);

            return foldout;
        }

        public bool FoldoutToggle(bool foldout, SerializedProperty toggle, string content)
        {
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, EditorStyles.foldout);

            EditorGUI.DrawRect(EditorGUI.IndentedRect(rect), Styles.foldoutTintColor);

            Rect foldoutRect = rect;
            foldoutRect.width = EditorGUIUtility.singleLineHeight;

            Rect toggleRect = rect;
            toggleRect.x += EditorGUIUtility.singleLineHeight;
            toggleRect.width = EditorGUIUtility.singleLineHeight;
            toggle.boolValue = EditorGUI.Toggle(toggleRect, toggle.boolValue);


            foldout = EditorGUI.Foldout(rect, foldout, "", true);

            rect.x += EditorGUIUtility.singleLineHeight * 2;
            EditorGUI.LabelField(rect, content, EditorStyles.boldLabel);

            return foldout;
        }

        public bool FoldoutReset(bool foldout, Action reset, string content)
        {
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, EditorStyles.foldout);

            EditorGUI.DrawRect(EditorGUI.IndentedRect(rect), Styles.foldoutTintColor);

            Rect foldoutRect = rect;
            foldoutRect.width = EditorGUIUtility.singleLineHeight;

            Rect buttonRect = rect;
            buttonRect.x += rect.width - EditorGUIUtility.singleLineHeight;
            buttonRect.width = EditorGUIUtility.singleLineHeight;
            if(FlatTexturedButton(buttonRect, new GUIContent(resetIconTexture, "Reset")))
                reset.Invoke();

            foldout = EditorGUI.Foldout(rect, foldout, "", true);

            rect.x += EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(rect, content, EditorStyles.boldLabel);

            return foldout;
        }

        public bool FoldoutToggleReset(bool foldout, SerializedProperty toggle, Action reset, string content)
        {
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, EditorStyles.foldout);

            EditorGUI.DrawRect(EditorGUI.IndentedRect(rect), Styles.foldoutTintColor);

            Rect foldoutRect = rect;
            foldoutRect.width = EditorGUIUtility.singleLineHeight;

            Rect toggleRect = rect;
            toggleRect.x += EditorGUIUtility.singleLineHeight;
            toggleRect.width = EditorGUIUtility.singleLineHeight;
            toggle.boolValue = EditorGUI.Toggle(toggleRect, toggle.boolValue);

            Rect buttonRect = rect;
            buttonRect.x += rect.width - EditorGUIUtility.singleLineHeight;
            buttonRect.width = EditorGUIUtility.singleLineHeight;
            if(FlatTexturedButton(buttonRect, new GUIContent(resetIconTexture, "Reset")))
                reset.Invoke();

            foldout = EditorGUI.Foldout(rect, foldout, "", true);

            rect.x += EditorGUIUtility.singleLineHeight * 2;
            EditorGUI.LabelField(rect, content, EditorStyles.boldLabel);

            return foldout;
        }

      
        static LayerMask LayerMaskField(GUIContent label, LayerMask layerMask)
        {
            List<string> layers = new List<string>(32);
            List<int> layerNumbers = new List<int>(32);

            for (int i = 0; i < 32; i++)
            {
                string layerName = LayerMask.LayerToName(i);
                if (layerName != "") {
                    layers.Add(layerName);
                    layerNumbers.Add(i);
                }
            }
            int maskWithoutEmpty = 0;
            for (int i = 0; i < layerNumbers.Count; i++) {
                if (((1 << layerNumbers[i]) & layerMask.value) != 0)
                    maskWithoutEmpty |= (1 << i);
            }
            maskWithoutEmpty = EditorGUILayout.MaskField(label, maskWithoutEmpty, layers.ToArray());
            int mask = 0;
            for (int i = 0; i < layerNumbers.Count; i++) {
                if ((maskWithoutEmpty & (1 << i)) != 0)
                    mask |= (1 << layerNumbers[i]);
            }
            layerMask.value = mask;
            return layerMask;
        }


		bool Button(Rect position, GUIContent content, bool pressed)
		{
			return GUI.Toggle(position, pressed, content, "button") != pressed;
		}



#if (UNITY_5_2 || UNITY_5_3 || UNITY_5_4_OR_NEWER) 
		void PivotEditorGUI()
		{
			// load unity built-in shader 
			if(previewWireMaterial == null)
			{
				Shader shader = Shader.Find("Hidden/Internal-Colored"); 
				if(shader)
				{				
					previewWireMaterial = new Material(shader);
					previewWireMaterial.hideFlags = HideFlags.HideAndDontSave;
				}
			}

			if(previewRender.camera == null || previewWireMaterial == null)
			{
				EditorGUILayout.HelpBox("Fail to initialize", MessageType.Error);
				return;
			}



			Event e = Event.current;
			Camera camera = previewRender.camera;
			Brush brush = settings.GetActiveTab().GetFirstSelectedBrush();
			if(brush == null)
				return;


			Action FrameView = () =>
			{
				Bounds bounds = previewObjectBounds;
				bounds.Encapsulate(brush.settings.multibrushSlots[previewSelectedSlot].pivotOffset);

				switch(previewSide)
				{
				case PreviewSide.Right:
					previewWindowPosition.x = -bounds.center.z;
					previewWindowPosition.y = -bounds.center.y;
					previewWindowScale = 1f/(new Vector3(bounds.extents.z, bounds.extents.y)).magnitude * 0.75f;
					break;
				case PreviewSide.Top:
					previewWindowPosition.x = -bounds.center.x;
					previewWindowPosition.y = -bounds.center.z;
					previewWindowScale = 1f/(new Vector3(bounds.extents.x, bounds.extents.z)).magnitude * 0.75f;
					break;
				case PreviewSide.Front:
					previewWindowPosition.x = bounds.center.x;
					previewWindowPosition.y = -bounds.center.y;
					previewWindowScale = 1f/(new Vector3(bounds.extents.x, bounds.extents.y)).magnitude * 0.75f;
					break;	
				}
			};



			// Check preview object change
			if(previewObject != brush.prefabSlots[previewSelectedSlot].gameObject)
			{
				previewObject = brush.prefabSlots[previewSelectedSlot].gameObject;

				if(previewObject)
				{
					previewObjectBounds = new Bounds();
					foreach (var renderer in previewObject.GetComponentsInChildren<Renderer>())
					{
						if(previewObjectBounds.size.magnitude < Mathf.Epsilon)
							previewObjectBounds = renderer.bounds;
						else
							previewObjectBounds.Encapsulate (renderer.bounds);
					}
				}

				FrameView();
			}



			Bounds boundsWithPivot = previewObjectBounds;
			boundsWithPivot.Encapsulate(brush.settings.multibrushSlots[previewSelectedSlot].pivotOffset);

			camera.orthographic = true;
			camera.orthographicSize = 1f/previewWindowScale;

			switch(previewSide)
			{
			case PreviewSide.Right:
				camera.transform.position = new Vector3(boundsWithPivot.center.x + (boundsWithPivot.extents.x + 5f), -previewWindowPosition.y, -previewWindowPosition.x);
				camera.transform.forward = -Vector3.right;
				camera.nearClipPlane = 1f;
				camera.farClipPlane = boundsWithPivot.size.x + 10f;
				break;
			case PreviewSide.Top:
				camera.transform.position = new Vector3(-previewWindowPosition.x, boundsWithPivot.center.y + (boundsWithPivot.extents.y + 5f), -previewWindowPosition.y);
				camera.transform.forward = -Vector3.up;
				camera.nearClipPlane = 1f;
				camera.farClipPlane = boundsWithPivot.size.y + 10f;
				break;
			case PreviewSide.Front:
				camera.transform.position = new Vector3(previewWindowPosition.x, -previewWindowPosition.y, boundsWithPivot.center.z + (boundsWithPivot.extents.z + 5f));
				camera.transform.forward = -Vector3.forward;
				camera.nearClipPlane = 1f;
				camera.farClipPlane = boundsWithPivot.size.z + 10f;
				break;	
			}




			float kPivotSize = 0.25f;

			Rect previewRect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(GUILayout.Height(EditorGUIUtility.currentViewWidth-40), GUILayout.Width(EditorGUIUtility.currentViewWidth-40)));
			int controlID = GUIUtility.GetControlID(s_PivotEditorHash, FocusType.Passive, previewRect);
			int pivotControlID = GUIUtility.GetControlID(s_PivotEditorPivotHash, FocusType.Passive);


			switch(e.GetTypeForControl(pivotControlID))
			{
			case EventType.MouseDown:
				if(e.button == 0 && previewRect.Contains(e.mousePosition))
				{
					#if (UNITY_5_4_OR_NEWER)
					Vector2 cameraMousePosition = new Vector2(e.mousePosition.x - previewRect.x, previewRect.height -  (e.mousePosition.y - previewRect.y));
					cameraMousePosition.x *= camera.pixelWidth / (previewRect.width * EditorGUIUtility.pixelsPerPoint);
					cameraMousePosition.y *= camera.pixelHeight / (previewRect.height * EditorGUIUtility.pixelsPerPoint);

					Ray ray = camera.ScreenPointToRay(EditorGUIUtility.PointsToPixels(cameraMousePosition));
					#else
					Vector2 cameraMousePosition = new Vector2(e.mousePosition.x - previewRect.x, previewRect.height -  (e.mousePosition.y - previewRect.y));
					cameraMousePosition.x *= camera.pixelWidth / previewRect.width;
					cameraMousePosition.y *= camera.pixelHeight / previewRect.height;

					Ray ray = camera.ScreenPointToRay(cameraMousePosition);
					#endif
					Bounds bounds = new Bounds(brush.settings.multibrushSlots[previewSelectedSlot].pivotOffset + Vector3.one * 0.125f * kPivotSize / previewWindowScale, Vector3.one * 0.25f * kPivotSize / previewWindowScale);
					if(bounds.IntersectRay(ray))
					{
						float d;
						Plane plane = new Plane(-camera.transform.forward, brush.settings.multibrushSlots[previewSelectedSlot].pivotOffset);
						if(plane.Raycast(ray, out d))
						{
							dragOffset = brush.settings.multibrushSlots[previewSelectedSlot].pivotOffset - ray.GetPoint(d);
						}
						GUIUtility.keyboardControl = pivotControlID;
						GUIUtility.hotControl = pivotControlID;
						e.Use();
					}
				}
				break;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == pivotControlID)
				{
					GUIUtility.hotControl = 0;
					e.Use();
				}
				break;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == pivotControlID)
				{
					#if (UNITY_5_4_OR_NEWER)
					Vector2 cameraMousePosition = new Vector2(e.mousePosition.x - previewRect.x, previewRect.height -  (e.mousePosition.y - previewRect.y));
					cameraMousePosition.x *= camera.pixelWidth / (previewRect.width * EditorGUIUtility.pixelsPerPoint);
					cameraMousePosition.y *= camera.pixelHeight / (previewRect.height * EditorGUIUtility.pixelsPerPoint);

					Ray ray = camera.ScreenPointToRay(EditorGUIUtility.PointsToPixels(cameraMousePosition));
					#else
					Vector2 cameraMousePosition = new Vector2(e.mousePosition.x - previewRect.x, previewRect.height -  (e.mousePosition.y - previewRect.y));
					cameraMousePosition.x *= camera.pixelWidth / previewRect.width;
					cameraMousePosition.y *= camera.pixelHeight / previewRect.height;

					Ray ray = camera.ScreenPointToRay(cameraMousePosition);
					#endif
					Plane plane = new Plane(-camera.transform.forward, brush.settings.multibrushSlots[previewSelectedSlot].pivotOffset);

					float d;
					if(plane.Raycast(ray, out d))
					{
						brush.settings.multibrushSlots[previewSelectedSlot].pivotOffset = ray.GetPoint(d) + dragOffset;
					}

					e.Use();
				}
				break;
			}



			switch(e.GetTypeForControl(controlID))
			{
			case EventType.MouseDown:
				if(previewRect.Contains(e.mousePosition) && e.alt)
				{
					GUIUtility.keyboardControl = controlID;
					GUIUtility.hotControl = controlID;
					e.Use();
				}
				break;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == controlID)
				{
					GUIUtility.hotControl = 0;
					e.Use();
				}
				break;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == controlID)
				{
					if (e.button == 0 || e.button == 2)
						previewWindowPosition += new Vector2(e.delta.x, -e.delta.y) * (2.0f /  Mathf.Max(previewRect.width, 1)) / previewWindowScale;

					if (e.button == 1)
					{   
						float aspect = previewRect.width / Mathf.Max(previewRect.height, 1);
						previewWindowScale += e.delta.magnitude / aspect * Mathf.Sign(Vector2.Dot(e.delta, new Vector2(1.0f, 0.0f))) * (2.0f / previewRect.width) * (previewWindowScale) * 0.5f;
						previewWindowScale = Mathf.Max(previewWindowScale, 0.01f);
					}

					e.Use();
				}
				break;
			case EventType.ScrollWheel:
				if(previewRect.Contains(e.mousePosition) && e.alt)
				{
					float aspect = previewRect.width / Mathf.Max(previewRect.height, 1);

					previewWindowScale += e.delta.magnitude / aspect * Mathf.Sign(Vector2.Dot(e.delta, new Vector2(1.0f, -0.1f).normalized)) * (2.0f / previewRect.width) * (previewWindowScale) * 5.5f;                
					previewWindowScale = Mathf.Max(previewWindowScale, 0.01f);

					e.Use();
				}
				break;
			case EventType.Repaint:
				{
					previewRender.BeginPreview (previewRect);

					previewWireMaterial.SetInt("_ZWrite", 0);
					previewWireMaterial.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);

					// Clear BG
					previewWireMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
					previewWireMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
					previewWireMaterial.SetColor("_Color", new Color(0.192156866f, 0.192156866f, 0.192156866f, 1f));
					previewRender.DrawMesh(previewQuadMesh, Matrix4x4.TRS(camera.transform.position + Vector3.Scale(camera.transform.position,camera.transform.forward), Quaternion.LookRotation(camera.transform.forward), new Vector3(camera.orthographicSize, camera.orthographicSize, camera.orthographicSize)), previewWireMaterial, 0);
					previewRender.Render();


					GL.wireframe = true;
					previewWireMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
					previewWireMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);


					// Render grid
					float winSizeMM = camera.orthographicSize * 1000f;

					float log = Mathf.Log(winSizeMM, 10);
					float floor = Mathf.Floor(log);
					float ceil = Mathf.Ceil(log);

					float gridSize1 = Mathf.Pow(10, floor) / 100f;
					float gridSize2 = Mathf.Pow(10, ceil) / 100f;

					Func<Vector3, float, Vector3> Vector3Snap = (Vector3 v, float snap) =>
					{
						return new Vector3(Mathf.Round(v.x / snap) * snap, Mathf.Round(v.y / snap) * snap, Mathf.Round(v.z / snap) * snap);
					};

					previewWireMaterial.SetColor("_Color", new Color(0.4f, 0.4f, 0.4f, ceil - log));
					previewRender.DrawMesh(previewGridMesh, Matrix4x4.TRS(Vector3Snap(camera.transform.position + Vector3.Scale(camera.transform.position,camera.transform.forward), gridSize2 / 20),
						Quaternion.LookRotation(camera.transform.forward),
						new Vector3(gridSize1, gridSize1, gridSize1)), previewWireMaterial, 0);

					previewRender.Render();

					previewWireMaterial.SetColor("_Color", new Color(0.4f, 0.4f, 0.4f, log - floor));
					previewRender.DrawMesh(previewGridMesh, Matrix4x4.TRS(Vector3Snap(camera.transform.position + Vector3.Scale(camera.transform.position,camera.transform.forward), gridSize2 / 20),
						Quaternion.LookRotation(camera.transform.forward),
						new Vector3(gridSize2, gridSize2, gridSize2)), previewWireMaterial, 0);

					previewRender.Render();

					if(previewObject != null)
					{
						// Render model
						previewWireMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
						previewWireMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
						previewWireMaterial.SetColor("_Color", new Color(0.7f, 0.7f, 0.7f, 1f));

						foreach (MeshFilter meshFilter in previewObject.GetComponentsInChildren<MeshFilter>())
						{
							if(meshFilter.sharedMesh)
							{
								int subMeshCount = meshFilter.sharedMesh.subMeshCount;
								for(int i = 0; i < subMeshCount; i++)									
									previewRender.DrawMesh(meshFilter.sharedMesh, meshFilter.transform.localToWorldMatrix, previewWireMaterial, i);
							}
						}
						previewRender.Render();


						// Render Pivot
						Matrix4x4 pivotMatrix = Matrix4x4.TRS(brush.settings.multibrushSlots[previewSelectedSlot].pivotOffset, Quaternion.identity, Vector3.one * kPivotSize / previewWindowScale);

						previewWireMaterial.SetColor("_Color", Color.white);
						previewRender.DrawMesh(previewPivotMesh, pivotMatrix, previewWireMaterial, 0);
						previewRender.DrawMesh(previewPivotMesh, pivotMatrix, previewWireMaterial, 2);
						previewRender.Render();

						GL.wireframe = false;
						previewWireMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
						previewWireMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
						previewWireMaterial.SetColor("_Color", new Color(1, 1, 1, 0.35f));
						previewRender.DrawMesh(previewPivotMesh, pivotMatrix, previewWireMaterial, 1);
						previewRender.Render();

					}
					GL.wireframe = false;
					previewRender.EndAndDrawPreview (previewRect);
				}
				break;
			}


			// ResetPivot - Frame  - Right/Top/Front buttons
			EditorGUI.BeginChangeCheck();
			previewSide = (PreviewSide)GUI.Toolbar(new Rect (previewRect.xMax - 160, previewRect.yMin + 10, 150, EditorGUIUtility.singleLineHeight), (int)previewSide, s_PreviewSideNames);
			if(EditorGUI.EndChangeCheck())
				FrameView();

			if(GUI.Button(new Rect (previewRect.xMax - 220, previewRect.yMin + 10, 50, EditorGUIUtility.singleLineHeight), "Frame"))
				FrameView();

			if(GUI.Button(new Rect (previewRect.xMax - 300, previewRect.yMin + 10, 70, EditorGUIUtility.singleLineHeight), "Reset Pivot"))
			{
				brush.settings.multibrushSlots[previewSelectedSlot].pivotOffset = Vector3.zero;
			}




			// multibrush slots
			{
				int slotCount = brush.prefabSlots.Length;
				int assigndSlotCount = 0;

				for(int i = 0; i < slotCount; i++)
				{
					if (brush.prefabSlots[i].gameObject != null)
					{
						if(brush.prefabSlots[previewSelectedSlot].gameObject == null)
							previewSelectedSlot = i;
						
						assigndSlotCount++;
					}
				}
				
				if(brush.settings.multibrushEnabled && assigndSlotCount > 1)
				{
					int kIconSize = (int)Mathf.Min((EditorGUIUtility.currentViewWidth-20) / slotCount, 64f);

					// increase preview cache size if needed
					AssetPreviewCacheController.AddCacheRequest("PivotEditorGUI", slotCount);

					using (new EditorGUILayout.HorizontalScope ())
					{
						GUILayout.FlexibleSpace();
						using (new EditorGUILayout.HorizontalScope (EditorStyles.helpBox))
						{					
							for(int i = 0; i < slotCount; i++)
							{
								if (brush.prefabSlots[i].gameObject == null)
									continue;

								Rect prefabRect = EditorGUILayout.GetControlRect(GUILayout.Width(kIconSize), GUILayout.Height(kIconSize));

								if(e.type == EventType.Repaint)
								{
									if (brush.prefabSlots[i].gameObject != null)
										GUI.DrawTexture(prefabRect, GetPrefabPreviewTexture(brush.prefabSlots[i].gameObject));
									else
										EditorGUI.DrawRect(prefabRect, new Color(0, 0, 0, 0.15f));

									if(previewSelectedSlot == i)
										EditorGUI.DrawRect(prefabRect, new Color(0, 0, 1, 0.15f));
								}

								if(e.type == EventType.MouseDown && prefabRect.Contains(e.mousePosition))
								{
									previewSelectedSlot = i;
									e.Use();
								}

							}
						}
						GUILayout.FlexibleSpace();
					}
				}
			}
		}

#endif

        void BrushSettingsGUI()
        {
            if(sceneSettings == null)
                return;

            Tab activeTab = settings.GetActiveTab();
            bool hasSelectedBrushes = activeTab.HasSelectedBrushes();
            bool hasMultipleSelectedBrushes = activeTab.HasMultipleSelectedBrushes();
            Brush brush = activeTab.GetFirstSelectedBrush();
            BrushSettings brushSettings = brush != null ? brush.settings : null;

            // Begin Scroll area
            windowScrollPos = EditorGUILayout.BeginScrollView(windowScrollPos);

            switch(lastTool)
            {
            case PaintTool.None:
            case PaintTool.Brush:
                {
                    if (!hasSelectedBrushes || brush == null)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.Space();
                        EditorGUILayout.HelpBox(Strings.selectBrush, MessageType.Info);
                        EditorGUILayout.Space();
                        EditorGUILayout.Space();
                        break;
                    }
                    else if (hasMultipleSelectedBrushes)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.Space();
                        EditorGUILayout.HelpBox(Strings.multiSelBrush, MessageType.Info);
                        EditorGUILayout.Space();
                        EditorGUILayout.Space();
                        break;
                    }


                   


                    brushSettingsFoldout = FoldoutReset(brushSettingsFoldout,
                        () => { Undo.RegisterCompleteObjectUndo(settings, "PP: Reset Brush Settings"); brushSettings.ResetBrush(); }, Strings.brushSettingsFoldout);
                    if (brushSettingsFoldout)
                    {
                        ++EditorGUI.indentLevel;
                        MakeUndoOnChange(ref brush.name, DelayedTextField (Strings.brushName, brush.name));
                        MakeUndoOnChange(ref brushSettings.brushRadius, EditorGUILayout.Slider(Strings.brushRadius, brushSettings.brushRadius, 0.01f, settings.maxBrushRadius));
                        MakeUndoOnChange(ref brushSettings.brushSpacing, EditorGUILayout.Slider(Strings.brushSpacing, brushSettings.brushSpacing, 0.01f, settings.maxBrushSpacing));
                        MakeUndoOnChange(ref brushSettings.brushOverallScale, Mathf.Max(0.01f, EditorGUILayout.FloatField (new GUIContent("Overall Brush Scale"), brushSettings.brushOverallScale)));
                        MakeUndoOnChange(ref brushSettings.brushOverlapCheck, EditorGUILayout.Toggle (new GUIContent("Overlap Check"), brushSettings.brushOverlapCheck));
                        if(brushSettings.brushOverlapCheck)
                        {
                            ++EditorGUI.indentLevel;
                            MakeUndoOnChange(ref brushSettings.brushOverlapCheckMode, (OverlapCheckMode)EditorGUILayout.EnumPopup (new GUIContent("Mode"), brushSettings.brushOverlapCheckMode));
                            if(brushSettings.brushOverlapCheckMode == OverlapCheckMode.Distance)
                            {
                                MakeUndoOnChange(ref brushSettings.brushOverlapDistance, EditorGUILayout.FloatField (new GUIContent("Min Distance"), brushSettings.brushOverlapDistance));
                            }
                            MakeUndoOnChange(ref brushSettings.brushOverlapCheckObjects, (OverlapCheckObjects)EditorGUILayout.EnumPopup (new GUIContent("Check"), brushSettings.brushOverlapCheckObjects));
                            if(brushSettings.brushOverlapCheckObjects == OverlapCheckObjects.OtherLayers)
                            {
                                MakeUndoOnChange(ref brushSettings.brushOverlapCheckLayers, LayerMaskField(new GUIContent("Layers"), brushSettings.brushOverlapCheckLayers));
                            }
                            --EditorGUI.indentLevel;
                        }
                        --EditorGUI.indentLevel;
                    }


                    MultibrushGUI(brush, true);


                    positionSettingsFoldout = FoldoutReset(positionSettingsFoldout,
                        () => { Undo.RegisterCompleteObjectUndo(settings, "PP: Reset Position Settings"); brushSettings.ResetPosition(); }, Strings.positionSettingsFoldout);
                    if (positionSettingsFoldout)
                    {
                        ++EditorGUI.indentLevel;
                        MakeUndoOnChange(ref brushSettings.positionOffset, Vector3Field(Strings.brushPositionOffset, brushSettings.positionOffset));

						#if (UNITY_5_2 || UNITY_5_3 || UNITY_5_4_OR_NEWER) 
						pivotEditorFoldout = EditorGUILayout.Foldout(pivotEditorFoldout, "Pivot Editor");
						if (pivotEditorFoldout)
						{
							++EditorGUI.indentLevel;
							PivotEditorGUI();
							--EditorGUI.indentLevel;
						}
						#endif

                        --EditorGUI.indentLevel;
                    }





                    orientationSettingsFoldout = FoldoutReset(orientationSettingsFoldout,
                        () => { Undo.RegisterCompleteObjectUndo(settings, "PP: Reset Orientation Settings"); brushSettings.ResetOrientation(); }, Strings.orientationSettingsFoldout);
                    if (orientationSettingsFoldout)
                    {
                        ++EditorGUI.indentLevel;
                        MakeUndoOnChange(ref brushSettings.orientationTransformMode, (TransformMode)EditorGUILayout.EnumPopup (Strings.brushOrientationTransform, brushSettings.orientationTransformMode));
                        MakeUndoOnChange(ref brushSettings.orientationMode, (OrientationMode)EditorGUILayout.EnumPopup (Strings.brushOrientationMode, brushSettings.orientationMode));
                        MakeUndoOnChange(ref brushSettings.flipOrientation, EditorGUILayout.Toggle (Strings.brushFlipOrientation, brushSettings.flipOrientation));
                        MakeUndoOnChange(ref brushSettings.rotation, Vector3Field(Strings.brushRotation, brushSettings.rotation));
                        MakeUndoOnChange(ref brushSettings.randomizeOrientationX, EditorGUILayout.Slider (Strings.brushRandomizeOrientationX, brushSettings.randomizeOrientationX, 0.0f, 100.0f));
                        MakeUndoOnChange(ref brushSettings.randomizeOrientationY, EditorGUILayout.Slider (Strings.brushRandomizeOrientationY, brushSettings.randomizeOrientationY, 0.0f, 100.0f));
                        MakeUndoOnChange(ref brushSettings.randomizeOrientationZ, EditorGUILayout.Slider (Strings.brushRandomizeOrientationZ, brushSettings.randomizeOrientationZ, 0.0f, 100.0f));
                        --EditorGUI.indentLevel;
                    }



                    scaleSettingsFoldout = FoldoutReset(scaleSettingsFoldout,
                        () => { Undo.RegisterCompleteObjectUndo(settings, "PP: Reset Scale Settings"); brush.settings.ResetScale(); }, Strings.scaleSettingsFoldout);
                    if (scaleSettingsFoldout)
                    {
                        ++EditorGUI.indentLevel;
                        MakeUndoOnChange(ref brushSettings.scaleTransformMode, (TransformMode)EditorGUILayout.EnumPopup (Strings.brushScaleTransformMode, brushSettings.scaleTransformMode));
                        MakeUndoOnChange(ref brushSettings.scaleMode, (AxisMode)EditorGUILayout.EnumPopup (Strings.brushScaleMode, brushSettings.scaleMode));
                        if (brushSettings.scaleMode == AxisMode.Uniform)
                        {
                            MakeUndoOnChange(ref brushSettings.scaleUniformMin, EditorGUILayout.FloatField (Strings.brushScaleUniformMin, brushSettings.scaleUniformMin));
                            MakeUndoOnChange(ref brushSettings.scaleUniformMax, EditorGUILayout.FloatField (Strings.brushScaleUniformMax, brushSettings.scaleUniformMax));
                        }
                        else
                        {
                            MakeUndoOnChange(ref brushSettings.scalePerAxisMin, Vector3Field (Strings.brushScalePerAxisMin, brushSettings.scalePerAxisMin));
                            MakeUndoOnChange(ref brushSettings.scalePerAxisMax, Vector3Field (Strings.brushScalePerAxisMax, brushSettings.scalePerAxisMax));
                        }
                        MakeUndoOnChange(ref brushSettings.scaleAux, EditorGUILayout.FloatField (new GUIContent("Aux Scale"), brushSettings.scaleAux));
                        --EditorGUI.indentLevel;
                    }

                    if(brush.settings.slopeEnabled)
                    {
                        slopeFilterFoldout = FoldoutReset(slopeFilterFoldout, 
                            () => { Undo.RegisterCompleteObjectUndo(settings, "PP: Reset Slope Filter"); brush.settings.ResetSlopeFilter(); }, "Slope Filter");
                        if (slopeFilterFoldout)
                        {                        
                            ++EditorGUI.indentLevel;
                            MakeUndoOnChange(ref brushSettings.slopeAngleMin, Mathf.Clamp(EditorGUILayout.FloatField (new GUIContent("Min Angle"), brushSettings.slopeAngleMin), 0f, 180f));
                            MakeUndoOnChange(ref brushSettings.slopeAngleMax, Mathf.Clamp(EditorGUILayout.FloatField (new GUIContent("Max Angle"), brushSettings.slopeAngleMax), 0f, 180f));
                            MakeUndoOnChange(ref brushSettings.slopeVector, (SlopeVector)EditorGUILayout.EnumPopup (new GUIContent("Reference Vector"), brushSettings.slopeVector));
                            if(brushSettings.slopeVector == SlopeVector.Custom)
                            {
                                ++EditorGUI.indentLevel;
								EditorGUILayout.BeginHorizontal();
                                MakeUndoOnChange(ref brushSettings.slopeVectorCustom, Vector3Field(new GUIContent("Custom Vector"), brushSettings.slopeVectorCustom));
								if(Button(EditorGUILayout.GetControlRect(GUILayout.Width(18)), new GUIContent("+", "Pick vector in scene view"), onPickSlopeNormal))
								{
									onPickSlopeNormal = true;
									PickObject("Pick Slope Vector", (go, raycastInfo) => {
										if(raycastInfo.isHit) {
											MakeUndoOnChange(ref brushSettings.slopeVectorCustom, raycastInfo.normal);
										}
									});
								}
								if(onPickSlopeNormal)
									onPickSlopeNormal = onPickObject;
								EditorGUILayout.EndHorizontal();
                                --EditorGUI.indentLevel;
                            }
                            MakeUndoOnChange(ref brushSettings.slopeVectorFlip, EditorGUILayout.Toggle (new GUIContent("Flip Vector"), brushSettings.slopeVectorFlip));
                            --EditorGUI.indentLevel;
                        }
                    }

                    if(brush.settings.gridEnabled)
                    {
                        gridFoldout = FoldoutReset(gridFoldout,
                            () => { Undo.RegisterCompleteObjectUndo(settings, "PP: Reset Grid"); brush.settings.ResetGrid(); }, "Grid");
                        if(gridFoldout)
                        {                        
                            ++EditorGUI.indentLevel;
                            MakeUndoOnChange(ref brushSettings.gridOrigin, Vector2Field (new GUIContent("Origin"), brushSettings.gridOrigin));
                            MakeUndoOnChange(ref brushSettings.gridStep, Vector2.Max(new Vector2(0.001f, 0.001f), Vector2Field (new GUIContent("Step"), brushSettings.gridStep)));
                            MakeUndoOnChange(ref brushSettings.gridAngle, EditorGUILayout.FloatField (new GUIContent("Angle"), brushSettings.gridAngle));
                            MakeUndoOnChange(ref brushSettings.gridPlane, (GridPlane)EditorGUILayout.EnumPopup (new GUIContent("Plane"), brushSettings.gridPlane));
                            if(brushSettings.gridPlane == GridPlane.Custom)
                            {
                                ++EditorGUI.indentLevel;
								EditorGUILayout.BeginHorizontal();
                                MakeUndoOnChange(ref brushSettings.gridNormal, Vector3Field(new GUIContent("Normal Vector"), brushSettings.gridNormal));
								if(Button(EditorGUILayout.GetControlRect(GUILayout.Width(18)), new GUIContent("+", "Pick vector in scene view"), onPickGridNormal))
								{
									onPickGridNormal = true;
									PickObject("Pick Grid Normal Vector", (go, raycastInfo) => {
										if(raycastInfo.isHit) {
											MakeUndoOnChange(ref brushSettings.gridNormal, raycastInfo.normal);
										}
									});
								}
								if(onPickGridNormal)
									onPickGridNormal = onPickObject;
								EditorGUILayout.EndHorizontal();
                                --EditorGUI.indentLevel;
                            }
                            --EditorGUI.indentLevel;
                        }
                    }


                }
                break;
            case PaintTool.PrecisePlace:
                {
                    if (!hasSelectedBrushes || brush == null)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.Space();
                        EditorGUILayout.HelpBox(Strings.selectBrush, MessageType.Info);
                        EditorGUILayout.Space();
                        EditorGUILayout.Space();
                        break;
                    }
                    else if (hasMultipleSelectedBrushes)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.Space();
                        EditorGUILayout.HelpBox(Strings.multiSelBrush, MessageType.Info);
                        EditorGUILayout.Space();
                        EditorGUILayout.Space();
                        break;
                    }






                    precisePlaceFoldout = FoldoutReset(precisePlaceFoldout,
                        () => {
                            Undo.RegisterCompleteObjectUndo(settings, "PP: Reset Precise Place Settings");
                            settings.ResetPrecisePlaceSnapSettings();
                        }, "Precise Place");
                    
                    if (precisePlaceFoldout)
                    {   

                        ++EditorGUI.indentLevel;


                        MakeUndoOnChange(ref brush.name, DelayedTextField (Strings.brushName, brush.name));


                        EditorGUILayout.Space();


                        MakeUndoOnChange(ref brushSettings.ppOrientationMode, (PrecisePlaceOrientationMode)EditorGUILayout.EnumPopup(new GUIContent("Orientation"), brushSettings.ppOrientationMode));
                        MakeUndoOnChange(ref brushSettings.ppFlipOrientation, EditorGUILayout.Toggle(new GUIContent("Flip Orientation"), brushSettings.ppFlipOrientation));

                        MakeUndoOnChange(ref brushSettings.ppFixedRotation, EditorGUILayout.Toggle(new GUIContent("Fixed Rotation"), brushSettings.ppFixedRotation));
                        if(brushSettings.ppFixedRotation)
                        {
							++EditorGUI.indentLevel;
                            MakeUndoOnChange(ref brushSettings.ppFixedRotationTransformMode, (TransformMode)EditorGUILayout.EnumPopup(new GUIContent("Rotation Transform"), brushSettings.ppFixedRotationTransformMode));
                            MakeUndoOnChange(ref brushSettings.ppFixedRotationValue, Vector3Field (new GUIContent("Rotation"), brushSettings.ppFixedRotationValue));     
							--EditorGUI.indentLevel;
                        }

                        MakeUndoOnChange(ref brushSettings.ppFixedScale, EditorGUILayout.Toggle(new GUIContent("Fixed Scale"), brushSettings.ppFixedScale));
                        if(brushSettings.ppFixedScale)
                        {
							++EditorGUI.indentLevel;
                            MakeUndoOnChange(ref brushSettings.ppFixedScaleTransformMode, (TransformMode)EditorGUILayout.EnumPopup(new GUIContent("Scale Transform"), brushSettings.ppFixedScaleTransformMode));
                            MakeUndoOnChange(ref brushSettings.ppFixedScaleValue,  Vector3Field (new GUIContent("Scale"), brushSettings.ppFixedScaleValue));
							--EditorGUI.indentLevel;
                        }

                        --EditorGUI.indentLevel;
                    }



                    precisePlaceSnapFoldout = FoldoutReset(precisePlaceSnapFoldout,
                        () => {
                            Undo.RegisterCompleteObjectUndo(settings, "PP: Reset Precise Place Snap Settings");
                            settings.ResetPrecisePlaceSnapSettings();
                        }, "Snap");

                    if (precisePlaceSnapFoldout)
                    {
                        ++EditorGUI.indentLevel;

                        EditorGUILayout.BeginHorizontal();
                        if(currentTool == PaintTool.PrecisePlace && Event.current.control)
                            EditorGUILayout.Toggle(new GUIContent("Snap Rotation"), !settings.ppSnapRotation);
                        else
                            MakeUndoOnChange(ref settings.ppSnapRotation, EditorGUILayout.Toggle(new GUIContent("Snap Rotation"), settings.ppSnapRotation));

                        EditorGUILayout.LabelField("(Hold Control)", Styles.leftGreyMiniLabel);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        if(currentTool == PaintTool.PrecisePlace && Event.current.shift)
                            EditorGUILayout.Toggle(new GUIContent("Snap Scale"), !settings.ppSnapScale);
                        else
                            MakeUndoOnChange(ref settings.ppSnapScale, EditorGUILayout.Toggle(new GUIContent("Snap Scale"), settings.ppSnapScale));
                        EditorGUILayout.LabelField("(Hold Shift)", Styles.leftGreyMiniLabel);
                        EditorGUILayout.EndHorizontal();

                        MakeUndoOnChange(ref settings.ppSnapRotationValue, Mathf.Max(EditorGUILayout.FloatField (new GUIContent("Snap Rotation Angle"), settings.ppSnapRotationValue), 0.001f));
                        MakeUndoOnChange(ref settings.ppSnapScaleValue, Mathf.Max(EditorGUILayout.FloatField (new GUIContent("Snap Scale Value"), settings.ppSnapScaleValue), 0.001f));

                        --EditorGUI.indentLevel;
                    }


                    MultibrushGUI(brush, true);



                    positionSettingsFoldout = FoldoutReset(positionSettingsFoldout,
                        () => { Undo.RegisterCompleteObjectUndo(settings, "PP: Reset Position Settings"); brushSettings.ResetPosition(); }, Strings.positionSettingsFoldout);
                    if (positionSettingsFoldout)
                    {
                        ++EditorGUI.indentLevel;
                        MakeUndoOnChange(ref brushSettings.positionOffset, Vector3Field(Strings.brushPositionOffset, brushSettings.positionOffset));
                        --EditorGUI.indentLevel;
                    }


                    if(brush.settings.gridEnabled)
                    {
                        gridFoldout = FoldoutReset(gridFoldout,
                            () => { Undo.RegisterCompleteObjectUndo(settings, "PP: Reset Grid"); brush.settings.ResetGrid(); }, "Grid");
                        if(gridFoldout)
                        {                        
                            ++EditorGUI.indentLevel;
                            MakeUndoOnChange(ref brushSettings.gridOrigin, Vector2Field (new GUIContent("Origin"), brushSettings.gridOrigin));
                            MakeUndoOnChange(ref brushSettings.gridStep, Vector2Field (new GUIContent("Step"), brushSettings.gridStep));
                            MakeUndoOnChange(ref brushSettings.gridAngle, EditorGUILayout.FloatField (new GUIContent("Angle"), brushSettings.gridAngle));
                            MakeUndoOnChange(ref brushSettings.gridPlane, (GridPlane)EditorGUILayout.EnumPopup (new GUIContent("Plane"), brushSettings.gridPlane));
                            if(brushSettings.gridPlane == GridPlane.Custom)
                            {
                                ++EditorGUI.indentLevel;
								EditorGUILayout.BeginHorizontal();
								MakeUndoOnChange(ref brushSettings.gridNormal, Vector3Field(new GUIContent("Normal Vector"), brushSettings.gridNormal));
								if(Button(EditorGUILayout.GetControlRect(GUILayout.Width(18)), new GUIContent("+", "Pick vector in scene view"), onPickGridNormal))
								{
									onPickGridNormal = true;
									PickObject("Pick Grid Normal Vector", (go, raycastInfo) => {
										if(raycastInfo.isHit) {
											MakeUndoOnChange(ref brushSettings.gridNormal, raycastInfo.normal);
										}
									});
								}
								if(onPickGridNormal)
									onPickGridNormal = onPickObject;
								EditorGUILayout.EndHorizontal();
                                --EditorGUI.indentLevel;
                            }
                            --EditorGUI.indentLevel;
                        }
                    }
                }
                break;
            case PaintTool.Erase:
                {
                    if (!hasSelectedBrushes && !settings.eraseByLayer)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.Space();
                        EditorGUILayout.HelpBox("Select Brushes (you can select multiple)", MessageType.Info);
                        EditorGUILayout.Space();
                        EditorGUILayout.Space();
                    }


                    eraseSettingsFoldout = FoldoutReset(eraseSettingsFoldout,
                        () => {
                            Undo.RegisterCompleteObjectUndo(settings, "PP: Reset Erase Settings");
                            settings.ResetEraseSettings();
                        }, "Erase Settings");
                    
                    if (eraseSettingsFoldout)
                    {
                        ++EditorGUI.indentLevel;
                        if(hasSelectedBrushes && !hasMultipleSelectedBrushes)
                            MakeUndoOnChange(ref brush.name, DelayedTextField (Strings.brushName, brush.name));
                        
                        MakeUndoOnChange(ref settings.eraseBrushRadius, EditorGUILayout.Slider(Strings.brushRadius, settings.eraseBrushRadius, 0.01f, settings.maxBrushRadius));
                        MakeUndoOnChange(ref settings.eraseByLayer, EditorGUILayout.Toggle (new GUIContent("Erase By Layer"), settings.eraseByLayer));
                        GUI.enabled = settings.eraseByLayer;
                        MakeUndoOnChange(ref settings.eraseLayers, LayerMaskField (new GUIContent("Erase Layers"), settings.eraseLayers));
                        GUI.enabled = true;
                        --EditorGUI.indentLevel;
                    }


                    if(brush != null && !hasMultipleSelectedBrushes)
                        MultibrushGUI(brush, false);
                }
                break;
            case PaintTool.Select:
                if (!hasSelectedBrushes && !settings.selectByLayer)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("Select Brushes (you can select multiple)", MessageType.Info);
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                }



                selectSettingsFoldout = FoldoutReset(selectSettingsFoldout,
                    () => {
                        Undo.RegisterCompleteObjectUndo(settings, "PP: Reset Select Settings");
                        settings.ResetSelectSettings();
                    }, "Select Settings");
                
                if (selectSettingsFoldout)
                {
                    ++EditorGUI.indentLevel;

                    if(hasSelectedBrushes && !hasMultipleSelectedBrushes)
                        MakeUndoOnChange(ref brush.name, DelayedTextField (Strings.brushName, brush.name));
                    
                    MakeUndoOnChange(ref settings.selectBrushRadius, EditorGUILayout.Slider(Strings.brushRadius, settings.selectBrushRadius,  0.01f, settings.maxBrushRadius));
                                      

                    MakeUndoOnChange(ref settings.selectByLayer, EditorGUILayout.Toggle (new GUIContent("Select By Layer"), settings.selectByLayer));

                    GUI.enabled = settings.selectByLayer;
                    MakeUndoOnChange(ref settings.selectLayers, LayerMaskField (new GUIContent("Select Layers"), settings.selectLayers));
                    GUI.enabled = true;



                    EditorGUILayout.BeginHorizontal ();
                    EditorGUILayout.PrefixLabel("Mode");
                    MakeUndoOnChange(ref settings.selectMode, (SelectMode)GUILayout.Toolbar((int)settings.selectMode, Strings.selectModeNames, GUILayout.MaxWidth(250)));
                    EditorGUILayout.EndHorizontal ();


                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();

                    GUILayout.FlexibleSpace();
                    if(GUI.Button(EditorGUILayout.GetControlRect(GUILayout.Width(100)), "Deselect All"))
                    {
                        selectionTool.selectedObjects.Clear();
                        Selection.objects = new UnityEngine.Object[0];
                    }
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();

                    --EditorGUI.indentLevel;
                }

                if(brush != null && !hasMultipleSelectedBrushes)
                    MultibrushGUI(brush, false);


                break;
            }







            commonSettingsFoldout = FoldoutReset(commonSettingsFoldout,
                () => {
                    Undo.RegisterCompleteObjectUndo(settings, "PP: Reset Common Settings");
                    settings.ResetCommonSettings();
                },
                Strings.commonSettingsFoldout);
            if (commonSettingsFoldout)
            {
                ++EditorGUI.indentLevel;

                MakeUndoOnChange(ref settings.paintOnSelected, EditorGUILayout.Toggle(Strings.brushPaintOnSelected, settings.paintOnSelected));
                GUI.enabled = !settings.paintOnSelected;
                MakeUndoOnChange(ref settings.paintLayers, LayerMaskField(Strings.brushPaintOnLayers, settings.paintLayers));
                MakeUndoOnChange(ref settings.ignoreLayers, LayerMaskField(Strings.brushIgnoreLayers, settings.ignoreLayers));
                GUI.enabled = true;


                MakeUndoOnChange(ref settings.placeUnder, (Placement)EditorGUILayout.EnumPopup(Strings.brushPlaceUnder, settings.placeUnder));
                if (settings.placeUnder == Placement.CustomObject)
                {
                    ++EditorGUI.indentLevel;

                    GameObject newParentForPrefabs = (GameObject)EditorGUILayout.ObjectField(Strings.brushCustomSceneObject, sceneSettings.parentForPrefabs, typeof(GameObject), true);
                    if (newParentForPrefabs != sceneSettings.parentForPrefabs)
                    {
                        Undo.RegisterCompleteObjectUndo(sceneSettings, "PP: Change value");

                        sceneSettings.parentForPrefabs = newParentForPrefabs;

                        if (sceneSettings.parentForPrefabs != null && AssetDatabase.Contains(sceneSettings.parentForPrefabs)) {
                            sceneSettings.parentForPrefabs = null;                        
                        }

                        Utility.MarkActiveSceneDirty();
                    }
                    --EditorGUI.indentLevel;
                }

                MakeUndoOnChange(ref settings.groupPrefabs, EditorGUILayout.Toggle(Strings.brushGroupPrefabs, settings.groupPrefabs));

                MakeUndoOnChange(ref settings.overwritePrefabLayer, EditorGUILayout.Toggle(Strings.brushOverwritePrefabLayer, settings.overwritePrefabLayer));
                GUI.enabled = settings.overwritePrefabLayer;
                MakeUndoOnChange(ref settings.prefabPlaceLayer, EditorGUILayout.LayerField(Strings.brushPrefabPlaceLayer, settings.prefabPlaceLayer));
                GUI.enabled = true;

                --EditorGUI.indentLevel;
            }


            EditorGUILayout.EndScrollView();

        }


        void MakeUndoOnChange<T>(ref T rvalue, T lvalue) 
        {
            if(!rvalue.Equals(lvalue))
            {
                RegisterValueUndo();
                rvalue = lvalue;
            }
        }

        void RegisterValueUndo()
        {
            Undo.RegisterCompleteObjectUndo(settings, "PP: Change value");
        }


        void OnMainGUI ()
        {            
			// Logo
			Rect logoRect = EditorGUILayout.GetControlRect(GUILayout.Height(56));
			if(Event.current.type == EventType.Repaint)
				Styles.logoFont.Draw(logoRect, "nTools|PrefabPainter", false, false, false, false);


            float kToolbarWidth = 280;
            float kToolbarHeight = 22;		

            // Tool select
            EditorGUILayout.BeginHorizontal ();
            GUILayout.FlexibleSpace ();
            currentTool = (PaintTool)GUI.Toolbar(EditorGUILayout.GetControlRect(GUILayout.MaxWidth(kToolbarWidth), GUILayout.Height(kToolbarHeight)), (int)currentTool, toolbarTextures);
            GUILayout.FlexibleSpace ();
            EditorGUILayout.EndHorizontal ();



            switch(currentTool)
            {
            case PaintTool.None:

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("No Tool Selected");
                EditorGUILayout.EndVertical();


                TabsGUI();
                BrushWindowGUI();
                BrushSettingsGUI();
                break;
            case PaintTool.Brush:

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Brush Tool");
                EditorGUILayout.LabelField("Click on surface to place objects", EditorStyles.wordWrappedMiniLabel);
                EditorGUILayout.EndVertical();


                TabsGUI();
                BrushWindowGUI();
                BrushSettingsGUI();
                break;
            case PaintTool.PrecisePlace:

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Precise Place Tool");
                EditorGUILayout.LabelField("Hold Control/Shift to snap rotationand scale", EditorStyles.wordWrappedMiniLabel);
                EditorGUILayout.EndVertical();

                TabsGUI();
                BrushWindowGUI();
                BrushSettingsGUI();
                break;
            case PaintTool.Erase:

				EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Erase Tool");
                EditorGUILayout.LabelField("Erase objects by layer/type", EditorStyles.wordWrappedMiniLabel);
                EditorGUILayout.EndVertical();

                TabsGUI();
                BrushWindowGUI();
                BrushSettingsGUI();
                break;
            case PaintTool.Select:

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Select Tool");
                EditorGUILayout.LabelField("Select objects by layer/type", EditorStyles.wordWrappedMiniLabel);
                EditorGUILayout.EndVertical();

                TabsGUI();
                BrushWindowGUI();
                BrushSettingsGUI();
                break;
            case PaintTool.Settings:

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Settings");
                EditorGUILayout.EndVertical();


                toolSettingsFoldout = FoldoutReset(toolSettingsFoldout,
                    () => {
                        Undo.RegisterCompleteObjectUndo(settings, "PP: Reset PrefabPainter Settings");
                        settings.ResetToolSettings();
                    },
                    "Settings");

                if (toolSettingsFoldout)
                {
                    float defaultLabelWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = EditorGUIUtility.labelWidth * 2.0f;

                    ++EditorGUI.indentLevel;

                    MakeUndoOnChange(ref settings.maxBrushRadius, Mathf.Max(EditorGUILayout.FloatField(Strings.settingsMaxBrushRadius, settings.maxBrushRadius), 0.5f));
                    MakeUndoOnChange(ref settings.maxBrushSpacing, Mathf.Max(EditorGUILayout.FloatField(Strings.settingsMaxBrushSpacing, settings.maxBrushSpacing), 0.5f));
                    MakeUndoOnChange(ref settings.surfaceCoords, (SurfaceCoords)EditorGUILayout.EnumPopup(Strings.settingsSurfaceCoords, settings.surfaceCoords));

                    MakeUndoOnChange(ref settings.hideSceneSettingsObject, EditorGUILayout.Toggle (Strings.settingsHideSceneSettingsObject, settings.hideSceneSettingsObject));
                    if(sceneSettings != null)
                    {
                        const HideFlags kHideFlags = HideFlags.HideInHierarchy|HideFlags.HideInInspector|HideFlags.DontSaveInBuild;
                        const HideFlags kDontHideFlags = HideFlags.DontSaveInBuild;
                        HideFlags flags;

                        if(settings.hideSceneSettingsObject)
                            flags = kHideFlags;
                        else
                            flags = kDontHideFlags;
                        
                        if(sceneSettings.gameObject.hideFlags != flags)
                        {
                            sceneSettings.gameObject.hideFlags = flags;

                            Utility.MarkActiveSceneDirty();
                            EditorApplication.RepaintHierarchyWindow();
                        }
                    }

                    MakeUndoOnChange(ref settings.groupName, EditorGUILayout.TextField(Strings.settingsGroupName, settings.groupName));
                    MakeUndoOnChange(ref settings.gridRaycastHeight, EditorGUILayout.FloatField(new GUIContent("Grid Raycast Height"), settings.gridRaycastHeight));
                    MakeUndoOnChange(ref settings.useAdditionalVertexStreams, EditorGUILayout.Toggle (new GUIContent("\"additionalVertexStreams\" (Use with caution)", "Used by some assets to deform meshes, can cause bugs with GI"),
                        settings.useAdditionalVertexStreams));


                    --EditorGUI.indentLevel;


                    EditorGUIUtility.labelWidth = defaultLabelWidth;
                }

                helpFoldout = Foldout(helpFoldout, "Help");
                if(helpFoldout)
                {
                    ++EditorGUI.indentLevel;

                    //EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    //EditorGUILayout.LabelField(Strings.helpText);
                    //EditorGUILayout.LabelField("Click on surface to place objects", EditorStyles.wordWrappedMiniLabel);
                    //EditorGUILayout.EndVertical();

                    EditorGUILayout.HelpBox(Strings.helpText, MessageType.None, true);

                    --EditorGUI.indentLevel;

                }
                break;
            }


            EditorUtility.SetDirty(settings);
        }




        void OnGUI ()
        {
            // Close this window if new one created
            if (s_activeWindow != null && s_activeWindow != this)
                this.Close ();


            InternalDragAndDrop.OnBeginGUI();

            HandleKeyboardEvents();

            OnMainGUI ();

            InternalDragAndDrop.OnEndGUI();

            // repaint every time for dinamic effects like drag scrolling
            if(InternalDragAndDrop.IsDragging() || InternalDragAndDrop.IsDragPerform())
                Repaint();
        }

#endregion // GUI


    } // class PrefabPainter




#region Dialogs
    //
    // class SaveSettingsDialog
    //
    public class SaveSettingsDialog : EditorWindow
    {
		BrushPresetDatabase presetDatabase = null;
        string settingsName = "Untitled Settings";
        PrefabPainterSettings settings = null;
        bool focusTextField = true;


        public static bool IsValidFileName(string fileName)
        {   
            return System.Text.RegularExpressions.Regex.IsMatch(fileName,
                @"^(?!^(PRN|AUX|CLOCK\$|NUL|CON|COM\d|LPT\d|\..*)(\..+)?$)[^\x00-\x1f\\?*:\"";|/<>]+$",
                System.Text.RegularExpressions.RegexOptions.CultureInvariant);
        }

        void SaveSettings(string newSettingsName)
        {
            Brush selectedBrush = settings.GetActiveTab().GetFirstSelectedBrush();
            if (selectedBrush == null)
                return;
            
            BrushSettings newSettings = new BrushSettings(selectedBrush.settings);

			presetDatabase.SavePreset(newSettingsName, newSettings);
        }

        void OnGUI()
        {
			if(settings == null || presetDatabase == null)
				return;

            bool isNameInvalid = false;
            bool isWillOverwrite = false;


            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Name:");
            GUI.SetNextControlName("nameTextField");
            settingsName = EditorGUILayout.TextField(settingsName);

            if (focusTextField)
            {
                focusTextField = false;

                TextEditor textEditor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                if (textEditor != null)
                    textEditor.SelectAll();

                GUI.FocusControl("nameTextField");
            }

			foreach(string saved in presetDatabase.presets) {
                if (String.Equals(saved, settingsName, StringComparison.CurrentCultureIgnoreCase)) {
                    isWillOverwrite = true;
                    break;
                }
            }

            if(settingsName.Length == 0 || !IsValidFileName(settingsName))
                isNameInvalid = true;


            if(isWillOverwrite) {                
                EditorGUILayout.LabelField("This will overwrite existing settings!");
            } else
                if(isNameInvalid) {
                    EditorGUILayout.LabelField("Invalid name.");
                } else {
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if(GUILayout.Button ("Cancel", GUILayout.Width (70))) {
                Close();
            }

            GUI.enabled = !isNameInvalid;
            if(GUILayout.Button (isWillOverwrite ? "Overwrite" : "Save", GUILayout.Width (70))) {
                SaveSettings(settingsName);
                Close();
            }
            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();

            if(Event.current.isKey && Event.current.keyCode == KeyCode.Escape)
                Close();

            if(Event.current.isKey && Event.current.keyCode == KeyCode.Return && !isNameInvalid)
            {
                SaveSettings(settingsName);
                Close();
            }
        }

        void OnLostFocus() {
            Close();
        }

		public static void ShowDialog(BrushPresetDatabase presetDatabase, PrefabPainterSettings settings)
        {
            Vector2 size = new Vector2(300, 90);

			presetDatabase.Refresh();
            SaveSettingsDialog window = (SaveSettingsDialog)EditorWindow.GetWindow (typeof (SaveSettingsDialog));
            window.minSize = size;
            window.maxSize = size;
            window.position = new Rect((Screen.currentResolution.width-size.x)/2, (Screen.currentResolution.height-size.x)/2, size.x, size.y);
            Utility.SetWindowTitle(window, new GUIContent("Save Settings", ""));
            window.settings = settings;
			window.presetDatabase = presetDatabase;
            window.ShowPopup();
        }
    }










    //
    // class DeleteSettingsDialog
    //
    public class DeleteSettingsDialog : EditorWindow
    {
		public BrushPresetDatabase presetDatabase = null;
        string[] settingsNames = null;
        int selected = 0;
        bool deleteAll = false;

        void OnGUI()
        {
			if (presetDatabase == null)
				return;

            if (settingsNames == null) {
				settingsNames = presetDatabase.presets.ToArray ();
			}

			EditorGUILayout.BeginVertical ();
            
            EditorGUILayout.Space();
            GUI.enabled = settingsNames.Length != 0 && deleteAll == false;
            selected = EditorGUILayout.Popup(selected, settingsNames);
            GUI.enabled = true;

            GUI.enabled = settingsNames.Length != 0;
            deleteAll = EditorGUILayout.Toggle("Delete All", deleteAll);
            GUI.enabled = true;

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if(GUILayout.Button ("Cancel", GUILayout.Width (70))) {
                Close();            
            }
            GUI.enabled = settingsNames.Length != 0;
            if(GUILayout.Button ("Delete", GUILayout.Width (70))) {
                if (deleteAll)
					presetDatabase.DeleteAll();
				else
					presetDatabase.DeletePreset(settingsNames[selected]);
                Close();    
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            if(Event.current.isKey && Event.current.keyCode == KeyCode.Escape)
                Close();

			EditorGUILayout.EndVertical ();
        }

        void OnLostFocus() {
            Close();
        }

		public static void ShowDialog(BrushPresetDatabase presetDatabase)
        {
            Vector2 size = new Vector2(300, 90);

			presetDatabase.Refresh();
            DeleteSettingsDialog window = (DeleteSettingsDialog)EditorWindow.GetWindow (typeof (DeleteSettingsDialog));
			window.presetDatabase = presetDatabase;
            window.minSize = size;
            window.maxSize = size;
            window.position = new Rect((Screen.currentResolution.width-size.x)/2, (Screen.currentResolution.height-size.x)/2, size.x, size.y);
            Utility.SetWindowTitle(window, new GUIContent("Delete Settings", ""));
            window.ShowPopup();
        }
    }

#endregion // Dialogs



} // namespace nTools.PrefabPainter
