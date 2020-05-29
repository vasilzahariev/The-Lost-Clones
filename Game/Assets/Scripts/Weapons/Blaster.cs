using UnityEngine;
using System.Collections;

public class Blaster : Weapon
{
    #region Properties

    public float BulletSpeed;

    #endregion

    #region Fields

    protected GameObject shootPoint;
    protected GameObject bulletPrefab;

    #endregion


    #region Methods

    public virtual void Shoot()
    {
        Bolt bullet = Instantiate(this.bulletPrefab, this.shootPoint.transform).GetComponent<Bolt>();
        bullet.Speed = this.BulletSpeed;
    }

    #endregion
}
