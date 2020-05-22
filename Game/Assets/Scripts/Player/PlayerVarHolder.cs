using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVarHolder : MonoBehaviour
{
    [HideInInspector]
    public Player Player;

    [HideInInspector]
    public PlayerMovement PlayerMovement;

    [HideInInspector]
    public LightsaberController LightsaberController;

    private void Awake()
    {
        this.Player = this.gameObject.GetComponent<Player>();
        this.PlayerMovement = this.gameObject.GetComponent<PlayerMovement>();
        this.LightsaberController = this.Player.GetComponentInChildren<LightsaberController>();
    }
}
