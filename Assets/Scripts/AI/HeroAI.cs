﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Handles everything related to the movement of Haru, our playable Character
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class HeroAI : Hero {

    #region Variable Declarations
    [Header("AI Parameters")]
    [SerializeField] float repathingDistance = 5f;
    [SerializeField] float randomness = 5f;
    [SerializeField] float shieldDelay = 2f;
    [Range(0,1)]
    [SerializeField] float tankFollowSpeed = 0.5f;
    [Range(0,5)]
    [SerializeField] float tankTargetDistance = 1.5f;
    [Range(0,1)]
    [SerializeField] float damageCornerPeek = 0.2f;
    [SerializeField] LayerMask attackRayMask;

    NavMeshAgent agent;
    //Transform homingMissile;
    Transform boss;
    Transform damage;
    //Transform tank;
    //Transform opfer;
    List<Transform> corners = new List<Transform>();
    List<Transform> middleTargets = new List<Transform>();
    List<Transform> allAITargets = new List<Transform>();
    int currentlyTargetedCorner;
    float randomnessTimer;
    float shieldDelayTimer;
    float normalAgentSpeed;
    #endregion



    #region Unity Event Functions
    override protected void Start() {
        base.Start();
        
        // Get references
        agent = GetComponent<NavMeshAgent>();
        //homingMissile = GameObject.FindGameObjectWithTag(Constants.TAG_HOMING_MISSILE).transform;
        boss = GameObject.FindGameObjectWithTag(Constants.TAG_BOSS).transform.parent;

        GameObject[] friends = GameObject.FindGameObjectsWithTag(Constants.TAG_HERO);
        foreach (GameObject go in friends)
        {
            if (go.transform.parent.GetComponent<Hero>() == null) continue;
            if (go.transform.parent.GetComponent<Hero>().ability == Ability.Damage) damage = go.transform;
            //if (go.transform.parent.GetComponent<Hero>().ability == Ability.Tank) tank = go.transform;
            //if (go.transform.parent.GetComponent<Hero>().ability == Ability.Opfer) opfer = go.transform;
        }

        GameObject[] cornersGO = GameObject.FindGameObjectsWithTag(Constants.TAG_AI_CORNER);
        foreach (GameObject go in cornersGO)
        {
            corners.Add(go.transform);
        }

        GameObject[] middleTargetsGO = GameObject.FindGameObjectsWithTag(Constants.TAG_AI_MIDDLE);
        foreach (GameObject go in middleTargetsGO)
        {
            middleTargets.Add(go.transform);
        }
        
        for (int i = 0; i < corners.Count + middleTargets.Count; i++)
        {
            if (i < corners.Count) allAITargets.Add(corners[i].transform);
            else allAITargets.Add(middleTargets[i - corners.Count].transform);
        }

        normalAgentSpeed = agent.speed;
    }

    new private void FixedUpdate()
    {
        if (active)
        {
            CalculateMovement();
            HandleAbilities();
        }
    }

    new private void Update()
    {
        if (active)
        {
            randomnessTimer += Time.deltaTime;
            if (cooldown) shieldDelayTimer += Time.deltaTime;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position + agent.velocity * (repathingDistance / agent.speed), 1f);


        for (int i = 0; i < agent.path.corners.Length; i++)
        {
            if (i < agent.path.corners.Length - 1) Gizmos.DrawLine(agent.path.corners[i], agent.path.corners[i + 1]);
            Gizmos.DrawWireSphere(agent.path.corners[i], 0.3f);
        }
    }
    #endregion



    #region Public Funtcions

    #endregion



    #region Private Functions
    private void CalculateMovement()
    {
        if (ability == Ability.Opfer)
        {
            if (agent.destination == transform.position) SetDestination(GetRandomTarget());

            if (agent.remainingDistance < repathingDistance)
            {
                if (randomnessTimer >= randomness)
                {
                    SetDestination(GetRandomMiddle());
                    randomnessTimer = 0;
                }
                else SetDestination(GetNextCorner());
            }
        }
        else if (ability == Ability.Damage)
        {
            // Move
            SetDamageDestination();

            // Rotate
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(boss.position - transform.position, Vector3.up), Time.deltaTime * rotateSpeed);
        }
        else if (ability == Ability.Tank)
        {
            SetTankDestination();
        }
    }

    private void HandleAbilities()
    {
        if (ability == Ability.Opfer)
        {
            Run();
        }
        else if (ability == Ability.Damage && cooldown)
        {
            Ray ray = new Ray(transform.position + Vector3.up * 0.5f, transform.forward);
            RaycastHit hitInfo;

            Debug.DrawRay(ray.origin, ray.direction * 70f, Color.red);
            if (Physics.Raycast(ray, out hitInfo, 70f, attackRayMask)/* && hitInfo.transform == boss*/)
            {
                Debug.DrawRay(ray.origin, hitInfo.point, Color.green);
                Attack();
            }
        }
        else if (ability == Ability.Tank)
        {
            if (shieldDelayTimer >= shieldDelay)
            {
                Defend();
                shieldDelayTimer = 0;
            }
        }
    }

    #region AI Methods
    private void SetDamageDestination()
    {
        // Calculate path to boss
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, boss.position, agent.areaMask, path);
        if (agent.destination == transform.position && path.corners.Length > 2)
        {
            Vector3 destination = path.corners[path.corners.Length - 2] + (boss.position - path.corners[path.corners.Length - 2]) * damageCornerPeek;
            SetDestination(destination);
        }

    }

    private void SetTankDestination() {
        Vector3 nearDamage = damage.position + (transform.position - damage.position).normalized * tankTargetDistance;
        SetDestination(nearDamage + (transform.position - nearDamage) * tankFollowSpeed);
    }

    private void SetDestination(Vector3 destination) {
        Vector3 start = transform.position + agent.velocity * (repathingDistance / agent.speed);

        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(start, destination, agent.areaMask, path);

        agent.SetPath(path);
    }

    private Vector3 GetRandomMiddle()
    {
        return middleTargets[Random.Range(0, middleTargets.Count)].position;
    }

    private Vector3 GetRandomTarget() {
        return allAITargets[Random.Range(0, allAITargets.Count)].position;
    }

    /// <summary>
    /// Returns the position of the closest corner of the level and (0,0,0) if no corner is found.
    /// </summary>
    /// <returns></returns>
    private Vector3 GetClosestCorner() {
        float closestDistance = 100f;
        Vector3 closestCorner = Vector3.zero;
        for (int i = 0; i < corners.Count; i++)
        {
            if (Vector3.Distance(corners[i].position, transform.position) < closestDistance)
            {
                closestCorner = corners[i].position;
                currentlyTargetedCorner = i;
            }
        }
        return closestCorner;
    }

    private Vector3 GetNextCorner() {
        if (currentlyTargetedCorner < corners.Count - 1)
        {
            currentlyTargetedCorner++;
            return corners[currentlyTargetedCorner].position;
        } else
        {
            currentlyTargetedCorner = 0;
            return corners[currentlyTargetedCorner].position;
        }
    }
    #endregion

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
        agent.speed = normalAgentSpeed * (speedBoost + 1);
        agent.speed = normalAgentSpeed * (speedBoost + 1);
    }
    #endregion
}
