using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBullet : MonoBehaviour
{
    public float Speed;
    public float Damage;

    private int direction;

    void Start()
    {
        this.direction = 1;

        this.transform.parent = null;
    }

    void FixedUpdate()
    {
        this.transform.Translate(this.direction * this.transform.forward * this.Speed * Time.fixedDeltaTime);
        //this.transform.Translate(this.direction * (this.transform.forward + this.transform.right) * this.Speed * Time.fixedDeltaTime);
    }

    public void ChangeDirection()
    {
        this.direction *= -1;
    }

    private void Destroy()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Character"))
        {
            Player player = other.gameObject.GetComponent<Player>();

            if (player != null)
            {
                if (!player.IsDead)
                {
                    if (player.blocking)
                    {
                        player.ReduceLightsaberStamina(1f);

                        this.ChangeDirection();
                    }
                    else
                    {
                        player.TakeDamage(this.Damage);

                        this.Destroy();
                    }
                }
            }
        }
        else if (other.gameObject.CompareTag("Lightsaber"))
        {
            Lightsaber lightsaber = other.gameObject.GetComponent<Lightsaber>();

            if (lightsaber != null)
            {
                this.ChangeDirection();
            }
        }
        else if (!other.gameObject.CompareTag("Bullet"))
        {
            this.Destroy();
        }
    }
}
