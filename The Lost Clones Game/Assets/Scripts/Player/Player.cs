using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour, IDamagable
{

    public GameObject LightsaberBlade;

    [HideInInspector]
    public bool IsSwinging;

    [HideInInspector]
    public bool IsDead;

    public float BaseHealth;
    public float BaseForceStamina;
    public float BaseLightsaberStamina;

    public int MainAttackAnimationsCount;

    [HideInInspector]
    public float Health;

    [HideInInspector]
    public float ForceStamina;

    [HideInInspector]
    public float LightsaberStamina;

    private Animator animator;

    [HideInInspector]
    public bool attacking;

    [HideInInspector]
    public bool blocking;

    private int attack;
    private bool nextAttack;
    private bool reloadingLightsaber;
    private bool isLightsaberActivated;

    void Start()
    {
        this.IsDead = false;

        this.animator = this.GetComponent<Animator>();

        this.attacking = false;

        this.Health = this.BaseHealth;
        this.ForceStamina = this.BaseForceStamina;
        this.LightsaberStamina = this.BaseLightsaberStamina;

        this.isLightsaberActivated = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            this.isLightsaberActivated = this.isLightsaberActivated ? false : true;
        }

        if (!this.blocking)
        {
            if (Input.GetMouseButtonDown(0) && this.attacking && this.attack != 0)
            {
                this.nextAttack = true;
            }

            if (Input.GetMouseButtonDown(0) && !this.attacking)
            {
                this.attacking = true;

                this.attack = 1;
            }
        }

        if (Input.GetMouseButtonDown(1) && this.LightsaberStamina != 0f)
        {
            this.blocking = true;

            this.reloadingLightsaber = false;
        }

        if (Input.GetMouseButtonUp(1))
        {
            this.blocking = false;
            this.reloadingLightsaber = true;
        }

        if (!this.blocking && (this.LightsaberStamina < 100f && this.LightsaberStamina >= 0f))
        {
            this.LightsaberStamina += 0.5f;
        }
    }

    void FixedUpdate()
    {
        if (this.reloadingLightsaber)
        {
            this.ReloadingLightsaberStamina();
        }

        this.AnimationParser();
        this.ActivateOrDeativateLightsaber();
    }

    private void AnimationParser()
    {
        if (!this.IsDead)
        {
            this.animator.SetInteger("Attack", this.attack);
            this.animator.SetBool("IsBlocking", this.blocking);
        }

        if (this.IsDead)
        {
            this.animator.SetInteger("Attack", 0);

            this.animator.SetBool("IsDead", this.IsDead);
        }
    }

    public void TakeDamage(float damage)
    {
        this.Health -= damage;

        this.Health = this.Health < 0f ? 0f : this.Health;

        if (this.Health == 0f && !this.IsDead)
        {
            this.IsDead = true;
        }
    }

    public void ReduceLightsaberStamina(float stamina)
    {
        this.LightsaberStamina -= stamina;

        this.LightsaberStamina = this.LightsaberStamina < 0f ? 0f : this.LightsaberStamina;

        if (this.LightsaberStamina == 0f && this.blocking)
        {
            this.blocking = false;
        }
    }

    private void ReloadingLightsaberStamina()
    {
        this.LightsaberStamina += 0.5f;

        this.LightsaberStamina = this.LightsaberStamina > 100f ? 100f : this.LightsaberStamina;

        if (this.LightsaberStamina == 100f)
        {
            this.reloadingLightsaber = false;
        }
    }

    private void ActivateOrDeativateLightsaber()
    {
        if (this.blocking || this.attacking)
        {
            this.LightsaberBlade.SetActive(true);
        }
        else
        {
            this.LightsaberBlade.SetActive(this.isLightsaberActivated);
        }
    }

    public void StartSwing()
    {
        this.IsSwinging = true;
    }

    public void StopAttack()
    {
        if (this.nextAttack && this.attack != this.MainAttackAnimationsCount)
        {
            this.attack++;

            this.nextAttack = false;
        }
        else
        {
            if (this.nextAttack && this.attack == this.MainAttackAnimationsCount)
            {
                this.attacking = true;

                this.attack = 1;
            }
            else
            {
                this.attacking = false;

                this.attack = 0;
            }
        }

        this.IsSwinging = false;
    }
}
