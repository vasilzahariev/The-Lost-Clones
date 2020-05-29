﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable<float>, IKillable, ITargetable
{
    #region Properties

    [Range(1f, 1000f)]
    public float Health;

    [HideInInspector]
    public GameObject Target;

    #endregion

    #region Fields

    protected Transform lookAt;
    protected Transform eyes;

    protected Animator animator;

    #endregion

    #region Methods

    protected virtual void AnimationParser()
    {
    }

    #endregion

    #region InterfaceMethods

    public virtual void TakeDamage(float damage)
    {
        float newHealth = this.Health - damage;

        this.Health = newHealth > 0 ? newHealth : 0f;
    }

    public virtual void Die()
    {
        Destroy(this.gameObject);
    }

    public virtual bool isDead()
    {
        return this.Health <= 0f;
    }

    public Transform GetLookAt()
    {
        return this.lookAt;
    }

    protected virtual void LockOnTarget()
    {
        this.transform.LookAt(this.Target.transform);
        this.transform.rotation = new Quaternion(0f,
                                                 this.transform.rotation.y,
                                                 0f,
                                                 this.transform.rotation.w);
    }

    #endregion
}
