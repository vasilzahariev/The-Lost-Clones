using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBullet : MonoBehaviour
{
    public float Speed;
    public float Damage;

    //private int direction;
    private Camera playerCamera;

    private Vector3 direction;

    private float xRot;
    private float yRot;
    private float zRot;

    void Start()
    {
        //this.direction = 1;

        this.playerCamera = Camera.main;

        this.direction = this.transform.forward * this.Speed;

        this.xRot = -90f;
        this.yRot = 0f;
        this.zRot = 0f;

        this.transform.parent = null;
    }

    void FixedUpdate()
    {
        //this.transform.Translate(this.direction * this.transform.forward * this.Speed * Time.fixedDeltaTime);
        //this.transform.Translate(this.direction * (this.transform.forward + this.transform.right) * this.Speed * Time.fixedDeltaTime);

        this.transform.Translate(this.direction * Time.fixedDeltaTime);
        this.transform.rotation = Quaternion.Euler(this.xRot, this.yRot, this.zRot);
    }

    public void ChangeDirection()
    {
        //this.direction *= -1;

        this.direction = -this.transform.forward * this.Speed;
        this.xRot += (this.playerCamera.transform.rotation.x * 100f);
        this.yRot = (this.playerCamera.transform.rotation.y * 100f) + 5;
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
