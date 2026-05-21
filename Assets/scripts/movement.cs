using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float groundAcceleration = 16f;
    public float airAcceleration = 7f;
    public float idleDeceleration = 10f;
    
    public float groundDrag;

    [Header("Jump")]
    public float jumpForce;
    public float jumpCoolDown;
    public float airMutiplier;
    public float coyoteTime = 0.12f;
    public float jumpBufferTime = 0.12f;
    bool readyToJump;

    [Header("keybinds")]
    public KeyCode jumpKey = KeyCode.Space;



    [Header("ground check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontallInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;
    float lastGroundedTime;
    float lastJumpPressedTime = -10f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        readyToJump = true;
    }


    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        if (grounded)
        {
            lastGroundedTime = Time.time;
        }

        MyInput();
        SpeedCon();


        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

    }

    private void FixedUpdate()
    {
        MovePlayer();
    }
    private void MyInput()
    {
        horizontallInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(jumpKey))
        {
            lastJumpPressedTime = Time.time;
        }

        bool canUseBufferedJump = Time.time - lastJumpPressedTime <= jumpBufferTime;
        bool canUseCoyoteTime = Time.time - lastGroundedTime <= coyoteTime;

        if (canUseBufferedJump && readyToJump && canUseCoyoteTime)
        {
            readyToJump = false;
            lastJumpPressedTime = -10f;
            Jump();
            Invoke(nameof(ResetJump), jumpCoolDown);
        }

    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontallInput;
        Vector3 desiredDirection = moveDirection.sqrMagnitude > 0.01f ? moveDirection.normalized : Vector3.zero;
        float acceleration = grounded ? groundAcceleration : airAcceleration;

        if (desiredDirection != Vector3.zero)
        {
            float multiplier = grounded ? 1f : airMutiplier;
            rb.AddForce(desiredDirection * acceleration * multiplier, ForceMode.Acceleration);
        }
        else if (grounded)
        {
            Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(-flatVelocity * idleDeceleration, ForceMode.Acceleration);
        }

    }

    private void SpeedCon()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized*moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

}
