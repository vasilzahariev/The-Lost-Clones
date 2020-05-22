using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBullet : MonoBehaviour
{
    private GameObject parent;

    #region MonoMethods

    void Start()
    {
        this.parent = this.transform.parent.parent.gameObject;

        this.transform.parent = null;
    }

    void Update()
    {
        this.transform.Translate(-this.transform.right * 10f * Time.deltaTime);
    }

    #endregion

    #region Collisions

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IDamagable<float> player = other.gameObject.GetComponent<IDamagable<float>>();

            player.TakeDamage(10f);
        }

        if (other.gameObject != this.parent)
        {
            Destroy(this.gameObject);
        }
    }

    #endregion
}
