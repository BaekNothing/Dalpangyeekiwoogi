using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Spine;
using Spine.Unity;
using Unity.Notifications.Android;
using GoogleMobileAds.Api;
using System;
using TMPro;

public static class CalculateUtility 
{
    public static float TimeToNumber(string yyyy_MM_dd_HH_mm, ref float Number){

        int Counter = 0;
        Number = 0f;

        foreach (char n in yyyy_MM_dd_HH_mm) {

            //더 세련되게 적을 수 있는 방법이 있다는걸 알긴 하는데., 그럼 나중에 못고칠듯 
            // yyyy _ MM _ dd _  H H _ mm
            // 0123 4 56 7 89 10 1112  1314

            if (Counter == 4 || Counter == 7 || Counter == 10 || Counter == 13) { /* DoNothing */ }
            else if (Counter == 0)
            { Number += int.Parse(n.ToString()) * 8760f * 60f; } //Year
            else if (Counter == 1)
            { Number += int.Parse(n.ToString()) * 8760f * 60f; }
            else if (Counter == 2)
            { Number += int.Parse(n.ToString()) * 8760f * 60f; }
            else if (Counter == 3)
            { Number += int.Parse(n.ToString()) * 8760f * 60f; }

            else if (Counter == 5)
            { Number += int.Parse(n.ToString()) * 10f * 732f * 60f; } //Month
            else if (Counter == 6)
            { Number += int.Parse(n.ToString()) * 732f * 60f; } //Month

            else if (Counter == 8)
            { Number += int.Parse(n.ToString()) * 10f * 24f * 60f; } //Day
            else if (Counter == 9)
            { Number += int.Parse(n.ToString()) * 24f * 60f; } 


            else if (Counter == 11)
            { Number += int.Parse(n.ToString()) * 10f * 60f; } //Hour
            else if (Counter == 12)
            { Number += int.Parse(n.ToString()) * 60f; } 

            else if (Counter == 14)
            { Number += int.Parse(n.ToString()) * 10f; } //Minuate
            else if (Counter == 15)
            { Number += int.Parse(n.ToString()); }

            Counter++;
        }
        
        // Debug.Log(Number.ToString());
        return Number;
    }
}

public class CSVReader
{
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };

    public static List<Dictionary<string, object>> Read(string file)
    {
        var list = new List<Dictionary<string, object>>();
        TextAsset data = Resources.Load(file) as TextAsset;

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);

        if (lines.Length <= 1) return list;

        var header = Regex.Split(lines[0], SPLIT_RE);
        for (var i = 1; i < lines.Length; i++)
        {

            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;

            var entry = new Dictionary<string, object>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                object finalvalue = value;
                int n;
                float f;
                if (int.TryParse(value, out n))
                {
                    finalvalue = n;
                }
                else if (float.TryParse(value, out f))
                {
                    finalvalue = f;
                }
                entry[header[j]] = finalvalue;
            }
            list.Add(entry);
        }
        return list;
    }
}
