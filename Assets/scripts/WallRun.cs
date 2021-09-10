using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    [SerializeField] Transform orientation;
    [Header("wall running")]
    public float wallDistance = 2f;
    public float minJumpHeight = 1.8f;
    public float wallRunGravity = 2f;
    public float wallRunJumpForce = 14f;
    bool wallLeft = false;
    bool wallRight = false;
    bool isSprinting;
    RaycastHit leftHit;
    RaycastHit rightHit;

    //reference
    PlayerMovement playerMovement;

    [Header("Camera")]
    public Camera cam;
    public float fov;
    public float wallFov;
    public float fovTransitionTime;
    public float cameraTilt;
    public float cameraTiltTime;
    
    public float tilt { get; private set; }
    private Rigidbody rb;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
    }
    bool CanWallRun()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight);
    }
    void Checkwall()
    {
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftHit, wallDistance);
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightHit, wallDistance);
    }
    private void Update()
    {
        Checkwall();

        if (CanWallRun())
        {
            if (wallLeft)
            {
                StartWallRun();
            }
            else if (wallRight)
            {
                StartWallRun();
            }
            else
            {
                StopWallRun();
            }
        }
        else
        {
            StopWallRun();
        }
    }
    void StartWallRun()
    {
        rb.useGravity = false;
        rb.AddForce(Vector3.down * wallRunGravity, ForceMode.Force);
        rb.AddForce(playerMovement.isSprinting ? transform.forward * 10f : transform.forward * 6f, ForceMode.Force);
        //rb.AddForce(Vector3.down * 1.5f,ForceMode.Acceleration);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, wallFov, fovTransitionTime * Time.deltaTime);

        if (wallLeft)
        {
            tilt = Mathf.Lerp(tilt, -cameraTilt, cameraTiltTime * Time.deltaTime);
        }
        else if (wallRight)
        {
            tilt = Mathf.Lerp(tilt, cameraTilt, cameraTiltTime * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (wallLeft)
            {
                Vector3 wallRunJumpDirection = (transform.up*1.5f) + leftHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z *1.8f);
                rb.AddForce(wallRunJumpDirection * wallRunJumpForce * 100, ForceMode.Force);
            }
            else if (wallRight)
            {
                Vector3 wallRunJumpDirection = (transform.up*1.5f) + rightHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z*1.8f);
                rb.AddForce(wallRunJumpDirection * wallRunJumpForce * 100, ForceMode.Force);
            }
        }
    }
    void StopWallRun()
    {
        rb.useGravity = true;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov, fovTransitionTime * Time.deltaTime);
        tilt = Mathf.Lerp(tilt, 0, cameraTiltTime * Time.deltaTime);
    }
}
