using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGun : MonoBehaviour
{
    #region Properties

    public GameObject BulletPrefab;

    #endregion

    #region Fields

    private GameObject shootPoint;

    private bool canShoot = true;

    #endregion Fields

    #region MonoMethods

    private void Awake()
    {
        this.shootPoint = UnityHelper.GetChildWithName(this.gameObject, "ShootPoint");

        //this.canShoot = true;
    }

    private void Update()
    {
        if (this.canShoot)
            this.Shoot();
    }

    #endregion

    #region Methods

    private void Shoot()
    {
        Instantiate(this.BulletPrefab, this.shootPoint.transform);

        this.canShoot = false;

        StartCoroutine(this.WaitBeforeShooting());
    }

    private IEnumerator WaitBeforeShooting()
    {
        yield return new WaitForSecondsRealtime(1f);

        this.canShoot = true;
    }

    #endregion
}
