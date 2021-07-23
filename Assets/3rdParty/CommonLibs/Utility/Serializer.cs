using System.Collections;
using System.Collections.Generic;

public static class SimpleSerializer
{
    /// <summary>
    /// 这个方法和GetValByBit是一对，是序列化的方法
    /// </summary>
    /// <param name="list"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static List<uint> BoolListToIntList(List<bool> list, int count)
    {
        if (list.Count < 1)
        {
            return new List<uint>();
        }
        var intCount = (count - 1) / 32 + 1;
        var intList = new List<uint>(intCount);
        for (int i = 0; i < intCount; i++)
        {
            uint val = 0;
            for (int j = 0; j < 32; j++)
            {
                var start = i * 32 + j;
                if (start > list.Count - 1)
                {
                    continue;
                }
                val += list[start] ? 1u << j : 0u;
            }
            intList.Add(val);
        }
        return intList;
    }

    public static byte[] BoolListToBytes(List<bool> list, int count)
    {
        var bytes = new byte[count / 8 + 1];
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = 0;
            for (int j = 0; j < 8; j++)
            {
                var index = i * 8 + j;
                if (index >= count)
                {
                    break;
                }
                if (list[index])
                {
                    bytes[i] += (byte)(1 << j);
                }
            }
        }
        return bytes;
    }

    public static List<bool> BytesToBoolList(byte[] bytes, int offset, int length, List<bool> resultList = null)
    {
        if (null == resultList)
        {
            resultList = new List<bool>();
        }
        resultList.Clear();
        for (int i = offset; i < length; i++)
        {
            var bt = bytes[i];
            for (int j = 0; j < 8; j++)
            {
                resultList.Add((bt >> j) % 2 == 0 ? false : true);
            }
        }
        return resultList;
    }

    public static BitArray BytesToBitArray(byte[] bytes, int offset, int length, BitArray array)
    {
        var index = 0;
        for (int i = offset; i < length; i++)
        {
            var bt = bytes[i];
            for (int j = 0; j < 8; j++)
            {
                array.Set(index++, (bt >> j) % 2 == 0 ? false : true);
            }
        }
        return array;
    }

    /// <summary>
    /// 这个方法和BoolListToIntList 是一对，是反序列化的方法
    /// </summary>
    /// <param name="list"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static bool GetValByBit(List<uint> list, int index)
    {
        var outIndex = index / 32;
        if (outIndex > list.Count - 1)
        {
            return false;
        }
        var val = list[outIndex];
        var innerIndex = index % 32;
        return (val >> innerIndex) % 2 == 1;
    }
}