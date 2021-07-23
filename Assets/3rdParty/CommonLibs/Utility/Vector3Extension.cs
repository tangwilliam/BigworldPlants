using UnityEngine;

namespace Assets.Scripts.Extensions
{
    public static class Vector3Extension
    {
        public static string ToDetailStirng(this Vector3 v3)
        {
            return string.Format("({0},{1},{2})", v3.x, v3.y, v3.z);
        }

        public static Vector3 Toxz(this Vector3 v3)
        {
            return new Vector3(v3.x, 0, v3.z);
        }

        public static Vector3 Toxy(this Vector3 v3)
        {
            return new Vector3(v3.x, v3.y, 0);
        }

        public static Vector3 Toyz(this Vector3 v3)
        {
            return new Vector3(0, v3.y, v3.z);
        }

        public static string ToStringEx(this Vector3 v3)
        {
            return string.Format("({0},{1},{2})", v3.x, v3.y, v3.z);
        }
    }
}
