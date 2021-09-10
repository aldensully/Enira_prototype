using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    float playerHeight = 2f;
    float playerHeightCrouched = 1f;
    [Header("movement")]
    public float moveSpeed = 6f;
    public float moveMult = 10f;
    public float airMult = 0.4f;
    float horizontalMovement;
    float verticalMovement;
    public float drag = 6f;
    public float airDrag = 1f;
    public float jumpForce = 15f;
    public float crouchTime = 10f;
    public float slideSpeed = 10f;
    [SerializeField] float walkSpeed = 8f;
    [SerializeField] float sprintSpeed = 12f;
    [SerializeField] float acceleration = 10f;
    
    public bool isSprinting = false;
    bool isCrouched = false;
    bool isSliding = false;
    bool isGrounded;

    //references
    Rigidbody rb;
    public CapsuleCollider playerCapsule;
    public Camera cam;
    CameraController camController;
    [Header("keybinds")]
    KeyCode jumpkey = KeyCode.Space;
    KeyCode sprintKey = KeyCode.LeftShift;
    KeyCode crouch = KeyCode.LeftControl;

    [SerializeField] LayerMask groundMask;
    [SerializeField] Transform groundCheck;
    Vector3 moveDirection;
    Vector3 slopeMoveDirection;

    RaycastHit slopeHit;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        camController = GetComponent<CameraController>();
    }
    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.4f, groundMask);
        MyInput();
        ControlDrag();
        ControlSpeed();
        if (Input.GetKeyDown(jumpkey) && isGrounded) Jump();
        if (Input.GetKeyDown(crouch)) HandleCrouchDown();
        else if (Input.GetKeyUp(crouch)) HandleCrouchUp();
        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
    }
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.5f)) return true;
        if (slopeHit.normal != Vector3.up) return true;
        else return false;
    }

    void ControlSpeed()
    {
        if(isGrounded && Input.GetKey(sprintKey))
        {
            moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceleration * Time.deltaTime);
            isSprinting = true;
        }
        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);
            isSprinting = false;
        }
    }
    void Jump()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }
    
    
    void HandleCrouchDown()
    {
        if(isGrounded && isSprinting)
        {
            //slide
            Slide();
        }
        else if(isGrounded)
        {
            //crouch
            playerCapsule.height = playerHeightCrouched;
        }
    }
    void HandleCrouchUp()
    {
        isSliding = false;
        playerCapsule.height = playerHeight;
        camController.TiltReset();
    }
    void Slide()
    {
        camController.Tilt(20f);
        isSliding = true;
        playerCapsule.height = playerHeightCrouched;
        rb.AddForce(transform.forward * slideSpeed, ForceMode.Force);
    }

    void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");
        moveDirection = transform.forward * verticalMovement + transform.right * horizontalMovement;
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }
    void ControlDrag()
    {
        rb.drag = isGrounded ? drag : airDrag;
    }
    void MovePlayer()
    {
        if (isGrounded && !OnSlope())
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * moveMult, ForceMode.Acceleration);
        }
        else if(isGrounded && OnSlope())
        {
            rb.AddForce(slopeMoveDirection.normalized * moveSpeed * moveMult, ForceMode.Acceleration);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * airMult, ForceMode.Acceleration);
        }
    }
}
