using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Animator animator;

    private float h;
    private float v;

    private bool forward;
    private bool backwards;
    private bool left;
    private bool right;

    void Start()
    {
        this.animator = this.gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        this.h = Input.GetAxis("Horizontal");
        this.v = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        this.Move();

        this.AnimationParser();
    }

    private void Move()
    {
        if (this.v > 0)
        {
            this.forward = true;
            this.backwards = false;
        }
        else if (this.v < 0)
        {
            this.backwards = true;
            this.forward = false;
        }
        else
        {
            this.forward = false;
            this.backwards = false;
        }

        if (this.h > 0)
        {
            this.right = true;
            this.left = false;
        }
        else if (this.h < 0)
        {
            this.left = true;
            this.right = false;
        }
        else
        {
            this.left = false;
            this.right = false;
        }
    }

    private void AnimationParser()
    {
        this.animator.SetBool("Forward", this.forward);
        this.animator.SetBool("Backwards", this.backwards);
        this.animator.SetBool("Left", this.left);
        this.animator.SetBool("Right", this.right);
    }
}
