using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Will
{
    public class GameController : MonoBehaviour
    {

        public GPUDataObject[] m_GPUDataObjects;
        public ComputeShader m_ComputeShader;
        public Mesh m_Mesh;
        public int m_SubMeshIndex = 0;
        public Material m_DrawMat;
        public Texture2D[] m_BaseMaps;
        public Vector4[] m_IndexForBaseMaps;

        private List<GPUInstancingObj> m_GPUInstancingObjs;

        //-------------------------------------------------
        // Singleton
        public static GameController instance = null;
        void Awake()
        {
            if (instance != null && instance != this)
                Destroy(gameObject);    // Ensures that there aren't multiple Singletons

            instance = this;
        }

        // Singleton
        //-------------------------------------------------

        void Start()
        {
            m_GPUInstancingObjs = new List<GPUInstancingObj>(m_GPUDataObjects.Length);
            for(int i = 0; i < m_GPUDataObjects.Length; i++)
            {
                m_GPUInstancingObjs.Add(new GPUInstancingObj(m_ComputeShader,i, m_Mesh, m_SubMeshIndex ,m_DrawMat, m_BaseMaps[i], m_IndexForBaseMaps[i], m_GPUDataObjects[i]));
            }
        }

        void Update()
        {
            for(int i = 0; i < m_GPUInstancingObjs.Count; i++)
            {
                m_GPUInstancingObjs[i].Render();
            }
        }

        private void OnDestroy()
        {
            for (int i = 0; i < m_GPUInstancingObjs.Count; i++)
            {
                m_GPUInstancingObjs[i].Dispose();
            }
        }
    }
}


