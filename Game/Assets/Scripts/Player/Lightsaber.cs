using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightsaber : MonoBehaviour
{
    #region Properties

    public GameObject Blade;
    public Player player;

    #endregion

    #region Fields

    ///

    #endregion

    #region MonoMethods

    void Start()
    {
        //
    }


    void Update()
    {
        if (this.player.IsConsoleActive)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            this.TurnTheBaldeOff();
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

    //

    #endregion
}
