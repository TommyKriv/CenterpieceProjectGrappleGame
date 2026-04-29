using System.Collections;
using UnityEngine;
using Cinemachine;

public class dashScript : MonoBehaviour
{
    [SerializeField] PlayerMovement pM;
    [SerializeField] GrappleGun1 g1;
    [SerializeField] CinemachineVirtualCamera Cam;
    public bool isDashing;
    private Rigidbody rb;
    [SerializeField] float dashingfov;
    public float dashingfovTime;
    [SerializeField] float dashCd;
    private float dashCdTimer;
    [SerializeField] WallRun wR;
    public float dashForce, dashUpward, dashDuration;

    public bool useCameraForward = true;
    public bool directionalDashing = true;
    //public bool disableGravity = false;
    public bool resetVel = true;

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
            isDashing = true;
            pM.dashDetector.SetActive(true);

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

    private Vector3 delayedForceToApply;

    IEnumerator delayedDashForce()
    {
        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
        float timeElapsed = 0;
        while (timeElapsed < dashingfovTime)    //Lerping the camera fov, stuck in a coroutine so i dont have to shove it in update, you remember how Lerping works i hope, having that whole dash to target thing.
        {
            Cam.m_Lens.FieldOfView = Mathf.Lerp(Cam.m_Lens.FieldOfView, dashingfov, timeElapsed / dashingfovTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        Cam.m_Lens.FieldOfView = dashingfov;
        yield break;
    }

    public void ResetDash()
    {
        isDashing = false;
        pM.dashDetector.SetActive(false);
        //wR.wRunningAllowed = true;
        Cam.m_Lens.FieldOfView = pM.fov;
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
