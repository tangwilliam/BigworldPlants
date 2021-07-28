using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoDraw : MonoBehaviour
{
    [SerializeField]
    Mesh m_Mesh;

    BufferData[] data_ = new BufferData[2];

    [SerializeField]
    bool[] toDraw_ = new bool[2];

    [SerializeField]
    Material mat_;

    [SerializeField]
    int numToDraw_ = 2;

    private void Start()
    {
        data_[0] = new BufferData( m_Mesh, Vector3.zero, Color.white, 10);
        data_[1] = new BufferData( m_Mesh, Vector3.right * 11, Color.blue, 10);
    }

    private void Update()
    {
        for (int i = 0; i < 2; ++i)
        {
            if (toDraw_[i])
                data_[i].Draw(mat_);
        }
    }

    private void OnDisable()
    {
        foreach (var d in data_)
            d.Dispose();
    }
}

class BufferData : System.IDisposable
{
    ComputeBuffer argsBuffer_;

    ComputeBuffer posBuffer_;
    ComputeBuffer colorsBuffer_;

    Bounds bounds_;

    Mesh m_Mesh;

    MaterialPropertyBlock m_MaterialPropertyBlock;

    public void Draw(Material mat)
    {
        //mat.SetBuffer("posBuffer_", posBuffer_);
        //mat.SetBuffer("colorBuffer_", colorsBuffer_);

        m_MaterialPropertyBlock.SetBuffer("posBuffer_", posBuffer_);

        Graphics.DrawMeshInstancedIndirect(m_Mesh, 0, mat, bounds_, argsBuffer_, 0, m_MaterialPropertyBlock);
    }

    public BufferData(Mesh mesh,Vector3 pos, Color color, int size)
    {
        m_Mesh = mesh;
        m_MaterialPropertyBlock = new MaterialPropertyBlock();

        bounds_ = new Bounds(pos, pos + Vector3.one * size);

        int count = size * size;

        argsBuffer_ = new ComputeBuffer(1, sizeof(int) * 5, ComputeBufferType.IndirectArguments);
        argsBuffer_.SetData(new int[] { 6, count, 0, 0, 0 });

        Vector3[] posData = new Vector3[count];

        for (int x = 0; x < size; ++x)
        {
            for (int y = 0; y < size; ++y)
            {
                int i = y * size + x;
                posData[i] = pos + new Vector3(x, y, 0);
            }
        }

        posBuffer_ = new ComputeBuffer(count, sizeof(float) * 3);
        posBuffer_.SetData(posData);

        Color[] colorData = new Color[count];
        for (int i = 0; i < count; ++i)
            colorData[i] = color;

        colorsBuffer_ = new ComputeBuffer(count, sizeof(float) * 4);
        colorsBuffer_.SetData(colorData);
    }



    public void Dispose()
    {
        argsBuffer_.Release();
        posBuffer_.Release();
        colorsBuffer_.Release();
    }
}

