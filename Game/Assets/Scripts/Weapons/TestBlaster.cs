using UnityEngine;
using System.Collections;
using UnityEditor;

public class TestBlaster : Blaster
{
    #region Fields

    private bool canShoot = true;

    #endregion

    #region MonoMethods

    private void Awake()
    {
        this.shootPoint = UnityHelper.GetChildWithName(this.gameObject, "ShootPoint");
        this.bulletPrefab = (GameObject) AssetDatabase.LoadAssetAtPath("Assets/Weapons/Blasters/Prefabs/Red Bolt.prefab", typeof(GameObject));
    }

    private void Update()
    {
        if (this.canShoot)
            this.Shoot();
    }

    #endregion

    #region Methods

    private IEnumerator Reload()
    {
        yield return new WaitForSecondsRealtime(3f);

        this.canShoot = true;
    }

    public override void Shoot()
    {
        base.Shoot();

        this.canShoot = false;

        StartCoroutine(this.Reload());
    }

    #endregion
}
