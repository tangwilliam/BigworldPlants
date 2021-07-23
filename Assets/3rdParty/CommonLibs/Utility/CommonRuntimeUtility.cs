using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Assets.CommonLibs.Utility
{
    public static class CommonRuntimeUtility
    {
        public static bool m_isLodColored;
        /// <summary>
        /// 更改Lod的显示
        /// </summary>
        /// <param name="color"></param>
        public static void ColorLOD()
        {
            m_isLodColored = true;
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            var roots = scene.GetRootGameObjects();
            foreach (var root in roots)
            {
                ColorLOD(root);
            }
        }

        public static void ColorLOD(GameObject root)
        {
            var lodgroups = root.GetComponentsInChildren<LODGroup>();
            foreach (var lodgroup in lodgroups)
            {
                var lods = lodgroup.GetLODs();
                for (int i = 0; i < lods.Length; i++)
                {
                    var lod = lods[i];
                    var renderers = lod.renderers;
                    foreach (var renderer in renderers)
                    {
                        if (null == renderer)
                        {
                            continue;
                        }
                        var mats = renderer.materials;
                        foreach (var mat in mats)
                        {
                            if (null == mat)
                            {
                                continue;
                            }
                            //mat.shader = Shader.Find("XingFei/Color");
                            switch (i)
                            {
                                case 0:
                                    mat.SetColor("_Color", Color.red);
                                    break;
                                case 1:
                                    mat.SetColor("_Color", Color.yellow);
                                    break;
                                case 2:
                                    mat.SetColor("_Color", Color.green);
                                    break;
                                default:
                                    mat.SetColor("_Color", Color.gray);
                                    break;
                            }
                        }
                    }
                }
            }
        }


        public static bool IsQuadOverlap(Vector3[] cornorsA, Vector3[] cornorsB)
        {
            if(cornorsA[2].x < cornorsB[0].x)
            {
                return false;
            }
            if(cornorsA[0].x > cornorsB[2].x)
            {
                return false;
            }
            if(cornorsA[2].y < cornorsB[0].y)
            {
                return false;
            }
            if(cornorsA[0].y > cornorsB[2].y)
            {
                return false;
            }
            return true;
        }

        public static void DebugLine(Vector3 start, Vector3 end,string name)
        {
            DebugL8.Log("DebugLine {2} {0} to {1}", start, end, name);
            var line = new GameObject(name);
            var mesh = new Mesh();
            mesh.SetVertices(new List<Vector3> { start, end });
            mesh.SetIndices(new int[] { 0, 1 }, MeshTopology.Lines, 0);
            var mf = line.AddComponent<MeshFilter>();
            mf.mesh = mesh;
            var mr = line.AddComponent<MeshRenderer>();
            mr.sharedMaterial = new Material(Shader.Find("Mobile/Diffuse"));
        }

        public static void DebugPoint(Vector3 pos, string name)
        {
            var point = new GameObject(name);
            var mesh = new Mesh();
            mesh.SetVertices(new List<Vector3> { pos });
            mesh.SetIndices(new int[] { 0 }, MeshTopology.Points, 0);
            var mf = point.AddComponent<MeshFilter>();
            mf.mesh = mesh;
            var mr = point.AddComponent<MeshRenderer>();
            mr.sharedMaterial = new Material(Shader.Find("Mobile/Diffuse"));
        }

        public static float GetScale(Vector3 lastStart, Vector3 lastEnd, Vector3 newStart, Vector3 newEnd)
        {
            var startDis = Vector3.Distance(lastEnd, lastStart);
            var endDis = Vector3.Distance(newEnd, newStart);
            return endDis / startDis; 
        }

        public static Texture2D GetCellColoredTexture(int texWidth, int texHeight, int totalWidth, int totalHeight, int cellWidth, int cellHeight)
        {
            var startTime = Time.time;
            var tex = new Texture2D(texWidth, texHeight);
            var xCount = totalWidth / cellWidth;
            var yCount = totalHeight / cellHeight;
            var cols = new Color[texWidth * texHeight];
            var maxThreacCount = 4;
            var flags = new bool[maxThreacCount];
            for(var i = 0; i < flags.Length; i++)
            {
                flags[i] = false;
            }
            for(var threadCount = 0; threadCount < maxThreacCount; threadCount ++)
            {
                ThreadPool.QueueUserWorkItem((obj) =>
                {
                    var index = threadCount;
                    for (int i = index * yCount / maxThreacCount; i < yCount / yCount / index ; i++)
                    {
                        for (int j = 0; j < xCount; i++)
                        {
                            cols[i * texWidth + j] = new Color(i % 2 == 0 ? 0 : 1, j % 2 == 0 ? 1 : 0, 1, 0.5f);
                        }
                    }
                    flags[index] = true;
                });
            }
            while(true)
            {
                bool finish = true;
                for(var i = 0; i < flags.Length; i++)
                {
                    if(!flags[i])
                    {
                        finish = false;
                    }
                }
                if(finish)
                {
                    break;
                }
            }
            tex.SetPixels(cols);
            tex.Apply();
            return tex;
        }

        public static int GetCollisionLayers(int srcLayer)
        {
            var mask = 0;
            for(int i = 0; i < 32; i++)
            {
                if (string.IsNullOrEmpty(LayerMask.LayerToName(i)))
                {
                    //is not a valid layer
                    continue;
                }
                if(!Physics.GetIgnoreLayerCollision(srcLayer, i))
                {
                    DebugL8.Log("collide with {0}", LayerMask.LayerToName(i));
                    mask |= 1 << i; 
                }
            }
            return mask;
        }

        public static void MaskToLayers(int mask, List<int> layers)
        {
            var remain = mask > 0 ? 1 : -1;
            for (int i = 0; i < 32; i++)
            {
                if((mask >> i) % 2 == remain)
                {
                    layers.Add(i);
                }
            }
        }
    }
    
    public static class KDTreeUtility
    {
        public static T FindMid<T>(List<T> list) where T : IComparable
        {
            return FindMid(list, 0, list.Count);
        }

        public static T FindMid<T>(List<T> list, Func<T, T, int> comparer)
        {
            return FindMid(list, 0, list.Count, comparer);
        }

        public static T FindMid<T>(List<T> list, int start, int end) where T : IComparable
        {
            var mid = SortOnce(list, start, end, (start + end) / 2);
            return mid;
        }

        public static T FindMid<T>(List<T> list, int start, int end, Func<T,T, int> comparer)
        {
            var mid = SortOnce(list, start, end, (start + end) / 2, comparer);
            return mid;
        }

        private static void Print<T>(List<T> list)
        {
            var output = "";
            foreach (var f in list)
            {
                output += f + ",";
            }
            DebugL8.Log(output);
        }

        private static T SortOnce<T>(List<T> list, int start, int end, int midIndex, Func<T, T, int> comparer)
        {
            Exchange(list, midIndex, start);
            var mid = list[start];
            int i = start + 1;

            for (; i < end; i++)
            {
                var cur = list[i];
                if (comparer(cur, mid) >= 0)
                {
                    var change = false;
                    for (int j = i + 1; j < end; j++)
                    {
                        if (comparer(list[j], mid) < 0)
                        {
                            Exchange(list, i, j);
                            change = true;
                            break;
                        }
                    }
                    if (!change)
                    {
                        break;
                    }
                }
            }

            var midIdx = i - 1;
            Exchange(list, start, midIdx);
            if (midIdx == midIndex)
            {
                return list[midIndex];
            }
            if (midIdx < midIndex)
            {
                return SortOnce(list, midIdx + 1, end, midIndex, comparer);
            }
            else
            {
                return SortOnce(list, start, midIdx, midIndex, comparer);
            }
        }

 
        private static T SortOnce<T>(List<T> list, int start, int end, int midIndex) where T : IComparable
        {
            var mid = list[start];
            int i = start + 1;

            for (; i < end; i++)
            {
                var cur = list[i];
                if (cur.CompareTo(mid) >= 0)
                {
                    var change = false;
                    for (int j = i + 1; j < end; j++)
                    {
                        if (list[j].CompareTo(mid) < 0)
                        {
                            Exchange(list, i, j);
                            change = true;
                            break;
                        }
                    }
                    if (!change)
                    {
                        break;
                    }
                }
            }

            var midIdx = i - 1;
            Exchange(list, start, midIdx);
            if (midIdx == midIndex)
            {
                return list[midIndex];
            }
            if (midIdx < midIndex)
            {
                return SortOnce(list, midIdx + 1, end, midIndex);
            }
            else
            {
                return SortOnce(list, start, midIdx, midIndex);
            }
        }

        public static void Exchange<T>(List<T> list, int x, int y)
        {
            var tmp = list[x];
            list[x] = list[y];
            list[y] = tmp;
        }
    }
}
