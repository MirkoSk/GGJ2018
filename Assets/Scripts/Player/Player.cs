﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class Player : SubscribedBehaviour
{

    #region Variable Declarations
    // Variables that should be visible in Inspector
    [Header("Movement")]
    [SerializeField]
    protected float movementSpeed = 10;
    [SerializeField]
    protected float rotateSpeed = 1000;

    [Header("Properties")]
    [Range(1, 4)]
    [SerializeField] protected int playerNumber;
    public int PlayerNumber { get { return playerNumber; }
        set {
            if (value >= 0 && value <= 4) playerNumber = value;
            else Debug.LogWarning("Trying to access playerNumber out of range. PlayerNumber = " + value);
        }
    }

    // Movement Variables
    protected float horizontalInput;
    protected float verticalInput;
    protected float horizontalLookInput;
    protected float verticalLookInput;
    protected bool active = true;

    // Component References
    protected new Rigidbody rigidbody;
    protected AudioSource audioSource;
    #endregion



    #region Unity Event Functions
    // Use this for initialization
    protected virtual void Start() {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        SetMovable(false);
    }

    protected virtual void FixedUpdate() {
        if (active)
        {
            MoveCharacter();
            RotateCharacter();
        }
    }

    // Update is called once per frame
    protected virtual void Update() {
        if (active)
        {
            horizontalInput = Input.GetAxis(Constants.INPUT_HORIZONTAL + playerNumber) * movementSpeed;
            verticalInput = Input.GetAxis(Constants.INPUT_VERTICAL + playerNumber) * movementSpeed;
            horizontalLookInput = Input.GetAxis(Constants.INPUT_LOOK_HORIZONTAL + playerNumber) * movementSpeed;
            verticalLookInput = Input.GetAxis(Constants.INPUT_LOOK_VERTICAL + playerNumber) * movementSpeed;
        }
    }
    #endregion



    #region Custom Event Functions
    protected override void OnLevelStarted()
    {
        SetMovable(true);
    }
    #endregion



    #region Public Funtcions
    /// <summary>
    /// Sets if the character is allowed to move or not and stops his current movement
    /// </summary>
    /// <param name="movable">Character allowed to move?</param>
    public void SetMovable(bool active) {
        this.active = active;

        if (!active) {
            horizontalInput = 0;
            verticalInput = 0;
        }

        try
        {
            GetComponent<Transmission>().enabled = active;
        }
        catch (System.NullReferenceException)
        {

        }
    }
    #endregion



    #region Private Functions
    /// <summary>
    /// Sets the velocity of the characters Rigidbody component
    /// </summary>
    private void MoveCharacter() {
        Vector3 newVelocity = rigidbody.velocity;
        newVelocity.x = horizontalInput;
        newVelocity.z = verticalInput;
        rigidbody.velocity = newVelocity;
    }

    private void RotateCharacter() {
        if (horizontalLookInput != 0 || verticalLookInput != 0) {
            Vector3 lookDirection = new Vector3(horizontalLookInput, 0f, verticalLookInput);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(lookDirection, Vector3.up), Time.deltaTime * rotateSpeed);
        }
    }
    #endregion
}
