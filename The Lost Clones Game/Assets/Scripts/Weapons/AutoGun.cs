using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoGun : MonoBehaviour
{
    public GameObject FirePoint;
    public GameObject BulletPrefab;

    private bool canShoot;

    private void Start()
    {
        this.canShoot = true;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.F) && this.canShoot)
        {
            Shoot();

            StartCoroutine(this.CountdownForShooting());
        }
    }

    private void Shoot()
    {
        Instantiate(this.BulletPrefab, this.FirePoint.transform);

        this.canShoot = false;
    }

    private IEnumerator CountdownForShooting()
    {
        this.canShoot = false;

        yield return new WaitForSeconds(0.2f);

        this.canShoot = true;
    }
}
