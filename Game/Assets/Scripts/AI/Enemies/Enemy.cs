using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The base class of all enemies
/// </summary>

public class Enemy : MonoBehaviour, IDamagable<float>, IKillable, ITargetable, IShootable
{
    #region Properties

    [Range(1f, 1000f)]
    public float Health; // The health of the Enemy

    public GameObject Target { get; protected set; } // The current target of the Enemy (where to look and shoot)

    public Weapon Weapon { get; protected set; } // The weapon of the Enemy

    #endregion

    #region Fields

    protected Transform _lookAt; // Where other AI and the player should look at when targeting
    protected Transform _eyes; // (To be improved) The eyes of the Enemy (the start point of the Raycast that looks for the player)
    protected Transform _shootAt; // Where other AI and the player should shoot their bullets

    protected Animator _animator; // The Animator component of the Enemy object

    #endregion

    #region Methods

    /// <summary>
    /// This method controlls the animations
    /// </summary>
    protected virtual void AnimationParser()
    {
    }

    #endregion

    #region InterfaceMethods

    /// <summary>
    /// This method is called when the Enemy has been hit by something that damages it
    /// This method reduces the health of the Enemy by a specific values
    /// </summary>
    /// <param name="damage">The amount of health that is reduced</param>
    public virtual void TakeDamage(float damage)
    {
        float newHealth = this.Health - damage;

        this.Health = newHealth > 0 ? newHealth : 0f;
    }

    /// <summary>
    /// This method is called when the Enemy has reached 0 health and has to be destroyed
    /// This method destroys the AI object (not script, but object)
    /// </summary>
    public virtual void Die()
    {
        Destroy(this.gameObject);
    }

    /// <summary>
    /// This method is called to check if the Enemy is dead
    /// </summary>
    /// <returns>If the Enemy is dead</returns>
    public virtual bool isDead()
    {
        return this.Health <= 0f;
    }

    /// <summary>
    /// This method is called when you want to access the Look At
    /// </summary>
    /// <returns>Look At Transform</returns>
    public Transform GetLookAt()
    {
        return this._lookAt;
    }

    /// <summary>
    /// This method is called when you want to access the Shoot At
    /// </summary>
    /// <returns>Shoot At Transform</returns>
    public Transform GetShootAt()
    {
        return this._shootAt;
    }

    /// <summary>
    /// This method makes the Enemy look at and locks it at it's target
    /// </summary>
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
