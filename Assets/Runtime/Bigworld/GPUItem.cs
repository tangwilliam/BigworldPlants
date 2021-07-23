using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Will
{
    [Serializable]
    public struct GPUItem
    {
        public Vector3 Position;
        public Matrix4x4 Matrix;

        //public uint GPUTypeId;

        public GPUItem( Matrix4x4 matrix, Vector3 position, uint id)
        {
            Matrix = matrix;
            Position = position;
            //GPUTypeId = id;
        }
    }

    [Serializable]
    public class GPUType
    {
        public Material TypeMaterial;
        public Mesh TypeMesh;
        public uint TypeId;
        public uint TypeLOD;
    }
}
