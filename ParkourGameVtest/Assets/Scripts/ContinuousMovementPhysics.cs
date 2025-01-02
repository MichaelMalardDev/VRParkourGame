using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ContinuousMovementPhysics : MonoBehaviour
{
    public SOPerso playerData;
    public float smoothingFactor = 0.01f;
    float smoothedHandSpeed = 0; // Initialize the variable with a default value
    // public float speed = 1;
    // public float turnSpeed = 60;
    private float _jumpVelocity = 7;
    // public float jumpHeight = 1.5f;
    // public bool onlyMoveWhenGrounded = false;
    // public bool jumpWithHand = true;

    [Header("Input Sources")]
    public InputActionProperty moveInputSource;
    public InputActionProperty turnAxisSource;
    public InputActionProperty jumpInputSource;

    [Space]
    [Header("Rigidbodies")]

    public Rigidbody playerRb;
    public Rigidbody leftHandRb;
    public Rigidbody rightHandRb;
    public Rigidbody headRb;

    [Space]
    [Header("Slidding")]
    public PhysicMaterial noFrictionMaterial;
    public float heigthMinToSlide = 0.5f;
    public float vitesseToSlide = 5f;

    [Space]
    [Header("Sounds")]
    public AudioSource audioSource;
    public AudioClip landingSound;
    public AudioClip movingSound;
    public float walkingSpeedThreshold;
    public float sprintPitchMultiplier;

    [Space]
    [Header("Autres")]
    public LayerMask groundLayer;
    public VolumeProfile volumeProfile;
    private Vignette vignette;

    public Transform directionSource;
    public Transform turnSource;
    public CapsuleCollider bodyCollider;
    public float threshold;
    public AnimateFade animateFade;
    private Vector2 _inputMoveAxis;
    private float _inputTrunAxis;
    private bool _isAnimatingFade = false;

    private bool wasJumping = false;

    void Awake()
    {
        if (!volumeProfile) throw new System.NullReferenceException(nameof(VolumeProfile));
        if (!volumeProfile.TryGet(out vignette)) throw new System.NullReferenceException(nameof(vignette));

        playerData.vignetteProfile = vignette;
    }

    void Start()
    {
        ResetVelocity();
        vignette.intensity.value = 0;
    }

    // Update is called once per frame
    void Update()
    {

        _inputMoveAxis = moveInputSource.action.ReadValue<Vector2>();
        _inputTrunAxis = turnAxisSource.action.ReadValue<Vector2>().x;
        bool jumpInput = jumpInputSource.action.WasPressedThisFrame();





        if (playerData.newGame) return;

        float handSpeed = ((leftHandRb.velocity - playerRb.velocity).magnitude + (rightHandRb.velocity - playerRb.velocity).magnitude) / 2;


        if (!playerData.jumpWithHand)
        {
            if (jumpInput && playerData.isGrounded)
            {
                _jumpVelocity = Mathf.Sqrt(-2 * Physics.gravity.y * playerData.jumpHeight);
                playerRb.velocity = Vector3.up * _jumpVelocity;
            }
        }
        else    //if (playerData.jumpWithHand)
        {
            bool inputJumpReleased = jumpInputSource.action.WasReleasedThisFrame();

            if (inputJumpReleased && playerData.isGrounded && handSpeed > playerData.minJumpWithHandSpeed)
            {
                _jumpVelocity = Mathf.Sqrt(-2 * Physics.gravity.y * playerData.jumpHeight);
                playerRb.velocity = Vector3.up * _jumpVelocity;
                // playerRb.velocity = Vector3.up * Mathf.Clamp(handSpeed, playerData.minJumpWithHandSpeed, playerData.maxJumpWithHandSpeed);
            }

        }




        smoothedHandSpeed = Mathf.Lerp(smoothedHandSpeed, handSpeed, Time.deltaTime * smoothingFactor);

        // TOTEST[x]: Run when hand speed is greater than a certain value
        if (smoothedHandSpeed >= playerData.handSpeedToRun)
        {
            playerData.speed = playerData.speedWhenRunning;
        }
        else if (smoothedHandSpeed < playerData.handSpeedToRun)
        {
            playerData.speed = playerData.baseSpeed;
        }

        if (playerData.wallrunning)
        {
            playerData.speed = playerData.wallRunSpeed;
        }

        // Debug.Log("Player Speed: " + playerRb.velocity.magnitude);

        float smoothedPlayerSpeed = Mathf.Lerp(playerRb.velocity.magnitude, playerData.speed, Time.deltaTime * smoothingFactor);

        // Vigette
        if (playerData.vignette)
        {
            if (smoothedPlayerSpeed > 0.1f)
            {
                vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, 1f, Time.deltaTime * 4);
                // vignette.intensity.value = 0.5f;
            }
            else
            {
                vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, 0, Time.deltaTime * 3);
                // vignette.intensity.value = 0;
            }
        }
    }

    void ManageSound(bool jumpInput)
    {
        // Vérifier si le personnage était en l'air lors du dernier cadre et est maintenant au sol
        if (wasJumping && !playerData.isGrounded)
        {
            // Le personnage vient d'atterrir
            wasJumping = false;

            // Jouer le son de landing
            if (landingSound != null)
            {
                Debug.Log("Landing");
                audioSource.PlayOneShot(landingSound);
            }
        }

        // Mettre à jour l'état de saut précédent
        wasJumping = !playerData.isGrounded && jumpInput;
    }

    void ResetVelocity()
    {
        playerRb.velocity = Vector3.zero;
        leftHandRb.velocity = Vector3.zero;
        rightHandRb.velocity = Vector3.zero;
        headRb.velocity = Vector3.zero;
    }

    void CheckForRespawn()
    {
        if (playerRb.position.y < threshold && !_isAnimatingFade)
        {
            Debug.Log("Respawn");
            _isAnimatingFade = true;
            animateFade.RespawnFade();
            Invoke(nameof(ResetPos), 1f);
        }
    }

    void ResetPos()
    {
        playerRb.MovePosition(playerData.respawnPoint);
        playerRb.velocity = Vector3.zero;

        leftHandRb.MovePosition(playerData.respawnPoint);
        leftHandRb.velocity = Vector3.zero;

        rightHandRb.MovePosition(playerData.respawnPoint);
        rightHandRb.velocity = Vector3.zero;

        headRb.MovePosition(playerData.respawnPoint);
        headRb.velocity = Vector3.zero;

        _isAnimatingFade = false;
        animateFade.OnFadeComplete();
    }

    void FixedUpdate()
    {
        // if (!playerData.isGrounded)
        // {
            playerData.isGrounded = CheckIfGrounded();
        // }

        if (_isAnimatingFade == false) CheckForRespawn();



        if ((playerData.isGrounded && playerData.onlyMoveWhenGrounded) || !playerData.onlyMoveWhenGrounded)
        {
            Quaternion yaw = Quaternion.Euler(0, directionSource.eulerAngles.y, 0);
            Vector3 direction = yaw * new Vector3(_inputMoveAxis.x, 0, _inputMoveAxis.y);

            Vector3 targetMovePosition = playerRb.position + direction * Time.fixedDeltaTime * playerData.speed;

            //Rotation
            float angle;
            Quaternion q;

            Vector3 axis = Vector3.up;

            angle = playerData.turnSpeed * Time.fixedDeltaTime * _inputTrunAxis;
            q = Quaternion.AngleAxis(angle, axis);
            playerRb.MoveRotation(playerRb.rotation * q);



            if (playerData.newGame) return;
            Vector3 newPosition = q * (targetMovePosition - turnSource.position) + turnSource.position;

            playerRb.MovePosition(newPosition);

            // Debug.Log("playerRb hauteur: " + headRb.position.y);
            // Debug.Log("playerRb vitesse: " + playerRb.velocity.magnitude);

            if (headRb.position.y <= heigthMinToSlide && playerRb.velocity.magnitude >= vitesseToSlide)
            {
                bodyCollider.material = noFrictionMaterial;
            }
            else
            {
                bodyCollider.material = null;
            }
        }

    }

    public bool CheckIfGrounded()
    {
        Debug.Log("CheckIfGrounded");
        Vector3 start = bodyCollider.transform.TransformPoint(bodyCollider.center);
        float rayLenght = bodyCollider.height / 2 - bodyCollider.radius + 0.05f;

        bool hasHit = Physics.SphereCast(start, bodyCollider.radius, Vector3.down, out RaycastHit hitInfo, rayLenght, groundLayer);

        return hasHit;
    }

    /// <summary>
    /// Callback to draw gizmos that are pickable and always drawn.
    /// </summary>
    void OnDrawGizmos()
    {
        Vector3 start = bodyCollider.transform.TransformPoint(bodyCollider.center);
        float rayLenght = bodyCollider.height / 2 - bodyCollider.radius + 0.05f;

        if(Physics.SphereCast(start, bodyCollider.radius, Vector3.down, out RaycastHit hitInfo, rayLenght, groundLayer))
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(hitInfo.point, 0.1f);
        }
        else
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(start + Vector3.down * rayLenght, 0.1f);
        }
    }

}
