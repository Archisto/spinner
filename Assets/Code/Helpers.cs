using UnityEngine;
using System.Collections;

public static class Helpers
{
    public static string ArrayToString<T>(this T[] array)
    {
        string str = "";

        if (array == null)
        {
            return str;
        }

        for (int i = 0; i < array.Length; i++)
        {
            if (i > 0)
            {
                str += ", ";
            }

            str += array[i].ToString();
        }

        return str;
    }
}
