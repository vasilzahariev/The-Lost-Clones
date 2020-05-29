using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1Droid : Enemy
{
    #region Properties

    public E5 Weapon;

    #endregion

    #region Fields

    private bool aim;
    private bool shoot;
    private bool reaload;

    #endregion

    #region MonoMethods

    private void Awake()
    {
        this.lookAt = UnityHelper.GetChildWithName(this.gameObject, "LookAt").transform;
        this.eyes = UnityHelper.GetChildWithName(this.gameObject, "Eyes").transform;
        this.animator = this.gameObject.GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (this.isDead())
        {
            this.Die();
        }

        if (this.Target == null || this.reaload)
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

    protected override void AnimationParser()
    {
        this.animator.SetBool("Aiming", this.aim);
        this.animator.SetBool("Shooting", this.shoot);

        base.AnimationParser();
    }

    private void LookForTarget()
    {
        RaycastHit hit;
        LayerMask mask = LayerMask.GetMask("Enemy");

        if (Physics.Raycast(this.eyes.position, this.eyes.forward, out hit, 100f))
        {
            Player player = hit.transform.gameObject.GetComponent<Player>();

            if (player != null)
            {
                this.Target = player.gameObject;
                this.aim = true;
            }
        }
    }

    #endregion
}
