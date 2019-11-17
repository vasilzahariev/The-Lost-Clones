using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcePush : MonoBehaviour
{
    public float BaseDamage;
    public float BasePushForce;
    public float BaseDistance;
    public float BaseThickness;
    public float BaseStaminaCost;
    public float ActiveSeconds;

    private BoxCollider boxCollider;

    private float damage;
    private float pushForce;
    private float distance;
    private float thickness;
    private float staminaCost;

    void Start()
    {
        this.boxCollider = this.gameObject.GetComponent<BoxCollider>();

        this.damage = this.BaseDamage;
        this.pushForce = this.BasePushForce;
        this.distance = this.BaseDistance;
        this.thickness = this.BaseThickness;
        this.staminaCost = this.BaseStaminaCost;

        this.boxCollider.size = new Vector3(this.thickness, this.boxCollider.size.y, this.distance);
        this.boxCollider.center = new Vector3(this.boxCollider.center.x, this.boxCollider.center.y, this.boxCollider.center.z + (this.distance / 2));
    }

    private void Update()
    {
        StartCoroutine(this.Count());
    }

    private IEnumerator Count()
    {
        yield return new WaitForSeconds(this.ActiveSeconds);

        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rg = other.gameObject.GetComponent<Rigidbody>();

        if (rg != null)
        {
            rg.AddForce(this.transform.forward * this.pushForce);
            rg.AddForce(this.transform.up * this.pushForce / 1.5f);
        }
    }
}
