﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class MenuAudio : MonoBehaviour
{

    #region Variable Declarations
    [SerializeField] AudioClip confirm;
    [Range(0,1)]
    [SerializeField] float confirmVolume = 1f;
    [SerializeField] AudioSource audioSource;
	#endregion
	
	
	
	#region Unity Event Functions

    #endregion



    #region Public Functions
    public void PlayConfirm() {
        audioSource.PlayOneShot(confirm, confirmVolume);
    }
	#endregion
}
