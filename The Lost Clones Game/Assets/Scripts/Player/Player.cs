﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour, IDamagable
{

    public GameObject LightsaberBlade;
    public GameObject ForcePush;
    public GameObject ForcePull;

    public string[] AttackTypes;

    [HideInInspector]
    public string AttackType;

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
    private bool gravity;
    private bool isUsingTheForce;
    private bool isForcePushing;
    private bool isForcePulling;
    private bool isBlockLooping;

    void Start()
    {
        this.AttackType = this.AttackTypes[0];

        this.IsDead = false;

        this.animator = this.GetComponent<Animator>();

        this.attacking = false;

        this.Health = this.BaseHealth;
        this.ForceStamina = this.BaseForceStamina;
        this.LightsaberStamina = this.BaseLightsaberStamina;

        this.isLightsaberActivated = false;

        this.gravity = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            this.gravity = this.gravity ? false : true;
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            this.isLightsaberActivated = this.isLightsaberActivated ? false : true;

            if (this.isLightsaberActivated && !FindObjectOfType<AudioManager>().IsPlaying("LightsaberIdle"))
            {
                FindObjectOfType<AudioManager>().Stop("LightsaberStop");
                FindObjectOfType<AudioManager>().Play("LightsaberStart");
                FindObjectOfType<AudioManager>().PlayAfter("LightsaberIdle", "LightsaberStart");
            }
            else if (!this.isLightsaberActivated && !this.attacking && !this.blocking)
            {
                FindObjectOfType<AudioManager>().Stop("LightsaberStart");
                FindObjectOfType<AudioManager>().Stop("LightsaberIdle");
                FindObjectOfType<AudioManager>().Play("LightsaberStop");
            }
        }

        if (Input.GetKeyDown(KeyCode.F) && !this.attacking && !this.isUsingTheForce && !this.blocking)
        {
            this.isUsingTheForce = true;
            this.isForcePushing = true;
        }

        if (Input.GetKeyDown(KeyCode.Q) && !this.attacking && !this.isUsingTheForce && !this.blocking)
        {
            this.isUsingTheForce = true;
            this.isForcePulling = true;
        }

        if (!this.blocking && !this.isUsingTheForce)
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

        if (Input.GetMouseButtonDown(1) && this.LightsaberStamina != 0f && !this.isUsingTheForce)
        {
            this.blocking = true;

            this.reloadingLightsaber = false;

            if (!this.isLightsaberActivated)
            {
                FindObjectOfType<AudioManager>().Stop("LightsaberStop");
                FindObjectOfType<AudioManager>().Play("LightsaberStart");
                FindObjectOfType<AudioManager>().Play("LightsaberIdle");
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            this.blocking = false;
            this.reloadingLightsaber = true;

            if (!this.isLightsaberActivated)
            {
                FindObjectOfType<AudioManager>().Stop("LightsaberStart");
                FindObjectOfType<AudioManager>().Play("LightsaberStop");
                FindObjectOfType<AudioManager>().Stop("LightsaberIdle");
            }
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

        if (!this.blocking)
        {
            this.isBlockLooping = false;
        }

        this.gameObject.GetComponent<Rigidbody>().useGravity = this.gravity;

        this.AnimationParser();
        this.ActivateOrDeativateLightsaber();
    }

    private void AnimationParser()
    {
        if (!this.IsDead)
        {
            this.animator.SetInteger("Attack", this.attack);
            this.animator.SetBool("IsBlocking", this.blocking);
            this.animator.SetBool("IsBlockLooping", this.isBlockLooping);
            this.animator.SetBool("IsForcePushing", this.isForcePushing);
            this.animator.SetBool("IsForcePulling", this.isForcePulling);
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

    public void PlayForcePushSound()
    {
        FindObjectOfType<AudioManager>().Play("ForcePush");
    }

    public void PlayForcePullSound()
    {
        FindObjectOfType<AudioManager>().Play("ForcePull");
    }

    public void StartForce()
    {
        if (this.isForcePushing)
        {
            this.ForcePush.SetActive(true);
        }
        else if (this.isForcePulling)
        {
            this.ForcePull.SetActive(true);
        }
    }

    public void StopForce()
    {
        if (this.isForcePushing)
        {
            this.isForcePushing = false;

            this.ForcePush.SetActive(false);
        }
        else if (this.isForcePulling)
        {
            this.isForcePulling = false;

            this.ForcePull.SetActive(false);
        }

        this.isUsingTheForce = false;
    }

    public void BlockingLoop()
    {
        this.isBlockLooping = true;
    }
}
