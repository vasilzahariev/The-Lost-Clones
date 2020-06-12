using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        this.CanDie = true;

        _lookAt = UnityHelper.GetChildWithName(this.gameObject, "LookAt").transform;
        _eyes = UnityHelper.GetChildWithName(this.gameObject, "Eyes").transform;
        _shootAt = UnityHelper.GetChildWithName(this.gameObject, "ShootAt").transform;
        _animator = this.gameObject.GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (this.isDead() && this.CanDie)
        {
            this.Die();
        }

        if (!this.IsStealthKilled)
        {
            this.LookForTarget();

            if (this.Target != null)
            {
                this.LockOnTarget();
            }
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
        _animator.SetBool("Aiming", _aim);
        _animator.SetBool("Shooting", _shoot);
        _animator.SetBool("StealthKilled", this.IsStealthKilled);

        base.AnimationParser();
    }

    public override void Die()
    {
        Destroy(this.Weapon);
        base.Die();
    }

    /// <summary>
    /// This method sends a Raycast that looks for the player (to be changed for friendly AI)
    /// </summary>
    private void LookForTarget()
    {
        LayerMask mask =~ LayerMask.GetMask("Player");

        // TODO: When there is a friendly AI add the search for them
        Collider[] hitColliders = Physics.OverlapSphere(_eyes.position, _viewRadius)
                                         .ToList().FindAll(t => t.transform.gameObject.CompareTag("Player")).ToArray();

        if (hitColliders.Length > 0)
        {
            foreach (Collider hitCollider in hitColliders)
            {
                Transform target = hitCollider.gameObject.transform;
                Vector3 dirToTarget = (_eyes.position - target.position).normalized;

                if (Vector3.Angle(-_eyes.forward, dirToTarget) < _viewAngle / 2f)
                {
                    RaycastHit hit;
                    float distance = Vector3.Distance(_eyes.position, target.position);

                    if (Physics.Raycast(_eyes.position, -dirToTarget, out hit, distance))
                    {
                        if (hit.transform.gameObject == target.transform.gameObject)
                        {
                            this.Target = target.gameObject;
                            _aim = true;
                        }
                        else
                        {
                            this.Target = null;
                            _aim = false;
                        }
                    }
                }
            }
        }
        else
        {
            this.Target = null;
            _aim = false;
        }
    }

    #endregion
}
