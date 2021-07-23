using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Will
{

    public class GPUCulling : MonoBehaviour
    {
        public bool DrawIndirect = true;

        public GPUDataObject[] m_GPUDataObjects;

        public ComputeShader m_ComputeShader;
        public Mesh m_Mesh;
        public int m_SubMeshIndex = 0;
        public Material m_DrawMat;

        private ComputeBuffer m_BufferWithArgs;
        private ComputeBuffer m_PosBuffer;
        private uint[] m_Args = new uint[5] { 0, 0, 0, 0, 0 }; // The GPU buffer containing the arguments for how many instances of this mesh to draw.
        private int m_CSCullingID;
        private Bounds m_Bounds = new Bounds(Vector3.zero, Vector3.one * 100000);
        private int m_CountsSqrted;


        void Start()
        {
            m_Args = new uint[] { m_Mesh.GetIndexCount(0), 0, 0, 0, 0 };
            m_BufferWithArgs = new ComputeBuffer(5, sizeof(uint), ComputeBufferType.IndirectArguments);
            m_BufferWithArgs.SetData(m_Args);

            //Vector3[] positions = ReadData(m_GPUDataObjects);
            GPUItem[] positions = ReadData(m_GPUDataObjects);
            int instanceCount = positions.Length;
            m_CountsSqrted = Mathf.CeilToInt(Mathf.Sqrt(instanceCount));

            // 需要绘制的个数，可以用instanceCount 进行粗放地指定，也可以参考知乎贴 https://zhuanlan.zhihu.com/p/278793984 的方式在ComputeShader中用 InterlockedAdd()进行自增得到。
            // 两个方案性能对比暂未测试。
            //m_Args[1] = (uint)instanceCount;
            //m_BufferWithArgs.SetData(m_Args);

            //m_PosBuffer = new ComputeBuffer(instanceCount, 3 * 4);
            m_PosBuffer = new ComputeBuffer(instanceCount, (4 * 4 + 3) * 4);
            m_PosBuffer.SetData(positions);

            //var posVisibleBuffer = new ComputeBuffer(instanceCount, 3 * 4);
            var posVisibleBuffer = new ComputeBuffer(instanceCount, (4 * 4 + 3) * 4);

            m_CSCullingID = m_ComputeShader.FindKernel("CSCulling");
            m_ComputeShader.SetBuffer(m_CSCullingID, "bufferWithArgs", m_BufferWithArgs);
            m_ComputeShader.SetBuffer(m_CSCullingID, "posAllBuffer", m_PosBuffer);
            m_ComputeShader.SetBuffer(m_CSCullingID, "posVisibleBuffer", posVisibleBuffer);

            m_DrawMat.SetBuffer("posVisibleBuffer", posVisibleBuffer);


        }


        void Update()
        {

            if (DrawIndirect)
            {
                DoCulling();
                Graphics.DrawMeshInstancedIndirect(m_Mesh, m_SubMeshIndex, m_DrawMat, m_Bounds, m_BufferWithArgs, 0, null, ShadowCastingMode.Off, false);
            }

            
        }

        void DoCulling()
        {
            m_Args[1] = 0; // Compute Shader 中用InterlockedAdd() 进行自增。得到实际要绘制的个数。
            m_BufferWithArgs.SetData(m_Args);

            m_ComputeShader.SetVector("cmrPos", Camera.main.transform.position);
            m_ComputeShader.SetVector("cmrDir", Camera.main.transform.forward);
            m_ComputeShader.SetFloat("cmrHalfFov", Camera.main.fieldOfView / 2);
            Matrix4x4 ProjectionMatrixInShader = GL.GetGPUProjectionMatrix(Camera.main.projectionMatrix, false); // C sharp 中Unity都使用OpenGL那套矩阵变换。但是Shader中则根据平台不同有不同的投影矩阵。于是需要将它转化为Shader中实际的矩阵
            var m = ProjectionMatrixInShader * Camera.main.worldToCameraMatrix;
            m_ComputeShader.SetMatrix("matrix_VP", m);
            m_ComputeShader.SetInt("countPerDimention", m_CountsSqrted);
            
            float threadsPerDimentionPerGroup = 8f; // 二维线程组中每一维的线程个数。使用float 是为了使用 CeilToInt()
            int dispatchCountPerDimention = Mathf.CeilToInt( m_CountsSqrted / threadsPerDimentionPerGroup);
            m_ComputeShader.Dispatch(m_CSCullingID, dispatchCountPerDimention, dispatchCountPerDimention, 1);


        }

        //Vector3[] ReadData(GPUDataObject[] dataOBjects)
        GPUItem[] ReadData(GPUDataObject[] dataOBjects)
        {

            int instanceCount = 0;
            for (int i = 0; i < dataOBjects.Length; i++)
            {
                instanceCount += dataOBjects[i].GPUItems.Count;
                Debug.Log("data count : " + dataOBjects[i].GPUItems.Count);
            }

            //Vector3[] positions = new Vector3[instanceCount];
            GPUItem[] positions = new GPUItem[instanceCount];

            int indexBias = 0;
            for(int i = 0; i < dataOBjects.Length; i++)
            {
                indexBias += (i >= 1 ? dataOBjects[i - 1].GPUItems.Count : 0);
                int itemCount = dataOBjects[i].GPUItems.Count;
                for (int j = 0; j < itemCount; j++)
                {
                    //positions[indexBias + j] = dataOBjects[i].GPUItems[j].Position;
                    positions[indexBias + j] = dataOBjects[i].GPUItems[j];
                }
            }

            Debug.Log("total data count : " + positions.Length);
            return positions;
        }



    }
}

