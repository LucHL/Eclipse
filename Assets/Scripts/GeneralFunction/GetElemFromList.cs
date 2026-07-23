using System.Collections.Generic;
using UnityEngine;

public static class GeneralFunction
{
    public static T GetRandomElementFromList<T>(this List<T> values)
    {
        if (values == null || values.Count == 0)
            return default;

        int randomIndex = Random.Range(0, values.Count);

        return values[randomIndex];
    }
}
