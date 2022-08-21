using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public static class TestUtils 
{
    public static bool SrcHasDest(string src, string dest)
    {
        if (src == null || dest == null)
            return false;
        return src.ToLower().Contains(dest.ToLower());
    }

    public static object GetField(this object obj, string fieldName)
    {
        return obj.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(obj);
    }

}
