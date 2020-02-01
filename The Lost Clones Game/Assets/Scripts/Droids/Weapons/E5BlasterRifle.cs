using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class E5BlasterRifle : MonoBehaviour, IBlaster
{
    private AudioSource ShotSound;

    private float maxDamage;
    private float minDamage;
    private float rateOfFire; // in RPMs
    private float rateOfFirePerSecond;
    private float secondsBetweenShots;
    private float startDropOffRange;
    private float endDropOffRange;
    private float venting;
    private float overheat; // shots
    private float heatPerShot;
    private float overheatValue; // in heat
    private float overheatedPenalty; // in seconds
    private float PassiveCooldownDelay; // in seconds
    private float PassiveCooldown; // heat per second

    private float currentHeat;

    private bool canShoot;
    private bool shouldReload;

    void Start()
    {
        this.ShotSound = this.gameObject.GetComponent<AudioSource>();

        this.maxDamage = 36f;
        this.minDamage = 19f;
        this.rateOfFire = 300f;
        this.rateOfFirePerSecond = this.rateOfFire / 60f;
        this.secondsBetweenShots = 1 / this.rateOfFirePerSecond;
        this.startDropOffRange = 20f;
        this.endDropOffRange = 40f;
        this.venting = 1f;
        this.overheat = 25f;
        this.heatPerShot = 0.04f;
        this.overheatValue = this.overheat * this.heatPerShot;
        this.overheatedPenalty = 1f;
        this.PassiveCooldownDelay = 5f;
        this.PassiveCooldown = 0.3f;

        this.currentHeat = 0f;
        this.canShoot = true;
        this.shouldReload = false;
    }

    private void Update()
    {
        if (this.canShoot && !this.shouldReload)
        {
            this.currentHeat += this.heatPerShot;

            if (this.currentHeat >= this.overheatValue)
            {
                this.shouldReload = true;
            }

            this.ShotSound.Play();

            if (!this.shouldReload)
            {
                StartCoroutine(this.WaitToShoot());
            }
            else
            {
                StartCoroutine(this.ReloadWait());
            }
        }
    }

    private IEnumerator WaitToShoot()
    {
        this.canShoot = false;
        
        yield return new WaitForSecondsRealtime(this.secondsBetweenShots);

        this.canShoot = true;
    }

    private IEnumerator ReloadWait()
    {
        yield return new WaitForSecondsRealtime(this.venting);

        this.shouldReload = false;
        this.currentHeat = 0f;
    }

    public bool IsReloading()
    {
        return this.shouldReload;
    }
}
