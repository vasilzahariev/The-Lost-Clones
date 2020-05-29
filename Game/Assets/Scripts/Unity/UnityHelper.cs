using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnityHelper
{
    public static GameObject GetChildWithName(GameObject parent, string childName)
    {
        GameObject child = null;

        Transform[] children = parent.GetComponentsInChildren<Transform>();

        foreach (Transform current in children)
        {
            if (current.gameObject.name == childName)
            {
                child = current.gameObject;

                break;
            }
        }

        if (child == null)
            Debug.LogError($"{parent.name} doesn't have a child with name: {childName}");

        return child;
    }
}
