﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    #region Properties
    #endregion

    #region Fields

    PlayerVarHolder _playerVarHolder;

    #endregion

    #region MonoMethods

    private void Awake()
    {
        this._playerVarHolder = this.gameObject.GetComponent<PlayerVarHolder>();
    }

    void Start()
    {

    }

    void Update()
    {

    }

    #endregion

    #region Methods
    #endregion
}
