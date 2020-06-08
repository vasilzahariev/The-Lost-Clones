using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// This class controlls the FPS element.
/// </summary>
public class FPS : MonoBehaviour
{
    /// <summary>
    /// All private variables
    /// </summary>
    #region Fields

    private TMP_Text _fpsText; // The text element

    private float _frameCount = 0f;
    private float _dt = 0f;
    private float _fps = 0f;
    private float _updateRate = 4f;

    #endregion

    #region MonoMethods

    private void Awake()
    {
        _fpsText = this.GetComponent<TMP_Text>();
    }

    private void Update()
    {
        this.UpTheFrame();

        this.DisplayFPS();
    }

    #endregion

    #region Methods

    private void UpTheFrame()
    {
        _frameCount++;

        _dt += Time.deltaTime;

        if (_dt > 1f / _updateRate)
        {
            _fps = _frameCount / _dt;

            _frameCount = 0;

            _dt -= 1f / _updateRate;
        }
    }

    private void DisplayFPS()
    {
        _fpsText.text = $"{Mathf.Ceil(_fps)} FPS";
    }

    #endregion
}
