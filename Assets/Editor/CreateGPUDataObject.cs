using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Will
{
    public class CreateGPUDataObject
    {
        public static int TotalChunkCount = 9;
        public static int ChunkRowCount = 3;
        public static float ChunkSize = 128f;

        public static string GrassRoot = "mesh_dsj_grass_lod0_group";
        public static uint GrassId = 0;
        public static string TerrainRoot = "Chunks";

        [MenuItem("Assets/Create/GPUData")]
        public static void CreateMyAsset()
        {
            GameObject[] plantGOs = CollectDataInScene(GrassRoot);

            GameObject[] terrainGOs = CollectDataInScene(TerrainRoot);
            List<List<GPUItem>> GPUItems = new List<List<GPUItem>>(terrainGOs.Length);
            TerrainBounds[] terrainBounds = new TerrainBounds[terrainGOs.Length];
            for(int i = 0; i < terrainGOs.Length; i++)
            {
                Vector3 pos = terrainGOs[i].transform.position;
                Vector3 maxPos = pos + new Vector3(ChunkSize, 0, ChunkSize);
                terrainBounds[i] = new TerrainBounds(new Vector2(pos.x, pos.z), new Vector2(maxPos.x, maxPos.z));
                GPUItems.Add(new List<GPUItem>());
            }

            


            for (int i = 0; i < plantGOs.Length; i++)
            {
                //Debug.Log(plantGOs[i].name);

                Transform trans = plantGOs[i].transform;
                
                // 将草都分别放入指定的地块
                for (int j = 0; j < terrainBounds.Length; j++)
                {
                    if(terrainBounds[j].IsInBounds(trans.position))
                    {
                        GPUItem gpuItem = new GPUItem(plantGOs[i].transform.localToWorldMatrix, plantGOs[i].transform.position, GrassId);
                        GPUItems[j].Add(gpuItem);
                        continue;
                    }
                }

            }


            for(int i = 0; i < terrainGOs.Length; i++)
            {
                GPUDataObject asset = ScriptableObject.CreateInstance<GPUDataObject>();
                asset.GPUItems = GPUItems[i];

                string name = terrainGOs[i].name;
                AssetDatabase.CreateAsset(asset, "Assets/GPUDatas/GPUData_" + name + ".asset");
                AssetDatabase.SaveAssets();

                Debug.Log(name + " has GPUitems count : " + asset.GPUItems.Count);
            }


            EditorUtility.FocusProjectWindow();
            //Selection.activeObject = asset;

        }


        public static GameObject[] CollectDataInScene(string rootName)
        {
            GameObject rootGO = GameObject.Find(rootName);
            int childrenCount = rootGO.transform.childCount;

            GameObject[] children = new GameObject[childrenCount];
            for (int i = 0; i < childrenCount; ++i)
            {
                children[i] = rootGO.transform.GetChild(i).gameObject;
            }

            Debug.Log(rootName + " has children gameObject: " + children.Length);

            return children;
        }
    }

}

