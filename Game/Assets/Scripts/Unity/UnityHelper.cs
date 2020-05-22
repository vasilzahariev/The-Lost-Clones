using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnityHelper
{
    public static GameObject GetChildWithName(GameObject parent, string childName)
    {
        GameObject child = null;
        int count = parent.transform.childCount;

        for (int index = 0; index < count; index++)
        {
            GameObject obj = parent.transform.GetChild(index).gameObject;

            if (obj.name == childName)
            {
                child = obj;

                break;
            }
        }

        if (child == null)
        {
            Debug.LogError($"{parent.name} doesn't have a child with name: {childName}");
        }

        return child;
    }
}
