using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//TODO : Do wallRun
public class WallRunning : MonoBehaviour
{
    [Header("Wall Running Settings")]
    public LayerMask wallLayer;
    public LayerMask groundLayer;
    public float wallRunForce;
    public float wallClimbSpeed;
    public float maxWallRunTime;
    private float _wallRunTimer;

    [Header("Input Sources")]
    public InputActionProperty moveInputSource;
    public InputActionProperty climbInputSource;
    private float _horizontal;
    private float _vertical;
    private bool _upRunning;


    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit _leftWallHit;
    private RaycastHit _rightWallHit;
    private bool _wallLeft;
    private bool _wallRight;

    [Header("References")]
    public Rigidbody playerRb;
    public Transform playerOrientation;
    public SOPerso playerData;

    void Update()
    {
        CheckForWall();
        StateMachine();

        Vector2 moveInput = moveInputSource.action.ReadValue<Vector2>();
        float move1 = moveInput.x;
        float move2 = moveInput.y;
        Debug.Log("x : " + move1);
        Debug.Log("y : " + move2);
    }

    void FixedUpdate()
    {
        if (playerData.wallrunning)
        {
            WallRunningMovement();
        }


    }

    void CheckForWall()
    {
        _wallRight = Physics.Raycast(playerRb.position, playerOrientation.right, out _rightWallHit, wallCheckDistance, wallLayer);
        _wallLeft = Physics.Raycast(playerRb.position, -playerOrientation.right, out _leftWallHit, wallCheckDistance, wallLayer);
        Debug.DrawRay(playerRb.position, playerOrientation.right * wallCheckDistance, Color.red); // Right
        Debug.DrawRay(playerRb.position, -playerOrientation.right * wallCheckDistance, Color.red); // Left
        Debug.Log("Wall Right : " + _wallRight);
        Debug.Log("Wall Left : " + _wallLeft);

    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, groundLayer);
    }

    void StateMachine()
    {
        //Input
        Vector2 moveInput = moveInputSource.action.ReadValue<Vector2>();
        _horizontal = moveInput.x;
        _vertical = moveInput.y;

        _upRunning = climbInputSource.action.IsPressed();

        //State 1 : Wall Running
        if ((_wallRight || _wallLeft) && _vertical > 0.5f && AboveGround())
        {
            if (!playerData.wallrunning)
            {
                Debug.Log("BlaBla");
                StartWallRun();
            }
        }
        else
        {
            if (playerData.wallrunning)
            {
                StopWallRun();
            }
        }
    }

    void StartWallRun()
    {
        playerData.wallrunning = true;
    }

    void WallRunningMovement()
    {
        playerRb.useGravity = false;
        playerRb.velocity = new Vector3(playerRb.velocity.x, 0, playerRb.velocity.z);

        Vector3 wallNormal = _wallRight ? _rightWallHit.normal : _leftWallHit.normal;
        Vector3 wallFoward = Vector3.Cross(wallNormal, transform.up);

        if ((playerOrientation.forward - wallFoward).magnitude > (playerOrientation.forward - -wallFoward).magnitude)
        {
            wallFoward = -wallFoward;
        }

        // Force avant
        playerRb.AddForce(wallFoward * wallRunForce, ForceMode.Force);

        // Force vers le haut
        if (_upRunning)
        {
            playerRb.velocity = new Vector3(playerRb.velocity.x, wallClimbSpeed, playerRb.velocity.z);
        }

        // Push player to the wall
        if (!(_wallLeft && _horizontal > 0) && !(_wallRight && _horizontal < 0))
            playerRb.AddForce(-wallNormal * 100, ForceMode.Force);
    }

    void StopWallRun()
    {
        playerData.wallrunning = false;
    }
}
