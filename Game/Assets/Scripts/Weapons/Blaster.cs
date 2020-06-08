using UnityEngine;
using System.Collections;

public class Blaster : Weapon
{
    #region Properties

    public bool CanShoot { get; protected set; }

    public bool Reloading { get; protected set; }

    #endregion

    #region Fields

    public float BulletSpeed; // The speed of the bullet

    [Range(1f, 1000f)]
    public float RateOfFire;

    protected GameObject _shootPoint; // The point where the bullet prefab is being instantiated
    protected GameObject _bulletPrefab; // The bullet prefab

    // TODO: The wielder could be a clone trooper, so this needs to be changed in the future
    protected Enemy _wielder; // The AI that holds the weapon

    #endregion

    #region Methods

    public virtual void Shoot()
    {
        Bolt bullet = Instantiate(this._bulletPrefab, this._shootPoint.transform).GetComponent<Bolt>();

        StartCoroutine(this.Reload());
    }

    public Enemy GetWielder()
    {
        return this._wielder;
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
