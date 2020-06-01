using UnityEngine;
using System.Collections;

public class Blaster : Weapon
{
    #region Properties

    public float BulletSpeed;

    [Range(1f, 1000f)]
    public float RateOfFire;

    [HideInInspector]
    public bool CanShoot;

    [HideInInspector]
    public bool Reloading;

    #endregion

    #region Fields

    protected GameObject shootPoint;
    protected GameObject bulletPrefab;
    protected Enemy wielder;

    #endregion


    #region Methods

    public virtual void Shoot()
    {
        Bolt bullet = Instantiate(this.bulletPrefab, this.shootPoint.transform).GetComponent<Bolt>();

        StartCoroutine(this.Reload());
    }

    public Enemy GetWielder()
    {
        return this.wielder;
    }

    protected float GetSecondsBetweenShots()
    {
        float rateOfFirePerSecond = this.RateOfFire / 60f;

        float secondsBetweenShots = 1f / rateOfFirePerSecond;

        return secondsBetweenShots;
    }

    protected virtual IEnumerator Reload()
    {
        float seconds = this.GetSecondsBetweenShots();

        this.Reloading = true;

        yield return new WaitForSecondsRealtime(seconds);

        this.Reloading = false;
    }

    #endregion
}
