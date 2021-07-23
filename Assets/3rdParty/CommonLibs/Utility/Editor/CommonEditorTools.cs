using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Assets.CommonLibs.Utility;


namespace CommonEditorTools
{

    public static class CommonUtility
    {
#if UNITY_EDITOR
        public static Vector3? Div(this Vector3 a, Vector3 b)
        {
            if(b.x == 0 || b.y ==0 || b.z == 0)
            {
                return null;
            }
            return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
        }

        public static float GetLodFactor(GameObject go)
        {
            if(null == go)
            {
                return 0.1f;
            }
            var lodGroup = go.GetComponentInChildren<LODGroup>();
            if (null == lodGroup)
            {
                return 0.1f;
            }
            var lods = lodGroup.GetLODs();
            if (lods.Length < 2)
            {
                return 0.1f;
            }
            return lods[lods.Length - 2].screenRelativeTransitionHeight;
        }

        public static float ComputeLodDrawDistance(float lodFactor, float bounds)
        {
            //这里不用lod2来算了，不然问题太多了
            //改成如果lod2大于一定值，就使用占屏比小于10（暂定）来加载和裁剪
            //如果小于10，那么就按一定比例处理
            var fov = 40;
            //将屏幕占满时的距离
            var normalizeDistance = GetNormalizedDistance(bounds, fov);
            //占比为0.1的距离
            lodFactor = Mathf.Max(lodFactor, 0.1f);
            var drawDistance = normalizeDistance / lodFactor;
            return drawDistance;
        }

        public static float GetNormalizedDistance(float bounds, float fov)
        {
            var normalizeDistance = bounds / Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);
            return normalizeDistance;
        }

        public static float GetSizeOfDistance(float distance, float fov)
        {
            return distance * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);
        }

        public static string GetABAssetNameByPath(string path)
        {
            return path;
        }

        #region 贴图标准

        /// <summary>
        /// 判断是否为图片
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsTexture(string path)
        {
            path = path.ToLower();
            return path.EndsWith(".png") || path.EndsWith(".tga") || path.EndsWith(".hdr") || path.EndsWith(".exr") || path.EndsWith(".psd") || path.EndsWith(".jpg");
        }

        /// <summary>
        /// 判断是否包含指定后缀
        /// d=diffuse, n=normal, e=emissive, s/m=pbr
        /// </summary>
        public static bool HasSuffix(string path, string suffix)
        {
            path = Path.GetFileNameWithoutExtension(path.ToLower());
            return path.Contains(suffix + "_") || path.EndsWith(suffix);
        }

        /// <summary>
        /// 判断最后一个后缀
        /// 角色贴图的类型(d-颜色,n-法线,s-pbr,e-自发光,lod)
        /// 角色贴图标准：分为五类，每张图必须包含类型后缀，且以类型后缀结尾
        /// 因为角色内部还定义了"e-装备"类似的定义，需要取最后的后缀，以最后一个有效后缀为准
        /// </summary>
        public static bool HasLastSuffix(string path, string suffix)
        {
            return GetLastSuffix(path) == suffix;
        }

        /// <summary>
        /// 获取最后一个后缀
        /// </summary>
        public static string GetLastSuffix(string path)
        {
            path = Path.GetFileNameWithoutExtension(path.ToLower());
            int idx = path.LastIndexOf("_");
            if (idx >= 0)
                return path.Substring(idx);
            else
                return string.Empty;
        }

        #endregion

        /// <summary>
        /// 判断是否是模型
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsModel(string path)
        {
            path = path.ToLower();
            return path.EndsWith(".fbx") || path.EndsWith(".obj");
        }

        /// <summary>
        /// 取消Mipmap勾选
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public static void UncheckMipMap(UnityEngine.Object obj)
        {
            var texture = obj as Texture;
            if (null == texture)
            {
                return;
            }
            var path = AssetDatabase.GetAssetPath(texture);
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (null == importer)
            {
                Debug.LogErrorFormat("Importer for {0} is null", path);
                return;
            }
            UncheckMipMap(importer);
        }

        public static void UncheckMipMap(TextureImporter texImporter)
        {
            if (null == texImporter)
            {
                return;
            }
            texImporter.mipmapEnabled = false;
        }

        public static AssetImporter GetAssetImporter(UnityEngine.Object obj)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            var importer = AssetImporter.GetAtPath(path);
            if (null == importer)
            {
                Debug.LogErrorFormat("Importer for {0} is null", path);
            }
            return importer;
        }

        public static void SetTextureCompression(TextureImporter importer, TextureImporterCompression importerCompression)
        {
            if (null == importer)
            {
                return;
            }
            importer.textureCompression = importerCompression;
        }


        [MenuItem("Assets/拷贝路径")]
        private static void CopyPath()
        {
            GUIUtility.systemCopyBuffer = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
        }

        /// <summary>
        /// 获取相对Resources文件夹的路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ConvertToResourceRelativePath(this string path)
        {
            var resourceStr = "/Resources/";
            var startIdx = path.IndexOf(resourceStr) + resourceStr.Length;
            var relative = path.Substring(startIdx, path.Length - startIdx);
            relative = relative.Replace(".prefab", "");
            return relative;
        }

        /// <summary>
        /// 递归遍历文件夹内的资源
        /// </summary>
        /// <param name="path"></param>
        /// <param name="processAction"></param>
        /// <param name="filter"></param>
        private static void LoopAllUnityObjectsInFolderRecursive(string path, Action<UnityEngine.Object> processAction, System.Func<string, bool> filter)
        {
            var directories = Directory.GetDirectories(path);
            foreach (var directory in directories)
            {
                LoopAllUnityObjectsInFolderRecursive(directory, processAction, filter);
            }
            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                if (null != filter && !filter(file))
                {
                    continue;
                }
                if (!file.EndsWith(".meta"))
                {
                    var obj = AssetDatabase.LoadMainAssetAtPath(file.ToRelativePath());
                    if (null != processAction)
                    {
                        processAction(obj);
                    }
                }
            }
        }

        /// <summary>
        /// 遍历文件夹内的资源
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <param name="processAction"></param>
        /// <param name="filter"></param>
        public static bool LoopAllUnityObjectsInFolder(string relativePath, Action<UnityEngine.Object> processAction, System.Func<string, bool> filter = null, bool showTip = false)
        {
            if (!IsFolder(relativePath))
            {
                if (showTip)
                {
                    EditorUtility.DisplayDialog("Error", string.Format("路径{0}不合法，请选择文件夹进行操作", relativePath), "确定");
                }
                return false;
            }
            else
            {
                if (!showTip || EditorUtility.DisplayDialog("Tip", string.Format("即将对{0}进行操作", relativePath), "确定", "取消"))
                {
                    var dir = relativePath.ToAbsolatePath();
                    if (!Directory.Exists(dir))
                    {
                        return false;
                    }
                    //var directories = Directory.GetDirectories(dir);
                    LoopAllUnityObjectsInFolderRecursive(relativePath.ToAbsolatePath(), processAction, filter);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 遍历当前选择的文件夹内的资源
        /// </summary>
        /// <param name="processAction"></param>
        /// <param name="filter"></param>
        public static void LoopAllUnityObjectsInSelectionFloder(Action<UnityEngine.Object> processAction, Func<string, bool> filter = null)
        {
            var instanceId = Selection.activeInstanceID;
            var path = AssetDatabase.GetAssetPath(instanceId);
            LoopAllUnityObjectsInFolder(path, processAction, filter);
        }

        /// <summary>
        /// 判断对象是否是Prefab
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsPrefab(UnityEngine.Object obj)
        {
            if(null == obj)
            {
                return false;
            }
            var prefabType = PrefabUtility.GetPrefabType(obj);
            return prefabType != PrefabType.None;
        }

        /// <summary>
        /// 判断路径是否是合法的Folder
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsFolder(string path)
        {
            var type = AssetDatabase.GetMainAssetTypeAtPath(path);
            if (type != typeof(DefaultAsset))
            {
                return false;
            }
            return Directory.Exists(ToAbsolatePath(path));
        }

        /// <summary>
        /// 相对路径转换为绝对路径
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public static string ToAbsolatePath(this string relativePath)
        {
            var assetLength = "Assets".Length;
            var absolatePath = Application.dataPath + relativePath.Substring(assetLength, relativePath.Length - assetLength).Replace("\\", "/");
            //Debug.LogFormat("{0} to absolate {1}", relativePath, absolatePath);
            return absolatePath;
        }

        /// <summary>
        /// 绝对路径转换为相对路径
        /// </summary>
        /// <param name="absolatePath"></param>
        /// <returns></returns>
        public static string ToRelativePath(this string absolatePath)
        {
            var relativePath = absolatePath.Replace(Application.dataPath, "Assets").Replace("\\", "/");
            //Debug.LogFormat("{0} to relative {1}", absolatePath, relativePath);
            return relativePath;
        }

        public static void LoopAllFilesWithDirFilterRecursive(string dirAbsolatePath, ICollection<string> allFiles, Func<string, bool> dirFilter, Func<string, bool> fileFilter)
        {
            if (null == allFiles)
            {
                return;
            }
            if (null == dirFilter || dirFilter(dirAbsolatePath))
            {
                var files = Directory.GetFiles(dirAbsolatePath);
                foreach (var file in files)
                {
                    if (null == fileFilter || fileFilter(file))
                    {
                        if(allFiles.Contains(file))
                        {
                            Debug.LogWarningFormat("{0} dupliced", file);
                        }
                        allFiles.Add(file.Replace("\\", "/"));
                    }
                }
            }
            var dirs = Directory.GetDirectories(dirAbsolatePath);
            foreach (var dir in dirs)
            {
                LoopAllFilesWithDirFilterRecursive(dir, allFiles, dirFilter, fileFilter);
            }
        }

        /// <summary>
        /// 条件返回True则终止向下递归
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="func"></param>
        public static void LoopChildDownSide(Transform parent, Func<Transform, bool> func)
        {
            foreach (Transform child in parent)
            {
                if (func(child))
                {
                    continue;
                }
                LoopChildDownSide(child, func);
            }
        }

        /// <summary>
        /// 获取对象的大小, 大小为aabb盒xzy坐标中的最大值
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="pos"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static bool GetRenderSize(GameObject gameObject, out float size)
        {
            size = 0;
            var maxHeight = float.MinValue;
            var minHeight = float.MaxValue;
            var maxWidth = float.MinValue;
            var minWidth = float.MaxValue;
            var maxLength = float.MinValue;
            var minLength = float.MaxValue;
            var renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
            var hasLegalRenderer = false;
            foreach(var renderer in renderers)
            {
                if (null != renderer && renderer.enabled)
                {
                    maxHeight = Mathf.Max(maxHeight, renderer.bounds.max.y);
                    maxWidth = Mathf.Max(maxWidth, renderer.bounds.max.z);
                    maxLength = Mathf.Max(maxLength, renderer.bounds.max.x);

                    minHeight= Mathf.Min(minHeight, renderer.bounds.min.y);
                    minWidth = Mathf.Min(minWidth, renderer.bounds.min.z);
                    minLength = Mathf.Min(minLength, renderer.bounds.min.x);
                    hasLegalRenderer = true;
                }
            }
            size = new Vector3(maxLength - minLength, maxHeight - minHeight, maxWidth - minWidth).magnitude;
            return hasLegalRenderer;
        }

        public static bool MulOf4(int a)
        {
            return a % 4 == 0;
        }

        public static void RemoveAllComponents(GameObject go)
        {
            var components = go.GetComponents<Component>();
            foreach(var comp in components)
            {
                if(comp.GetType() == typeof(Transform))
                {
                    continue;
                }
                UnityEngine.Object.DestroyImmediate(comp);
            }
        }

        public static void CheckAndCreateDirectory(string filePath)
        {
            var folderPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        /// <summary>
        /// 更改Lod的显示
        /// </summary>
        /// <param name="color"></param>
        public static void ColorLOD()
        {
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            var roots = scene.GetRootGameObjects();
            foreach (var root in roots)
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
                            if(null == renderer)
                            {
                                continue;
                            }
                            var mats = renderer.materials;
                            foreach (var mat in mats)
                            {
                                if(null == mat)
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
                                        mat.SetColor("_Color", Color.blue);
                                        break;
                                    case 2:
                                        //绿色不好区分
                                        mat.SetColor("_Color", Color.yellow);
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
        }

#endif

        #region BundleName转换
        public static string s_reourceRootAbsolute = "Assets/Res/";
        public static string s_packedTextureRootPath = "Assets/Res/ui/texture/";
        private static int s_packedTextureRootPathLength = s_packedTextureRootPath.Length;
        private static Dictionary<string, string> s_path2ABNameDict = new Dictionary<string, string>(4096 * 10, CustomOrdinalStringComparer.GetComparer());
        public static string GetBundleName(string inputPath)
        {
            string abName;
            if (s_path2ABNameDict.TryGetValue(inputPath, out abName))
            {
                return abName;
            }
            abName = inputPath;
            //说明不是从assets/目录开始写的，需要拼接目录前缀
            if (abName.IndexOf("Assets/") != 0)
                abName = s_reourceRootAbsolute + abName;

            //说明是要加载打成了图集的图
            if (abName.IndexOf(s_packedTextureRootPath) == 0)
            {
                int idx = abName.IndexOf('/', s_packedTextureRootPathLength);
                abName = GetAtlasABName(abName.Substring(s_packedTextureRootPathLength, idx - s_packedTextureRootPathLength));
                s_path2ABNameDict.Add(inputPath, abName);
                return abName;
            }
#if UNITY_EDITOR // 这里只用于editor下收集abname的时候使用
            else if (abName.IndexOf("Assets/Res/art/scenes/scenes/") == 0 && abName.EndsWith(".unity"))
            {
                return Path.GetFileNameWithoutExtension(abName);
            }
#endif
            else
            {
                abName = MD5FilesGenerator.GetMd5Hash(abName);
                s_path2ABNameDict.Add(inputPath, abName);
                return abName;
            }
        }

        public static string GetAssetName(string path)
        {
            if (path.IndexOf("Assets/") == 0)
                return path;
            else
                return s_reourceRootAbsolute + path;
        }

        public static string GetAtlasABName(string folderName)
        {
            return "atlas_" + folderName;
        }
        #endregion

        public static void LoopAllFiles(string path, List<string> files)
        {
            var curFiles = Directory.GetFiles(path);
            files.AddRange(curFiles);
            var dirs = Directory.GetDirectories(path);
            foreach (var dir in dirs)
            {
                LoopAllFiles(dir, files);
            }
        }

        public static bool IsValidAsset(string fileName)
        {
            var invalid = fileName.EndsWith(".exe")
                || fileName.EndsWith(".txt")
                || fileName.EndsWith(".bat")
                || fileName.EndsWith(".dll");
            return !invalid;
        }

        public static void CheckAndCreateDirectory(string filePath, bool remove = false)
        {
            var folderPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            else
            {
                if (remove)
                {
                    Directory.Delete(folderPath, true);
                    Directory.CreateDirectory(folderPath);
                }
            }
        }
    }
}