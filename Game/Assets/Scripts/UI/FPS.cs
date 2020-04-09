using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPS : MonoBehaviour
{
    private TMP_Text fpsText;

    private float frameCount = 0f;
    private float dt = 0f;
    private float fps = 0f;
    private float updateRate = 4f;

    #region MonoMethods

    private void Awake()
    {
        this.fpsText = this.GetComponent<TMP_Text>();
    }

    private void Update()
    {
        this.frameCount++;

        this.dt += Time.deltaTime;

        if (this.dt > 1f / this.updateRate)
        {
            this.fps = this.frameCount / this.dt;

            this.frameCount = 0;

            this.dt -= 1f / this.updateRate;
        }

        this.fpsText.text = $"{Mathf.Ceil(this.fps)} FPS";
    }

    #endregion
}
