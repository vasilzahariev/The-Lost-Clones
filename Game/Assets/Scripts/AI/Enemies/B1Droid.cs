using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The B1 Battle Droid class
/// </summary>
public class B1Droid : Enemy
{
    #region Properties

    public E5 Blaster { get; private set; } // The blaster (weapon) of the B1

    #endregion

    #region Fields

    private bool _aim; // Is the B1 aiming
    private bool _shoot; // Is the B1 shooting

    #endregion

    #region MonoMethods

    private void Awake()
    {
        this.Blaster = this.GetComponentInChildren<E5>();
        this.Weapon = this.Blaster;

        this._lookAt = UnityHelper.GetChildWithName(this.gameObject, "LookAt").transform;
        this._eyes = UnityHelper.GetChildWithName(this.gameObject, "Eyes").transform;
        this._shootAt = UnityHelper.GetChildWithName(this.gameObject, "ShootAt").transform;
        this._animator = this.gameObject.GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (this.isDead())
        {
            this.Die();
        }

        if (this.Target == null)
        {
            this.LookForTarget();
        }
        else
        {
            this.LockOnTarget();
        }
    }

    private void LateUpdate()
    {
        this.AnimationParser();
    }

    #endregion

    #region Methods

    /// <summary>
    /// This method controlls the animations
    /// </summary>
    protected override void AnimationParser()
    {
        this._animator.SetBool("Aiming", this._aim);
        this._animator.SetBool("Shooting", this._shoot);

        base.AnimationParser();
    }

    /// <summary>
    /// This method sends a Raycast that looks for the player (to be changed for friendly AI)
    /// </summary>
    private void LookForTarget()
    {
        RaycastHit hit;
        LayerMask mask = LayerMask.GetMask("Enemy");

        if (Physics.Raycast(this._eyes.position, this._eyes.forward, out hit, 100f))
        {
            Player player = hit.transform.gameObject.GetComponent<Player>();

            if (player != null)
            {
                this.Target = player.gameObject;
                this._aim = true;
            }
        }
    }

    #endregion
}
