using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 延时和重复都受TimeScale的影响,线程不安全
/// </summary>
public class UpdateManager : MonoBehaviour
{
    //游戏程序是否退出了
    private static bool m_IsApplicationQuitting = false;
    private static object m_Lock = new object();

    private static UpdateManager m_instance;
    private static void CreateInstance()
    {
        //这里应该是App要关闭了，不需要再重新创建一个
        if (m_IsApplicationQuitting) return;
        lock (m_Lock)
        {
            if (null == m_instance)
            {
                var go = new GameObject(typeof(UpdateManager).ToString());
                DontDestroyOnLoad(go);
                m_instance = go.AddComponent<UpdateManager>();
            }
        }
    }

    private static LinkedList<Action> m_updateFuncList = new LinkedList<Action>();

    public static void RegisterUpdate(Action update)
    {
        if (m_instance == null) CreateInstance();
        m_updateFuncList.AddLast(update);
    }

    public static void UnregisterUpdate(Action update)
    {
        m_updateFuncList.Remove(update);
    }

    LinkedListNode<Action> m_CurNode;
    private void Update()
    {
        m_CurNode = m_updateFuncList.First;
        while (m_CurNode != null)
        {
            m_CurNode?.Value();
            m_CurNode = m_CurNode.Next;
        }
    }

    private void OnDestroy()
    {
        m_IsApplicationQuitting = false;
    }
}
