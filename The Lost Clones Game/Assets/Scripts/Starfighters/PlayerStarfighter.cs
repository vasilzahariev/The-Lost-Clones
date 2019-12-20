using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStarfighter : MonoBehaviour
{
    public Camera Camera;

    public float Speed;
    public float RotationSpeed;

    private Rigidbody rg;

    private bool left;
    private bool right;

    void Start()
    {
        this.rg = this.gameObject.GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            this.left = true;
        }
        else
        {
            this.left = false;
        }

        if (Input.GetKey(KeyCode.D))
        {
            this.right = true;
        }
        else
        {
            this.right = false;
        }
    }

    private void FixedUpdate()
    {
        this.Rotate();
    }

    private void Rotate()
    {
        float z = 0f;

        if (this.left)
        {
            z = 1f;
        }
        else if (this.right)
        {
            z = -1f;
        }

        Vector3 forward = this.Camera.transform.forward;
        Vector3 right = this.Camera.transform.right;

        Vector3 desiredMoveDirection = forward;

        Quaternion prevRotation = this.transform.rotation;

        this.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), this.RotationSpeed);
    }
}
