using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightsaber : MonoBehaviour
{
    #region Properties

    public GameObject Blade { get; private set; }

    public float Damage { get; set; }

    #endregion

    #region Fields

    private Player _player;

    private GameObject _trails;

    private bool _canDealDamage;

    #endregion

    #region MonoMethods

    private void Awake()
    {
        this.Blade = UnityHelper.GetChildWithName(this.gameObject, "Blade");

        this._trails = UnityHelper.GetChildWithName(this.Blade, "Trails");

        this._trails.SetActive(false);
    }

    void Start()
    {
        this._player = Object.FindObjectOfType<Player>();

        this._canDealDamage = false;
    }


    void Update()
    {
        if (this._player.IsConsoleActive)
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
        this._trails.SetActive(active);
    }

    public void SetCanDealDamage(bool canDealDamage)
    {
        this._canDealDamage = canDealDamage;
    }

    #endregion

    #region Collision

    public void OnTriggerEnter(Collider other)
    {
        if (this._canDealDamage && other.gameObject.CompareTag("Enemy"))
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
