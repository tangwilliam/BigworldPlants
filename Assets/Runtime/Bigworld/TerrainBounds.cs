using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Will
{
    public struct TerrainBounds
    {
        public Vector2 MinXZ;
        public Vector2 MaxXZ;

        public TerrainBounds( Vector2 minXZ, Vector2 maxXZ)
        {
            MinXZ = minXZ;
            MaxXZ = maxXZ;
        }

        public bool IsInBounds( Vector2 P )
        {
            if((P.x >= MinXZ.x) && (P.x <= MaxXZ.x) && (P.y >= MinXZ.y) &&(P.y <= MaxXZ.y)){
                return true;
            }
            else
            {
                return false;
            }
            
        }

        public bool IsInBounds(Vector3 P)
        {
            if ((P.x >= MinXZ.x) && (P.x <= MaxXZ.x) && (P.z >= MinXZ.y) && (P.z <= MaxXZ.y))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
