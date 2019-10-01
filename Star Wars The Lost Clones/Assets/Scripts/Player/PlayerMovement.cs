using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Camera Camera;

    public float Speed;

    private float h;
    private float v;

    void Start()
    {
        
    }

    void Update()
    {
        this.h = Input.GetAxis("Horizontal") * this.Speed;
        this.v = Input.GetAxis("Vertical") * this.Speed;
    }

    void FixedUpdate()
    {
        this.Rotate();

        this.transform.Translate(this.h * Time.fixedDeltaTime, 0f, this.v * Time.fixedDeltaTime);
    }

    private void Rotate()
    {
        this.transform.rotation = new Quaternion(0f, this.Camera.transform.rotation.y * Time.fixedDeltaTime, 0f, this.Camera.transform.rotation.w);
    }
}
