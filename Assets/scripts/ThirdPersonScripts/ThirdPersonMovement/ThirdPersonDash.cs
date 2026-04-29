using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ThirdPersonDash : MonoBehaviour
{
    [SerializeField] ThirdPersonMovement pM;
    [SerializeField] ThirdPersonCam pCam;
    [SerializeField] GrappleGun1 g1;
    [SerializeField] Camera Cam;
    public bool isDashing;
    public Rigidbody rb;
    [SerializeField] float dashingfovTime;
    [SerializeField] float dashCd;
    private float dashCdTimer;
    [SerializeField] ThirdPersonWallrun wR;
    [SerializeField] float dashForce, dashUpward, dashDuration;

    public bool directionalDashing = true;
    public bool disableGravity = false;
    public bool resetVel = true;

    private bool kinematicStatus = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (dashCdTimer > 0)
            dashCdTimer -= Time.deltaTime;
    }
    public void Dash()
    {
        if (dashCdTimer > 0)
            return;
        else dashCdTimer = dashCd;
        //wR.wRunningAllowed = false;

        if (pCam.targeting == true && g1.isGrappled)
        {
            pM.dashMode = true;
            kinematicStatus = false;
            rb.useGravity = false;
            g1.StopGrapple();
            pCam.targeting = false;
            if (pM.target.GetComponent<Rigidbody>().isKinematic == true)
            {
                kinematicStatus = true;
            }
            if (pM.timeSlow)
            {
                pM.timeSlow = false;
                Time.timeScale = 1f;
            }
            pM.target.GetComponent<Rigidbody>().isKinematic = true;
            pM.target.GetComponent<Collider>().enabled = false;
            //pM.target.GetComponent<enemyHitScript>().targetable = false;
            rb.AddForce(transform.up * pM.jumpHeight, ForceMode.Impulse);
            dashCdTimer = dashCd / 2;
            StartCoroutine(delayedDashMode(pM.target));
        }
        else
        {

            isDashing = true;
            //pM.dashDetector.SetActive(true);

            Vector3 direction = GetDirectionalInput(pM.Orientation);
            Vector3 forceToApply = direction * dashForce + pM.Orientation.up * dashUpward;
            Vector3 slamForceDash = direction * (dashForce * 1.2f) + pM.Orientation.up * -dashUpward * 1.4f;
            if (!pM.isSlamming)
            {
                rb.AddForce(forceToApply, ForceMode.Impulse);
                StartCoroutine(delayedDashForce());
            }
            else
            {
                rb.AddForce(slamForceDash, ForceMode.Impulse);
                StartCoroutine(delayedDashForce());
            }

            Invoke(nameof(ResetDash), dashDuration);
        }
    }

    private Vector3 delayedForceToApply;

    IEnumerator delayedDashForce()
    {
        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
        float timeElapsed = 0;
        while (timeElapsed < dashingfovTime)    //Lerping the camera fov, stuck in a coroutine so i dont have to shove it in update, you remember how Lerping works i hope, having that whole dash to target thing.
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        yield break;
    }

    IEnumerator delayedDashMode(GameObject target)
    {
        //this.GetComponent<PlayerLook>().targeting = false;
        Vector3 tempVel = rb.velocity;
        Vector3 actualTarget = target.transform.position;
        var startPosition = gameObject.transform.position;
        actualTarget.y += 12;
        float timeElapsed = 0;
        while (timeElapsed < dashingfovTime)
        {
            transform.position = Vector3.Lerp(startPosition, actualTarget, timeElapsed * 5 / 2); //need to eventually make this based on distance rather than time
            timeElapsed += Time.deltaTime;
            yield return null; //prevents instant tp to target.
        }
        pM.doubleJump = true;
        rb.velocity += tempVel;
        pM.dashMode = false;
        rb.AddForce(pM.Orientation.forward * (dashForce/* * 1.16f */));
        yield return waitForJump();

        yield return dashModeAfter(target);
    }

    IEnumerator dashModeAfter(GameObject target)
    {
        rb.useGravity = true;
        isDashing = false;
        float timeElapsed = 0;

        Vector3 actualTarget = transform.position;
        actualTarget.y -= 12f;
        target.transform.position = actualTarget;
        target.GetComponent<Collider>().enabled = true;
        target.GetComponent<Rigidbody>().isKinematic = kinematicStatus;

        while (timeElapsed < dashingfovTime)
        { 
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        wR.wRunningAllowed = true;
        yield break;
    }

    IEnumerator waitForJump()
    {
        float timeElapsed = 0;

        do
        {
            if (Input.GetKey(KeyCode.Space))
            {
                Vector3 forceToApply = pM.Orientation.up * dashUpward * 20f;
                rb.AddForce(forceToApply, ForceMode.Impulse);
                yield break;
            }
            timeElapsed += Time.deltaTime;
        }
        while (timeElapsed < 2f);
        yield return null;
    }

    public void ResetDash()
    {
        isDashing = false;
        //pM.dashDetector.SetActive(false);
        wR.wRunningAllowed = true;
        //rb.useGravity = true;
    }

    private Vector3 GetDirectionalInput(Transform orientation)
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3();

        if (directionalDashing)
            direction = orientation.forward * verticalInput + orientation.right * horizontalInput;
        else
            direction = orientation.forward;

        if (verticalInput == 0 && horizontalInput == 0)
            direction = orientation.forward;

        return direction.normalized;
    }
}
