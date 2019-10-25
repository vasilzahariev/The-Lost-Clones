using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable
{
    public float Health;

    private bool isDead;

    // Start is called before the first frame update
    void Start()
    {
        this.isDead = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(float damage)
    {
        this.Health -= damage;

        this.Health = this.Health > 0f ? this.Health : 0f;

        if (this.Health == 0f)
        {
            this.isDead = true;

            this.Die();
        }
    }

    private void Die()
    {
        Destroy(this.gameObject);
    }
}
