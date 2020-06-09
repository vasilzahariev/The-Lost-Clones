using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a static class that helps with some commonly used Unity methods and etc
/// </summary>
public static class UnityHelper
{
    /// <summary>
    /// Finds a children with the given name
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="childName"></param>
    /// <returns></returns>
    public static GameObject GetChildWithName(GameObject parent, string childName)
    {
        GameObject child = null;

        Transform[] children = parent.GetComponentsInChildren<Transform>(true);

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

    public static GameObject GetParentWithName(GameObject child, string parentName)
    {
        GameObject parent = null;

        Transform[] parents = child.GetComponentsInParent<Transform>();

        foreach (Transform currentParent in parents)
        {
            if (currentParent.name == parentName)
            {
                parent = currentParent.gameObject;

                break;
            }
        }

        if (parent == null)
            Debug.LogError($"{child.name} doesn't have a parent with name: {parentName}");

        return parent;
    }
}
