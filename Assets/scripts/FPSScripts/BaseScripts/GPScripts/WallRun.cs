using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class WallRun : MonoBehaviour //HATES BOX COLLIDERS
{
    [SerializeField] PlayerMovement pM;
    [SerializeField] dashScript ds;

    [SerializeField] Transform orientation;

    [SerializeField] float wallDistance;
    [SerializeField] float minimumJumpHeight = 1.6f;

    [SerializeField] float wallRunGravity;
    [SerializeField] float wallRunJumpForce, wallRunForward;

    [SerializeField] CinemachineVirtualCamera Camera;
    //private float wallRunfov; //felt best at 92
    public float wallRunfovTime;
    [SerializeField] float camTilt;
    [SerializeField] float camTiltTime;
    [SerializeField] LayerMask groundMask;

    public bool isWRunning;

    public float Tilt { get; set;}

    public bool wallLeft = false;
    public bool wallRight = false;

    RaycastHit leftWallHit;
    RaycastHit rightWallHit;

    private Rigidbody rb;

    public bool wRunningAllowed = true;
    public float holdDrag;
    private Vector3 startOrient;
    Vector3 wallRunJumpDirection;
    private bool wallForce, startForce;
    public bool camReset, wallJump;

    [SerializeField] float cd;
    public float cdTimer;

    [SerializeField] FMODUnity.StudioEventEmitter Aud;

    private Vector3 wallAngle;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        holdDrag = pM.groundDrag;
        cdTimer = 0;
    }

    public bool CanWallRun()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minimumJumpHeight);
    }

    void CheckWall()
    {
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallDistance, groundMask);
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallDistance, groundMask);
    }

    private void Update()
    {
        if (!isWRunning)
        {
            cdTimer -= Time.deltaTime;
            if(camReset)
            {
                if (Camera.m_Lens.FieldOfView != pM.fov && !ds.isDashing)
                {
                    Camera.m_Lens.FieldOfView = Mathf.Lerp(Camera.m_Lens.FieldOfView, pM.fov, wallRunfovTime * Time.deltaTime);
                }
                Tilt = Mathf.Lerp(Tilt, 0, camTiltTime * Time.deltaTime);

                if(Mathf.Abs(Tilt) <= 0)
                {
                    camReset = false;
                }
            }
        }

        if (!pM.isSliding)
        {
            CheckWall();

            if (CanWallRun() && cdTimer <= 0) 
            {
                if (wallLeft)
                {
                    // wallAngle = leftWallHit
                    StartWallRun();
                }
                else if (wallRight)
                {
                    StartWallRun();
                }
                else if (isWRunning)
                {
                    StopWallRun();
                }
            }
            else
                Tilt = Mathf.Lerp(Tilt, 0, camTiltTime * Time.deltaTime);
        }
    }

    public void StopWallRun()
    {
        cdTimer = cd;
        pM.groundDrag = holdDrag;
        rb.useGravity = true;
        pM.canSlam = true;
        camReset = true;
        wallForce = false;
        isWRunning = false;
        if(Aud.IsPlaying())
        {
            Aud.Stop();
        }
    }

    void StartWallRun()
    {
        isWRunning = true;
        pM.groundDrag = 0f;
        camReset = false;
        rb.useGravity = false;
        startOrient = orientation.forward;
        wallForce = true;
        startForce = true;

        //Camera.m_Lens.FieldOfView = Mathf.Lerp(Camera.m_Lens.FieldOfView, pM.fov + 3, wallRunfovTime * Time.deltaTime);

        pM.canSlam = false;

        if (wallLeft && Mathf.Abs(rb.velocity.magnitude) > 5)
        {
            if (!Aud.IsPlaying())
            {
                Aud.EventInstance.setParameterByName("Panning", -40);
                Aud.Play();
            }
            Tilt = Mathf.Lerp(Tilt, -camTilt, camTiltTime * Time.deltaTime);
        }
        else if(wallRight)
        {
            if (!Aud.IsPlaying() && Mathf.Abs(rb.velocity.magnitude) > 5)
            {
                Aud.EventInstance.setParameterByName("Panning", 40);
                Aud.Play();
            }
            Tilt = Mathf.Lerp(Tilt, camTilt, camTiltTime * Time.deltaTime);
        }

        if(Input.GetButtonDown("Jump"))
        {
            if (wallLeft)
            {
                wallRunJumpDirection = (transform.up * 0.8f) + leftWallHit.normal;
            }

            if (wallRight)
            {
                wallRunJumpDirection = (transform.up * 0.8f) + rightWallHit.normal;
            }
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            pM.groundDrag = holdDrag;
            Aud.Stop();
            wallJump = true;
            StopWallRun();
        }
    }
    //Make sure we credit https://www.youtube.com/channel/UCW7dxGTnyzJ3KYWzLbGHhx big hubunger help

    private void FixedUpdate()
    {
        if(startForce)
        {
            rb.AddForce(orientation.forward * (wallRunForward/2), ForceMode.Impulse);
            startForce = false;
        }

        if(wallForce)
        {
            rb.AddForce(orientation.forward * wallRunForward, ForceMode.Force);
            rb.AddForce(Vector3.down * wallRunGravity, ForceMode.Force);
        }

        if(wallJump)
        {
            rb.AddForce(transform.up * wallRunJumpForce * 2.25f, ForceMode.Impulse);
            rb.AddForce(orientation.forward * wallRunJumpForce * 3f, ForceMode.Impulse);
            rb.AddForce(wallRunJumpDirection * wallRunJumpForce * 12.25f, ForceMode.Impulse);
            wallJump = false;
        }
    }
}
