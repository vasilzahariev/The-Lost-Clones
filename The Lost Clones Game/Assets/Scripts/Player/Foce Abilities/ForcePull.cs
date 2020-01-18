using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcePull : MonoBehaviour
{
    public float BaseDamage;
    public float BasePullForce;
    public float BaseDistance;
    public float BaseThickness;
    public float BaseStaminaCost;

    private BoxCollider boxCollider;

    private float damage;
    private float pullForce;
    private float distance;
    private float thickness;
    private float staminaCost;

    void Start()
    {
        this.boxCollider = this.gameObject.GetComponent<BoxCollider>();

        this.damage = this.BaseDamage;
        this.pullForce = this.BasePullForce;
        this.distance = this.BaseDistance;
        this.thickness = this.BaseThickness;
        this.staminaCost = this.BaseStaminaCost;

        this.boxCollider.size = new Vector3(this.thickness, this.boxCollider.size.y, this.distance);
        this.boxCollider.center = new Vector3(this.boxCollider.center.x, this.boxCollider.center.y, this.boxCollider.center.z + (this.distance / 2));
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rg = other.gameObject.GetComponent<Rigidbody>();

        if (rg != null)
        {
            rg.AddForce(-this.transform.forward * this.pullForce);
            rg.AddForce(this.transform.up * this.pullForce / 5f);
        }
    }
}
