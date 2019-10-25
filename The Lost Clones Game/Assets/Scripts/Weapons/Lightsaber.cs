using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightsaber : MonoBehaviour
{
    public float Damage;

    private Player player;

    void Start()
    {
        this.player = this.gameObject.GetComponentInParent<Player>();
    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            IDamagable enemy = other.gameObject.GetComponent<Enemy>();

            if (enemy != null && this.player.attacking)
            {
                enemy.TakeDamage(this.Damage);
            }
        }
    }
}
