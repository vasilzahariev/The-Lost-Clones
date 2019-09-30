using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateModel : MonoBehaviour
{
    void Update()
    {
        this.transform.Rotate(0f, 60f * Time.deltaTime, 0f);
    }
}
