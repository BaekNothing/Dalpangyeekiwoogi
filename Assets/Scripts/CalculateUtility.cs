using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Spine;
using Spine.Unity;
using Unity.Notifications.Android;
using System;

public static class CalculateUtility 
{
    public static float TimeToNumber(string yyyy_MM_dd_HH_mm)
    {
        if (DateTime.TryParseExact(yyyy_MM_dd_HH_mm, "yyyy_MM_dd_HH_mm", 
            System.Globalization.CultureInfo.InvariantCulture, 
            System.Globalization.DateTimeStyles.None, out DateTime parsedDateTime))
        {
            float yearComponent = parsedDateTime.Year * 8760f * 60f;
            float monthComponent = parsedDateTime.Month * 732f * 60f;
            float dayComponent = parsedDateTime.Day * 24f * 60f;
            float hourComponent = parsedDateTime.Hour * 60f;
            float minuteComponent = parsedDateTime.Minute;
    
            return yearComponent + monthComponent + dayComponent + hourComponent + minuteComponent;
        }
        else
        {
            // Handle invalid format
            throw new ArgumentException("Invalid date format", nameof(yyyy_MM_dd_HH_mm));
        }
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
