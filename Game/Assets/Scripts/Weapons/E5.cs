using UnityEngine;
using System.Collections;
using UnityEditor;

public class E5 : Blaster
{
    #region MonoMethods

    private void Awake()
    {
        this.shootPoint = UnityHelper.GetChildWithName(this.gameObject, "Shoot Point");
        this.bulletPrefab = (GameObject) AssetDatabase.LoadAssetAtPath("Assets/Weapons/Blasters/Prefabs/Red Bolt.prefab", typeof(GameObject));
    }

    #endregion
}
