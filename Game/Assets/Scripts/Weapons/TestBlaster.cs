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
        this._shootPoint = UnityHelper.GetChildWithName(this.gameObject, "ShootPoint");
        this._bulletPrefab = (GameObject) AssetDatabase.LoadAssetAtPath("Assets/Weapons/Blasters/Prefabs/Red Bolt.prefab",
                                                                       typeof(GameObject));

        this.CanShoot = true;
    }

    private void FixedUpdate()
    {
        if (this.CanShoot && !this.Reloading)
            this.Shoot();
    }

    #endregion

    #region Methods

    public override void Shoot()
    {
        base.Shoot();
    }

    //protected override IEnumerator Reload() 
    //{
    //    float seconds = this.GetSecondsBetweenShots();

    //    yield return new WaitForSecondsRealtime(seconds);

    //    this.canShoot = true;
    //}

    #endregion
}
