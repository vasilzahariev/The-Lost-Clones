using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    #region Properties

    public Console Console;

    [HideInInspector]
    public bool IsConsoleActive;

    #endregion

    #region MonoMethods

    void Start()
    {
        this.IsConsoleActive = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            this.IsConsoleActive = !this.IsConsoleActive;

            this.Console.gameObject.SetActive(this.IsConsoleActive);
        }
    }

    #endregion
}
