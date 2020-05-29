using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    #region Properties

    public GameObject PlayerObject;

    #endregion

    #region Fields

    private Player player;

    private Slider health;

    #endregion

    private void Awake()
    {
        this.player = this.PlayerObject.GetComponent<Player>();
        this.health = UnityHelper.GetChildWithName(this.gameObject, "Health").GetComponent<Slider>();

        this.health.minValue = 0f;
        this.health.maxValue = this.player.Health;
        this.health.value = this.player.Health;
    }

    private void Update()
    {
        this.health.value = this.player.Health;
    }
}
