﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[CreateAssetMenu(menuName = "Scriptable Objects/Abilities/Tank/Default")]
public class Tank : Ability
{

    #region Variable Declarations
    // Serialized Fields

    // Private

    #endregion



    #region Public Properties

    #endregion



    #region Public Functions
    public override void Tick(float deltaTime, bool abilityButtonPressed)
    {
        base.Tick(deltaTime, abilityButtonPressed);

        if (abilityActive && (!abilityButtonPressed || energyPoolRecharging))
        {
            DeactivateShield();
        }
    }

    public override void TriggerAbility()
    {
        hero.Shield.SetActive(true);
        hero.Rigidbody.mass = 100f;
        audioSource.PlayOneShot(soundClip, volume);

        abilityActive = true;
    }

    public void DeactivateShield()
    {
        hero.Shield.SetActive(false);
        hero.Rigidbody.mass = 1f;
        abilityActive = false;
    }
    #endregion



    #region Private Functions

    #endregion
}

