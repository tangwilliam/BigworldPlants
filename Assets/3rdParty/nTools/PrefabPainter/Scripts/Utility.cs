
#if (UNITY_EDITOR)

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace nTools.PrefabPainter
{
	public static class XmlUtility
	{
		public static T FromXmlString<T>(string xml) where T : class
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			StringReader stringReader = new StringReader (xml);
			return xmlSerializer.Deserialize (stringReader) as T;
		}

		public static string ToXmlString(object obj)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
			StringWriter stringWriter = new StringWriter();
			xmlSerializer.Serialize (stringWriter, obj);
			return stringWriter.ToString ();
		}
	}
 
    public static class Utility
    {
        public static bool IsVector2Equal (Vector2 a, Vector2 b, float epsilon = 0.001f)
        {
            return Mathf.Abs (a.x - b.x) < epsilon && Mathf.Abs (a.y - b.y) < epsilon;
        }

        public static bool IsVector3Equal (Vector3 a, Vector3 b, float epsilon = 0.001f)
        {            
            return Mathf.Abs (a.x - b.x) < epsilon && Mathf.Abs (a.y - b.y) < epsilon && Mathf.Abs (a.z - b.z) < epsilon;
        }

        public static Vector3 RoundVector(Vector3 v, int digits)
        {
            return new Vector3((float)Math.Round(v.x, digits), (float)Math.Round(v.y, digits), (float)Math.Round(v.z, digits));
        }

        public static float Round(float v, int digits)
        {
            return (float)Math.Round(v, digits);
        }

        public static string TruncateString(string str, GUIStyle style, int maxWidth)
        {
            GUIContent ellipsis = new GUIContent("...");
            string shortStr = "";

            float ellipsisSize = style.CalcSize(ellipsis).x;
            GUIContent textContent = new GUIContent("");

            char[] charArray = str.ToCharArray();
            for(int i = 0; i < charArray.Length; i++)
            {
                textContent.text += charArray[i];

                float size = style.CalcSize(textContent).x;

                if (size > maxWidth - ellipsisSize)
                {
                    shortStr += ellipsis.text;
                    break;
                }

                shortStr += charArray[i];
            }

            return shortStr;
        }


        public static void MarkActiveSceneDirty()
        {
#if (UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
            EditorApplication.MarkSceneDirty ();
#else       
            UnityEngine.SceneManagement.Scene activeScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(activeScene);
#endif
        }


        public static void ForAllInHierarchy(GameObject gameObject, Action<GameObject> action)
        {
            action(gameObject);

            for (int i = 0; i < gameObject.transform.childCount; i++)
                ForAllInHierarchy(gameObject.transform.GetChild(i).gameObject, action);
        }


        public static void SetWindowTitle(EditorWindow window, GUIContent titleContent)
        {
#if (UNITY_5_0)
            window.title = titleContent.text;
#else
            window.titleContent = titleContent;
#endif
        }

    } // class Utility


    static public class DebugCounter
    {
        static Dictionary<string, int> m_counters = new Dictionary<string, int>(CustomOrdinalStringComparer.GetComparer());

        static public void Increase(string name)
        {
            if(!m_counters.ContainsKey(name))
            {
                m_counters.Add(name, 1);
                return;
            }

            m_counters[name]++;
        }
        static public void Reset(string name)
        {
            if(!m_counters.ContainsKey(name))
            {
                m_counters.Add(name, 0);
                return;
            }

            m_counters[name] = 0;
        }
        static public void LogAndReset(string name)
        {
            if(!m_counters.ContainsKey(name))
                m_counters.Add(name, 0);

            DebugL8.Log("{0}: {1}", name , m_counters[name]);
            m_counters[name] = 0;
        }
    }

} // namespace 

#endif // (UNITY_EDITOR)

