using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ThirdPersonWallrun : MonoBehaviour
{
    [SerializeField] ThirdPersonMovement pM;
    [SerializeField] ThirdPersonDash ds;

    [SerializeField] Transform orientation;

    [SerializeField] float wallDistance;
    [SerializeField] float minimumJumpHeight = 1.6f;

    [SerializeField] float wallRunGravity;
    [SerializeField] float wallRunJumpForce, wallRunForward;

    [SerializeField] Camera Camera;
    //private float fov;
    //private float wallRunfov; //felt best at 92
    public float wallRunfovTime;
    [SerializeField] float camTilt;
    [SerializeField] float camTiltTime;
    [SerializeField] LayerMask groundMask;

    public bool isWRunning;

    public float Tilt { get; private set; }

    bool wallLeft = false;
    bool wallRight = false;

    RaycastHit leftWallHit;
    RaycastHit rightWallHit;

    private Rigidbody rb;

    public bool wRunningAllowed = true;
    private float holdDrag;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        holdDrag = pM.groundDrag;
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
        if (wRunningAllowed == true)
        {
            CheckWall();

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
                Tilt = Mathf.Lerp(Tilt, 0, camTiltTime * Time.deltaTime);
        }
            return;
    }

    public void StopWallRun()
    {
        isWRunning = false;

        rb.useGravity = true;

        pM.canSlam = true;

        Tilt = Mathf.Lerp(Tilt, 0, camTiltTime * Time.deltaTime);
        pM.groundDrag = holdDrag;
    }

    void StartWallRun()
    {
        isWRunning = true;
        pM.groundDrag = 0f;

        rb.useGravity = false;
        rb.AddForce(orientation.forward * wallRunForward, ForceMode.Force);
        rb.AddForce(Vector3.down * wallRunGravity, ForceMode.Force);

        pM.canSlam = false;

        if (wallLeft)
        {
            Tilt = Mathf.Lerp(Tilt, -camTilt, camTiltTime * Time.deltaTime);
        }
        else if (wallRight)
        {
            Tilt = Mathf.Lerp(Tilt, camTilt, camTiltTime * Time.deltaTime);
        }

        if (Input.GetButtonDown("Jump"))
        {

            if (wallLeft)
            {
                Vector3 wallRunJumpDirection = (transform.up * 0.8f) + leftWallHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(wallRunJumpDirection * wallRunJumpForce * 100, ForceMode.Force);
                pM.groundDrag = holdDrag;
            }

            if (wallRight)
            {
                Vector3 wallRunJumpDirection = (transform.up * 0.8f) + rightWallHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(wallRunJumpDirection * wallRunJumpForce * 100, ForceMode.Force);
                pM.groundDrag = holdDrag;

            }
        }
    }
    //Make sure we credit https://www.youtube.com/channel/UCW7dxGTnyzJ3KYWzLbGHhx big hubunger help
}
