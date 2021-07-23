using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomOrdinalStringComparer : IEqualityComparer<string>
{
    private static CustomOrdinalStringComparer m_Comparer = new CustomOrdinalStringComparer();
    public static CustomOrdinalStringComparer GetComparer()
    {
        return m_Comparer;
    }

    public bool Equals(string a, string b)
    {
        if (a.Length != b.Length) return false;
        int aLen = a.Length;
        int i = 0;
        while (i < aLen && a[i] == b[i])
        {
            i++;
        }

        return i == aLen;
    }

    public int GetHashCode(string c)
    {
        return c.GetHashCode();
    }

}
