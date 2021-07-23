using System.Collections.Generic;
using UnityEngine;

namespace Assets.CommonLibs.Utility
{
    public interface IPoolItem
    {
        void OnInit();
        void OnRelease();
    }

    public class CommonPool<T> where T : IPoolItem, new()
    {
        Queue<T> m_pool = new Queue<T>();
        public T Get()
        {
            T r;
            if (m_pool.Count < 1)
            {
                r = new T();
            }
            else
            {
                r = m_pool.Dequeue();
                r.OnInit();
            }
            return r;
        }

        public void Release(T val)
        {
            if(m_pool.Count > 0 && ReferenceEquals(m_pool.Peek(), val))
            {
                //重复回收
                DebugL8.LogError("release same item twice");
            }
            val.OnRelease();
            m_pool.Enqueue(val);
        }
    }
}
