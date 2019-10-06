using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Slider HealthBar;
    public Slider ForceStaminaBar;
    public Slider LightsaberStaminaBar;

    public bool IsDead;

    public float BaseHealth;
    public float BaseForceStamina;
    public float BaseLightsaberStamina;

    private Animator animator;

    private float health;
    private float forceStamina;
    private float lightsaberStamina;

    void Start()
    {
        this.HealthBar.maxValue = this.BaseHealth;
        this.ForceStaminaBar.maxValue = this.BaseForceStamina;
        this.LightsaberStaminaBar.maxValue = this.BaseLightsaberStamina;

        this.IsDead = false;

        this.animator = this.GetComponent<Animator>();

        this.health = this.BaseHealth;
        this.forceStamina = this.BaseForceStamina;
        this.lightsaberStamina = this.BaseLightsaberStamina;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            this.TakeDamage(50f);
        }
    }

    void FixedUpdate()
    {
        this.UpdateBars();

        this.animator.SetBool("IsDead", this.IsDead);
    }

    private void UpdateBars()
    {
        this.HealthBar.value = this.health;
        this.HealthBar.GetComponentInChildren<Text>().text = $"{this.health} / {this.BaseHealth} HP";

        this.ForceStaminaBar.value = this.forceStamina;
        this.ForceStaminaBar.GetComponentInChildren<Text>().text = $"{this.forceStamina} / {this.BaseForceStamina} HP";

        this.LightsaberStaminaBar.value = this.lightsaberStamina;
        this.LightsaberStaminaBar.GetComponentInChildren<Text>().text = $"{this.lightsaberStamina} / {this.BaseLightsaberStamina} HP";
    }

    public void TakeDamage(float damage)
    {
        this.health -= damage;

        this.health = this.health < 0f ? 0f : this.health;

        if (this.health == 0f)
        {
            this.IsDead = true;
        }
    }
}
