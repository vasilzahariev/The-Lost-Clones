using UnityEngine;
using System.Collections;
using UnityEditor;

public class E5 : Blaster
{

    #region MonoMethods

    private void Awake()
    {
        this.shootPoint = UnityHelper.GetChildWithName(this.gameObject, "ShootPoint");
        this.bulletPrefab = (GameObject) AssetDatabase.LoadAssetAtPath("Assets/Weapons/Blasters/Prefabs/Red Bolt.prefab",
                                                                       typeof(GameObject));

        this.wielder = this.gameObject.GetComponentInParent<Enemy>();
        this.CanShoot = true;
    }

    private void FixedUpdate()
    {
        if (this.CanShoot && !this.Reloading && this.wielder.Target != null)
            this.Shoot();
    }

    #endregion

    #region Methods

    #endregion
}
