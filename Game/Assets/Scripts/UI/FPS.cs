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

    private TMP_Text fpsText; // The text element

    private float frameCount = 0f;
    private float dt = 0f;
    private float fps = 0f;
    private float updateRate = 4f;

    #endregion

    #region MonoMethods

    private void Awake()
    {
        this.fpsText = this.GetComponent<TMP_Text>();
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
        this.frameCount++;

        this.dt += Time.deltaTime;

        if (this.dt > 1f / this.updateRate)
        {
            this.fps = this.frameCount / this.dt;

            this.frameCount = 0;

            this.dt -= 1f / this.updateRate;
        }

        Debug.Log(this.dt);
    }

    private void DisplayFPS()
    {
        this.fpsText.text = $"{Mathf.Ceil(this.fps)} FPS";
    }

    #endregion
}
