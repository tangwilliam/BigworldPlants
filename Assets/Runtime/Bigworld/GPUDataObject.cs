using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Will
{
    [Serializable]
    public class GPUDataObject : ScriptableObject
    {
        public List<GPUItem> GPUItems;

        public GPUDataObject( List<GPUItem> gpuItems )
        {
            GPUItems = gpuItems;
        }

    }

}

