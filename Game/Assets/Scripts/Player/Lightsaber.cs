using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightsaber : MonoBehaviour
{
    #region Properties

    [HideInInspector]
    public GameObject Blade;

    [HideInInspector]
    public float Damage;

    #endregion

    #region Fields

    private Player player;

    private GameObject trails;

    private bool canDealDamage;

    #endregion

    #region MonoMethods

    private void Awake()
    {
        this.Blade = UnityHelper.GetChildWithName(this.gameObject, "Blade");

        this.trails = UnityHelper.GetChildWithName(this.Blade, "Trails");

        this.trails.SetActive(false);
    }

    void Start()
    {
        this.player = Object.FindObjectOfType<Player>();

        this.canDealDamage = false;
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
        this.Blade.SetActive(!this.Blade.activeSelf);
    }

    public void TrailsOnOff(bool active)
    {
        this.trails.SetActive(active);
    }

    public void SetCanDealDamage(bool canDealDamage)
    {
        this.canDealDamage = canDealDamage;
    }

    #endregion

    #region Collision

    public void OnTriggerEnter(Collider other)
    {
        if (this.canDealDamage && other.gameObject.CompareTag("Enemy"))
        {
            IDamagable<float> enemy = other.GetComponent<IDamagable<float>>();

            if (enemy != null)
            {
                enemy.TakeDamage(this.Damage);
            }
        }
    }

    #endregion
}
