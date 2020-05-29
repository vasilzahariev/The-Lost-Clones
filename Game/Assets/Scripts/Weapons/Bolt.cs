using UnityEngine;
using System.Collections;

public class Bolt : MonoBehaviour
{
    #region Properties

    [HideInInspector]
    public float Speed;

    #endregion

    #region Fields

    private GameObject weapon;

    #endregion

    #region MonoMethods

    private void Awake()
    {
        this.weapon = this.gameObject.GetComponentInParent<Weapon>().gameObject;

        this.gameObject.transform.parent = null;
    }

    private void FixedUpdate()
    {
        this.Move();
    }

    #endregion

    #region Methods

    private void Move()
    {
        this.transform.Translate(-this.transform.right * this.Speed * Time.fixedDeltaTime);
    }

    #endregion

    #region Collision

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != this.weapon)
            Destroy(this.gameObject);
    }

    #endregion
}
