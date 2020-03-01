﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    public Slider HealthBar;
    public Slider ForceStaminaBar;
    public Slider LightsaberStaminaBar;

    public Image GravityOnImage;
    public Image GravityOffImage;

    private Player player;

    void Start()
    {
        this.player = this.GetComponent<Player>();

        this.HealthBar.maxValue = this.player.BaseHealth;
        this.ForceStaminaBar.maxValue = this.player.BaseForceStamina;
        this.LightsaberStaminaBar.maxValue = this.player.BaseLightsaberStamina;
    }

    void FixedUpdate()
    {
        this.UpdateBars();
        this.GravityController();
    }

    private void UpdateBars()
    {
        this.HealthBar.value = this.player.Health;
        //this.HealthBar.GetComponentInChildren<Text>().text = $"{this.player.Health} / {this.player.BaseHealth} HP";

        this.ForceStaminaBar.value = this.player.ForceStamina;
        this.ForceStaminaBar.GetComponentInChildren<Text>().text = $"{this.player.ForceStamina} / {this.player.BaseForceStamina} FSP";

        this.LightsaberStaminaBar.value = this.player.LightsaberStamina;
        this.LightsaberStaminaBar.GetComponentInChildren<Text>().text = $"{this.player.LightsaberStamina} / {this.player.BaseLightsaberStamina} LSP";
    }

    private void GravityController()
    {
        Rigidbody rg = this.player.gameObject.GetComponent<Rigidbody>();

        if (rg.useGravity == true)
        {
            this.GravityOnImage.gameObject.SetActive(true);
            this.GravityOffImage.gameObject.SetActive(false);
        }
        else
        {
            this.GravityOnImage.gameObject.SetActive(false);
            this.GravityOffImage.gameObject.SetActive(true);
        }
    }
}
