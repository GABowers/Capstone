using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tools
{
    public static Dictionary<int, Color> ColorLookup = new Dictionary<int, Color>
    {
        {0, Color.black},
        {1, Color.blue},
        {2, Color.cyan},
        {3, Color.gray},
        {4, Color.green},
        {5, Color.magenta},
        {6, Color.red},
        {7, Color.white},
        {8, Color.yellow}
    };

    public static void AssignIntVal(ref int? target, string val)
    {
        int result;
        if (int.TryParse(val, out result))
            target = result;
        else
            target = null;
    }

    public static void AssignFloatVal(ref float? target, string val)
    {
        float result;
        if (float.TryParse(val, out result))
            target = result;
        else
            target = result;
    }
}
