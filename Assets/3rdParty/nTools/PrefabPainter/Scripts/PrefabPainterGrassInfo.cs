#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class PrefabPainterGrassInfo : GPUInstancingTag
{
    static Dictionary<int, Dictionary<int, Material>> matSet = new Dictionary<int, Dictionary<int, Material>>();

    const string BASEMAP_PATH =
        "Assets/Res/art/scenes/terrain_bigworld/BigworldData/{0}_{1}/BaseMap/_terrain_{2}_{3}_Basemap_Diffuse.tga";

    void Start()
    {
        if (SceneManager.GetActiveScene().name.ToLower().Contains("bigworld"))
        {
            return;
        }
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);

        foreach (var r in renderers)
        {
            Material sharedMat = r.sharedMaterial;
            if (sharedMat.shader.name == "XingFei/Terrain/GrassBlade" && sharedMat.GetFloat("_UseBasemap") > 0.01f)
            {
                int instanceId = sharedMat.GetInstanceID();
                if (!matSet.ContainsKey(instanceId))
                    matSet.Add(instanceId, new Dictionary<int, Material>());

                Vector3 pos = transform.position;
                int x = (int)(pos.x / 128), z = (int)(pos.z / 128);
                int id = x * 64 + z;
                bool hasId = matSet[instanceId].ContainsKey(id);
                if (hasId && matSet[instanceId][id]) r.material = matSet[instanceId][id];
                else
                {
                    if (hasId && !matSet[instanceId][id])
                        matSet.Clear();
                    Material mat = new Material(r.sharedMaterial);
                    Texture baseMap = AssetDatabase.LoadAssetAtPath<Texture>(string.Format(BASEMAP_PATH, x / 8 + 1, z / 8 + 1, x + 1, z + 1));
                    if (baseMap)
                        mat.SetTexture("_BaseMap", baseMap);
                    mat.SetVector("_IndexForBasemap", new Vector4(x * 128.0f, z * 128.0f, 128.0f, 128.0f));
                    if (!mat.GetTexture("_LightMap"))
                    {
                        mat.SetFloat("_UseLightmapIntensity", 0);
                        mat.DisableKeyword("_USELIGHTMAPINTENSITY_ON");
                    }
                    r.material = mat;
                    matSet[instanceId].Add(id, mat);
                }
            }
        }
    }
}
#endif