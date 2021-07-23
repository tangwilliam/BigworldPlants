using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Will;

public class GPUCullingSample : MonoBehaviour
{
    public GPUDataObject m_GPUDataObject;
    private int m_InstanceCount;
    private int m_CountsSqrted;

    public ComputeShader m_ComputeShader;
    public Mesh m_Mesh;
    public Material m_DrawMat;
    ComputeBuffer m_BufferWithArgs;
    private uint[] m_Args;
    private int m_CSCullingID;
    private Bounds m_Bounds = new Bounds(Vector3.zero, Vector3.one * 100000);
    ComputeBuffer m_PosBuffer;
    
    void Start()
    {
        // Get data
        m_InstanceCount = m_GPUDataObject.GPUItems.Count;
        m_CountsSqrted = Mathf.CeilToInt(Mathf.Sqrt(m_InstanceCount));
        Debug.Log(m_GPUDataObject.GPUItems.Count);

        Vector3[] posList = new Vector3[m_InstanceCount];
        for (int i = 0; i < m_InstanceCount; i++)
        {
            posList[i] = m_GPUDataObject.GPUItems[i].Position;
        }

        m_PosBuffer = new ComputeBuffer(m_InstanceCount, 3 * 4);
        m_PosBuffer.SetData(posList);


        m_Args = new uint[] { m_Mesh.GetIndexCount(0), 0, 0, 0, 0 };
        m_BufferWithArgs = new ComputeBuffer(5, sizeof(uint), ComputeBufferType.IndirectArguments);
        m_BufferWithArgs.SetData(m_Args);

        var posVisibleBuffer = new ComputeBuffer(m_InstanceCount, 3 * 4);

        m_CSCullingID = m_ComputeShader.FindKernel("CSCulling");
        m_ComputeShader.SetBuffer(m_CSCullingID, "bufferWithArgs", m_BufferWithArgs);
        m_ComputeShader.SetBuffer(m_CSCullingID, "posAllBuffer", m_PosBuffer);
        m_ComputeShader.SetBuffer(m_CSCullingID, "posVisibleBuffer", posVisibleBuffer);

        m_DrawMat.SetBuffer("posVisibleBuffer", posVisibleBuffer);

    }
    void DoCulling()
    {
        m_Args[1] = 0;
        m_BufferWithArgs.SetData(m_Args);

        m_ComputeShader.SetVector("cmrPos", Camera.main.transform.position);
        m_ComputeShader.SetVector("cmrDir", Camera.main.transform.forward);
        m_ComputeShader.SetFloat("cmrHalfFov", Camera.main.fieldOfView / 2);
        var m = GL.GetGPUProjectionMatrix(Camera.main.projectionMatrix, false) * Camera.main.worldToCameraMatrix;

        //高版本 可用  computeShader.SetMatrix("matrix_VP", m); 代替 下面数组传入
        float[] mlist = new float[] {
            m.m00,m.m10,m.m20,m.m30,
           m.m01,m.m11,m.m21,m.m31,
            m.m02,m.m12,m.m22,m.m32,
            m.m03,m.m13,m.m23,m.m33
        };


        m_ComputeShader.SetFloats("matrix_VP", mlist);
        m_ComputeShader.Dispatch(m_CSCullingID, m_CountsSqrted / 16, m_CountsSqrted / 16, 1);
    }

    void Update()
    {
        DoCulling();
        Graphics.DrawMeshInstancedIndirect(m_Mesh, 0, m_DrawMat, m_Bounds, m_BufferWithArgs, 0, null, ShadowCastingMode.Off, false);

    }


}
