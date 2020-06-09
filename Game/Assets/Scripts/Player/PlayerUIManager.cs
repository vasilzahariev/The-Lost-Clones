using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    #region Properties

    [SerializeField] private GameObject _playerObject;

    #endregion

    #region Fields

    private Player _player;

    private Slider _health;

    #endregion

    private void Awake()
    {
        this._player = this._playerObject.GetComponent<Player>();
        this._health = UnityHelper.GetChildWithName(this.gameObject, "Health").GetComponent<Slider>();

        this._health.minValue = 0f;
        this._health.maxValue = this._player.Health;
        this._health.value = this._player.Health;
    }

    private void Update()
    {
        this._health.value = this._player.Health;
    }
}
