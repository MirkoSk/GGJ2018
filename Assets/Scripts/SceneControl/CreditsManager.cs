﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rewired;

/// <summary>
/// 
/// </summary>
public class CreditsManager : LevelManager
{

    #region Variable Declarations
    [SerializeField] MusicTrack backgroundTrack = null;
    [SerializeField] Points points = null;
	#endregion
	
	
	
	#region Unity Event Functions
	private void Start()
    {
        StartCoroutine(StartAudioNextFrame());
	}
	
	private void Update()
    {
        if (InputHelper.GetButtonDown(RewiredConsts.Action.UICANCEL) || InputHelper.GetButtonDown(RewiredConsts.Action.UISUBMIT))
        {
            SceneManager.Instance.LoadMainMenu();
        }
	}
    #endregion



    #region Inherited Functions
    protected override void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        points.ResetPoints(true);

        Invoke("RaiseLevelStarted", 3f);
    }

    protected override void RaiseLevelLoaded(float levelStartDelay)
    {
        base.RaiseLevelLoaded(levelStartDelay);
    }

    protected override void RaiseLevelStarted()
    {
        base.RaiseLevelStarted();
    }
    #endregion



    IEnumerator StartAudioNextFrame()
    {
        yield return null;
        AudioManager.Instance.StartTrack(backgroundTrack);
    }
}