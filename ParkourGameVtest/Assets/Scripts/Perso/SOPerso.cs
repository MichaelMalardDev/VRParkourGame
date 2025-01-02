using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "SOPerso", menuName = "ScriptableObjects/SOPerso", order = 1)]
public class SOPerso : ScriptableObject
{
    [Header("Movement Settings")]
    public float speed = 2f; // Vitesse de déplacement du personnage
    public float baseSpeed = 2.0f; // Vitesse de déplacement de base du personnage
    public float speedWhenRunning = 4.0f; // Vitesse de déplacement du personnage quand il court

    public float jumpHeight = 1.5f; // Force de saut du personnage
    public bool onlyMoveWhenGrounded = false;
    public float handSpeedToRun = 3.0f;
    public bool isGrounded;

    [Header("Wall Running Settings")]
    public float wallRunSpeed;
    public bool wallrunning;

    [Header("Turn Settings")]
    public float turnSpeed = 60.0f; // Vitesse de rotation du personnage

    [Space]
    [Header("Jump with Hand Settings")]
    public bool jumpWithHand = true;
    public float minJumpWithHandSpeed = 2;
    public float maxJumpWithHandSpeed = 6;

    [Space]
    [Header("Climbing Settings")]
    public bool leftClimb = false;
    public bool rightClimb = false;

    [Space]
    [Header("Health Settings")]
    public float health = 5f; // Points de vie du personnage
    public float maxHealth = 5f; // Points de vie maximum du personnage
    public float attackDamage = 10.0f; // Dégâts infligés par le personnage
    public bool isDead = false;
    public bool newGame = true;
    public bool firstGame = true;
    public bool inTutorial = true;
    private Vector3 _respawnPoint;
    public Vector3 respawnPoint { get => _respawnPoint; set => _respawnPoint = value; }

    [Space]
    [Header("Body Collider")]
    public float bodyHeightMin = 0.5f;
    public float bodyHeightMax = 2f;

    [Space]
    [Header("Zone")]
    public bool inZone = false;

    [Space]
    [Header("LeaderBoard")]
    public float distance = 0f;
    private float _distanceScore;
    public float distanceScore { get => _distanceScore; set => _distanceScore = value; }
    public float totalDistance;
    public float time = 0f;
    private float _timeScore;
    public float timeScore { get => _timeScore; set => _timeScore = value; }
    public float totalTime;

    [Space]
    [Header("Vignette")]
    public bool vignette = false;
    private Vignette _vignetteProfile;
    public Vignette vignetteProfile { get => _vignetteProfile; set => _vignetteProfile = value; }



    public void Initialize()
    {
        speed = baseSpeed;
        leftClimb = false;
        rightClimb = false;
        wallrunning = false;
        health = maxHealth;
        isDead = false;
        inZone = false;
        distance = 0f;
        time = 0f;
        newGame = true;
    }

    public void InitializeScore()
    {
        distanceScore = 0f;
        timeScore = 0f;
    }

    public void ChangeRotaionSpeed(float value)
    {
        turnSpeed = value;
    }

    public void ChangeJumpWithHand()
    {
        jumpWithHand = !jumpWithHand;
    }

    public void ChangeJumpWithHandSpeed(float value)
    {
        minJumpWithHandSpeed = value;
    }

    public void ChangeVignette()
    {
        vignette = !vignette;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            isDead = true;
        }
    }


}
