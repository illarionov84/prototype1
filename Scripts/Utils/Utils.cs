using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static void Shuffle<T>(this IList<T> list)
    {
        var n = list.Count;
        while (n > 1)
        {
            n--;
            var k = Random.Range(0, n + 1);
            var value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    
    public static int RandomExclude(int count, params int[] excludeValues)
    {
        List<int> range = new List<int>();
        bool isExludeValue;
        for (int i = 0; i < count; i++)
        {
            isExludeValue = false;
            for (int j = 0; j < excludeValues.Length; j++)
            {
                if (i == excludeValues[j])
                {
                    isExludeValue = true;
                    break;
                }
            }

            if (!isExludeValue)
            {
                range.Add(i);
            }
        }
        return range[Random.Range(0, range.Count)];
    }
}
