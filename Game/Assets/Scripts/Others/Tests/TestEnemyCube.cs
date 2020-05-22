using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyCube : MonoBehaviour, IDamagable<float>, ITargetable, IKillable
{
    #region Properties

    public float Health = 100f;

    #endregion

    #region Field

    private Transform lookAt;

    #endregion

    #region MonoMethods

    void Awake()
    {
        this.lookAt = UnityHelper.GetChildWithName(this.gameObject, "LookAt").transform;
    }

    void Update()
    {
    }

    private void FixedUpdate()
    {
        if (this.Health == 0f)
        {
            this.Die();
        }
    }

    #endregion

    #region Methods

    private IEnumerator WaitBeforeDestroying(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);

        Destroy(this.gameObject);
    }

    #endregion

    #region InterfaceMethods

    public void TakeDamage(float damage)
    {
        float newHealth = this.Health - damage;

        this.Health = newHealth > 0f ? newHealth : 0f; 
    }

    public Transform GetLookAt()
    {
        return this.lookAt;
    }

    public void Die()
    {
        StartCoroutine(this.WaitBeforeDestroying(2f));
    }

    public bool isDead()
    {
        return this.Health == 0f ? true : false;
    }

    #endregion
}
