﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 
/// </summary>
public class MainMenu : MonoBehaviour
{

    #region Variable Declarations
    [SerializeField] GameObject playButton = null;

    List<InputHelper.PlayerRuleSet> ruleSets = new List<InputHelper.PlayerRuleSet>();
	#endregion
	
	
	
	#region Unity Event Functions
	private void Start()
    {
        ruleSets = InputHelper.ChangeRuleSetForAllPlayers(RewiredConsts.LayoutManagerRuleSet.RULESETMENU);
	}

    private void OnDisable()
    {
        AudioManager.Instance.StopPlaying();
        InputHelper.ChangeRuleSetForPlayers(ruleSets);
    }
    #endregion



    #region Public Functions
    public void LoadFirstLevel()
    {
        SceneManager.Instance.LoadNextScene();
    }

    public void LoadTutorial()
    {
        SceneManager.Instance.LoadTutorial();
    }

    public void LoadCredits()
    {
        SceneManager.Instance.LoadCredits();
    }

    public void ExitGame()
    {
        SceneManager.Instance.ExitGame();
    }
    #endregion
}
