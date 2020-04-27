using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightsaber : MonoBehaviour
{
    #region Properties

    public GameObject Blade;

    [HideInInspector]
    public float Damage;

    [HideInInspector]
    public bool CanDealDamage;

    #endregion

    #region Fields

    private Player player;

    private bool isAttacking;

    #endregion

    #region MonoMethods

    void Start()
    {
        this.player = Object.FindObjectOfType<Player>();

        this.CanDealDamage = false;
    }


    void Update()
    {
        if (this.player.IsConsoleActive)
        {
            return;
        }
    }

    #endregion

    #region Methods

    public void TurnTheBaldeOff()
    {
        this.Blade.gameObject.SetActive(!this.Blade.activeSelf);
    }

    #endregion

    #region Collision

    public void OnTriggerEnter(Collider other)
    {
        if (this.CanDealDamage && other.gameObject.CompareTag("Enemy"))
        {
            //
        }
    }

    #endregion
}
