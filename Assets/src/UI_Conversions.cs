using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Conversions
{
    static public int UserInputToInt(string input)
    {
        if(string.IsNullOrEmpty(input))
        {
            return -1;
        }

        return System.Convert.ToInt32(input);
    }

    static public int UserInputToInt(TMP_InputField input)
    {
        string str = input.text;
        if(string.IsNullOrEmpty(str))
        {
            return -1;
        }
        return System.Convert.ToInt32(str);
    }

    static public float UserInputToFloat(string input)
    {
        if(string.IsNullOrEmpty(input))
        {
            return -1.0f;
        }
        return System.Convert.ToSingle(input);
    }

    static public float UserInputToFloat(TMP_InputField input)
    {
        string str = input.text;
        if(string.IsNullOrEmpty(str))
        {
            return -1.0f;
        }
        return System.Convert.ToSingle(str);
    }

}
