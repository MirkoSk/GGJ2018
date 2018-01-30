﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles everything related to the movement of Haru, our playable Character
/// </summary>
public class Hero : Player {

    #region Variable Declarations
    // Variables that should be visible in Inspector
    [Header("Damage")]
    [SerializeField] protected int damagePerShot = 10;
    [SerializeField] protected float attackCooldown = 0.2f;
    [SerializeField] protected float projectileSpeed = 10f;

    [Header("Tank")]
    [SerializeField] protected float defendCooldown = 3f;
    [SerializeField] protected float defendDuration = 2f;

    [Header("Opfer")]
    [Tooltip("Proportional to base movement speed")]
    [SerializeField] protected float speedBoost = 0.5f;

    [Header("Properties")]
    [SerializeField] protected PlayerColor playerColor;
    public PlayerColor PlayerColor { get { return playerColor; } set { playerColor = value; } }
    public Ability ability;

    [Header("Sound")]
    [SerializeField]
    protected AudioClip wobbleBobbleSound;
    [Range(0, 1)]
    [SerializeField]
    protected float wobbleBobbleVolume = 1f;
    [SerializeField] protected AudioClip attackSound;
    [Range(0, 1)]
    [SerializeField]
    protected float attackSoundVolume = 1f;

    [Header("References")]
    [SerializeField]
    protected GameObject projectilePrefab;
    [SerializeField] protected GameObject wobbleBobble;
    public SpriteRenderer healthIndicator;
    [SerializeField] protected SpriteRenderer cooldownIndicator;
    public SpriteRenderer CooldownIndicator { get { return cooldownIndicator; } }
    [SerializeField] protected Sprite[] defendCooldownSprites;
    public Sprite TankSprite { get { return defendCooldownSprites[defendCooldownSprites.Length - 1]; } }
    [SerializeField] protected Sprite damageSprite;
    public Sprite DamageSprite { get { return damageSprite; } }
    [SerializeField] protected Sprite opferSprite;
    public Sprite OpferSprite { get { return opferSprite; } }
    [SerializeField] protected Renderer playerMeshRenderer;
    [SerializeField] protected Material greenPlayerMat;
    [SerializeField] protected Material redPlayerMat;
    [SerializeField] protected Material bluePlayerMat;

    protected bool cooldown = true;
    protected Coroutine resetDefendCoroutine;

    public int PlayerNumber { get { return playerNumber; } }
    #endregion



    #region Unity Event Functions
    override protected void Start() {
        base.Start();

        // Set the correct Ability Sprite
        if (ability == Ability.Damage) cooldownIndicator.sprite = DamageSprite;
        else if (ability == Ability.Opfer) cooldownIndicator.sprite = OpferSprite;
        else if (ability == Ability.Tank) cooldownIndicator.sprite = TankSprite;

        // Set the correct colors
        if (playerColor == PlayerColor.Blue) {
            playerMeshRenderer.material = bluePlayerMat;
            cooldownIndicator.color = GameManager.Instance.BluePlayerColor;
            healthIndicator.color = GameManager.Instance.BluePlayerColor;
        }
        else if (playerColor == PlayerColor.Green) {
            playerMeshRenderer.material = greenPlayerMat;
            cooldownIndicator.color = GameManager.Instance.GreenPlayerColor;
            healthIndicator.color = GameManager.Instance.GreenPlayerColor;
        }
        else if (playerColor == PlayerColor.Red) {
            playerMeshRenderer.material = redPlayerMat;
            cooldownIndicator.color = GameManager.Instance.RedPlayerColor;
            healthIndicator.color = GameManager.Instance.RedPlayerColor;
        }
    }

    override protected void Update() {
        base.Update();

        HandleAbilities();
    }
    #endregion



    #region Public Funtcions
    /// <summary>
    /// Cancels the ResetDefend Coroutine. Gets called when a transmission happens during reset of the defend ability.
    /// </summary>
    public void CancelDefendReset() {
        if (resetDefendCoroutine != null) StopCoroutine(resetDefendCoroutine);
        wobbleBobble.SetActive(false);
        cooldown = true;
    }
    #endregion



    #region Private Functions
    private void HandleAbilities() {
        if (cooldown) {
            if (ability == Ability.Opfer) {
                Run();
            }
            else if (ability == Ability.Damage && Input.GetButton(Constants.INPUT_ABILITY + playerNumber)) {
                Attack();
            }
            else if (ability == Ability.Tank && Input.GetButtonDown(Constants.INPUT_ABILITY + playerNumber)) {
                Defend();
            }
        }
    }

    private void Attack() {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
        projectile.GetComponent<HeroProjectile>().damage = damagePerShot;
        projectile.GetComponent<HeroProjectile>().playerColor = playerColor;
        projectile.GetComponent<Rigidbody>().velocity = transform.forward * projectileSpeed;

        audioSource.PlayOneShot(attackSound, attackSoundVolume);

        cooldown = false;
        StartCoroutine(ResetAttackCooldown());
    }

    private void Defend() {
        wobbleBobble.SetActive(true);
        cooldown = false;
        cooldownIndicator.sprite = defendCooldownSprites[0];
        audioSource.PlayOneShot(wobbleBobbleSound, wobbleBobbleVolume);
        resetDefendCoroutine = StartCoroutine(ResetDefend());
    }

    private void Run() {
        horizontalInput *= (speedBoost + 1);
        verticalInput *= (speedBoost + 1);
    }
    #endregion



    #region Coroutines
    IEnumerator ResetAttackCooldown() {
        yield return new WaitForSecondsRealtime(attackCooldown);
        cooldown = true;
    }

    IEnumerator ResetDefend() {
        // Wait for defend duration and turn of wobbleBobble
        yield return new WaitForSecondsRealtime(defendDuration);
        wobbleBobble.SetActive(false);

        // Start Cooldown and update CooldownIndicator
        for (float i = 0; i < defendCooldown; i += Time.deltaTime) {
            yield return null;
            cooldownIndicator.sprite = defendCooldownSprites[Mathf.FloorToInt((i / defendCooldown) * defendCooldownSprites.Length)];
        }

        // Reset Cooldown
        cooldown = true;
        resetDefendCoroutine = null;
    }
    #endregion
}
