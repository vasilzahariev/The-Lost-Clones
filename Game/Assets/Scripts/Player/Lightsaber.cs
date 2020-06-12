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
    private LightsaberController _lightsaberController;

    private GameObject _trails;

    private bool _canDealDamage;

    #endregion

    #region MonoMethods

    private void Awake()
    {
        this.Blade = UnityHelper.GetChildWithName(this.gameObject, "Blade");

        _lightsaberController = this.gameObject.GetComponentInParent<LightsaberController>();

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
                if (_lightsaberController.StealthKilling)
                    enemy.TakeDamage(1000f, _player.gameObject);
                else
                    enemy.TakeDamage(this.Damage, _player.gameObject);
            }
        }
    }

    #endregion
}
