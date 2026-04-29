using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using Cinemachine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public GameObject target;

    private PlayerLook pL;

    [SerializeField] GrappleGun1 g1;
    [SerializeField] WallRun wR;
    [SerializeField] dashScript dS;
    [SerializeField] Camera Cam;
    public CinemachineVirtualCamera Vcam;
    [SerializeField] GameObject uiHolder;
    [SerializeField] GameObject camPos;
    float playerHeight;

    public Transform Orientation;

    [SerializeField] private DialogueUI dialogueUI;

    public DialogueUI DialogueUI => dialogueUI;
    public IInteractable Interactable { get; set; }

    //Movement 
    public float fov;

    public float moveSpeed;

    public float jumpHeight = 6f;

    float movementMultiplier = 10f;

    float x, y;
    float threshold = 0.01f;

    [SerializeField] float counterMovement;

    [SerializeField] float airMultiplier;

    public float groundDrag;
    [SerializeField] float airDrag;

    float horizontalMovement;
    float verticalMovement;

    Vector3 playerScale;
    Vector3 crouchScale = new Vector3(1, 0.7f, 1);

    //GroundCheck
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;
    float groundDistance = 0.6f; //0.26

    public bool isGrounded;
    public bool doubleJump, canDj;
    public float jumpShake, grappleShake;
    public bool isSlamming;
    public bool canSlam = true;
    public bool dashMode;
    public bool timeSlow = false;
    public bool restricted = false;
    public bool unlimited, freeze = false;

    public bool isSliding;
    public float slideForce;
    [SerializeField] float camTilt;
    [SerializeField] float camTiltTime;

    public bool onRail = false;
    public float grindSpeed;
    public float grindJumpMult;
    [SerializeField] float heightOffset;
    float timeForFullSpline;
    float elapsedTime;
    [SerializeField] float lerpSpeed = 10f;
    [SerializeField] railGrindScript currentRailScript;
    private bool dirCalculated;

    public GameObject dashDetector, BounceDetector, camHolder, slideDetector;

    [SerializeField] float slamForce;

    [SerializeField] float coyoteTime;
    public float coyoteTimeCounter;

    [SerializeField] float jumpBufferTime;
    private float jumpBufferCounter;

    [SerializeField] float slideCoyoteTime;
    private float slideCoyoteCounter;

    [SerializeField] float ledgeDelay;
    private float ledgeDelayCounter;

    public bool isJumping;
    private bool jump;

    public float fallSpeedMult, fallSpeedCap, slamSpeedCap;

    Vector3 moveDirection;
    Vector3 slopeMoveDirection;

    public Rigidbody rb;

    RaycastHit slopeHit;

    public KeyCode dashKey = KeyCode.LeftShift;
    public GameObject[] enemies;
    public float targetDist;
    public Renderer target2;

    private gameMaster gm;

    private float baseGroundDrag;
    private Vector3 slideDir;
    [SerializeField] FMODUnity.StudioEventEmitter footsteps;

    private bool atLedge;

    private PhysicMaterial mat;

    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f))
        {
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    private void Awake()
    {
        gm = gameMaster.instance;
        if (gm.checkPoints)
        {
            transform.position = gm.lastCheckPoint;
            Debug.Log("Merp");
        }
    }

    private void Start()
    {
        playerHeight = gameObject.transform.localScale.y;
        Vcam.m_Lens.FieldOfView = fov;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        baseGroundDrag = groundDrag;
        playerScale = transform.localScale;
        pL = this.GetComponent<PlayerLook>();
        beforePos = camPos.transform.localPosition.y;
        fov = gm.fov;
        footsteps = GetComponent<FMODUnity.StudioEventEmitter>();
        mat = this.GetComponent<CapsuleCollider>().material;
    }

    private void Update()
    {
        ControlDrag();
        MyInput();
        IsGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (freeze)
        {
            rb.velocity = Vector3.zero;
        }

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            mat.staticFriction = 1.1f;
            mat.dynamicFriction = 0.15f;
        }
        else
        {
            mat.staticFriction = 0f;
            mat.dynamicFriction = 0f;
        }

        if (ledgeDelayCounter > 0)
        {
            ledgeDelayCounter -= Time.deltaTime;
        }

        if (!isGrounded && !onRail)
        {
            coyoteTimeCounter -= Time.deltaTime;
            if (rb.useGravity)
            {
                ledgeCheck();
            }
        }

        if (Input.GetButton("Jump"))
        {
            if (atLedge && ledgeDelayCounter <= 0f)
            {
                Vector3 hitPoint = ledgeCheck();
                if (hitPoint != Vector3.zero)
                {
                    StartCoroutine(ledgeClimb(hitPoint));
                }
            }
        }


        if (Input.GetButtonDown("Jump") && !wR.isWRunning)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        slideCoyoteCounter -= Time.deltaTime;

        if (Input.GetKeyDown("r"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (rb.velocity.y < 0f)
        {
            if (isJumping)
            {
                isJumping = false;
            }
            rb.velocity = new Vector3(rb.velocity.x, (rb.velocity.y * fallSpeedMult), rb.velocity.z);

            if (!isSlamming)
            {
                rb.velocity = new Vector3(rb.velocity.x, Mathf.Max(rb.velocity.y, -fallSpeedCap), rb.velocity.z);
            }
            else
            {
                rb.velocity = new Vector3(rb.velocity.x, Mathf.Max(rb.velocity.y, -slamSpeedCap), rb.velocity.z);
            }
        }

        if (!restricted)
        {
            if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f || jumpBufferCounter > 0f && onRail)
            {
                jump = true;
                coyoteTimeCounter = 0f;
            }
            else if (!wR.isWRunning && !wR.wallJump && Input.GetButtonDown("Jump") && !isGrounded && coyoteTimeCounter <= 0f && doubleJump && canDj && !onRail)
            {
                if (isSlamming)
                {
                    ResetSlam();
                }
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                jump = true;
                doubleJump = false;
            }
        }

        if (Input.GetButtonUp("Jump") && isJumping && !onRail)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y / 1.8f, rb.velocity.z);
        }

        if (Input.GetKeyDown(dashKey) && !dashMode && !onRail)
        {
            dS.Dash();
        }

        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);

        if (isSliding)
        {
            groundDrag = 0;
            if (rb.velocity.magnitude < 60 || !Input.GetButton("Crouch"))
            {
                StopSlide();
            }
        }

        if (isSliding && OnSlope())
        {
            gameObject.GetComponent<CapsuleCollider>().material.dynamicFriction = 0;
            rb.velocity = (slideDir * (slideForce - 20)) + new Vector3(0, rb.velocity.y, 0);
        }

        if (doubleJump == false && wR.isWRunning == true || doubleJump == false && onRail)
        {
            doubleJump = true;
        }

        if (Input.GetButtonDown("Crouch") && !isGrounded && !isSlamming && slideCoyoteCounter <= 0f && !dashMode && !onRail && canSlam) //Crouch slide detection in jumphitscript
        {
            dS.ResetDash();
            if (isJumping)
                isJumping = false;
            StartCoroutine(slam());
        }

        if (Input.GetKey("e") && !dashMode)
        {
            Renderer target = getClosestTargetableEnemy();
            if (target != null)
            {
                target2 = isInCameraView(target.gameObject);
                if (target2 != null)
                {
                    autoLock(target2);
                }
                else
                    return;
            }
            else
            {
                GetComponent<PlayerLook>().targeting = false;
                if (timeSlow)
                {
                    timeSlow = false;
                    Time.timeScale = 1f;
                }
            }
            return;

        }
        if (Input.GetKeyUp("e"))
        {
            pL.targeting = false;
            target = null;
            target2 = null;
            if (timeSlow)
            {
                timeSlow = false;
                Time.timeScale = 1f;
            }
        }

        if (Input.GetKeyDown(KeyCode.F)/* && dialogueUI.IsOpen == false*/)
        {
            if (Interactable != null)
            {
                Interactable.Interact(player: this);
            }
        }

        if (onRail)
        {
            MovePlayerAlongRail();
        }
    }
    void MyInput()
    {
        if (restricted)
            return;
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        if (wR.isWRunning)
        {
            if (wR.wallLeft)
            {
                if (horizontalMovement == -1)
                    horizontalMovement = 0;
            }
            if (wR.wallRight)
            {
                if (horizontalMovement == 1)
                    horizontalMovement = 0;
            }
        }

        moveDirection = Orientation.forward * verticalMovement + Orientation.right * horizontalMovement;

        if (isSliding || onRail) //The slide tilting
        {
            switch (horizontalMovement)
            {
                case 0:
                    wR.Tilt = Mathf.Lerp(wR.Tilt, 0, camTiltTime * Time.deltaTime);
                    break;
                case 1:
                    wR.Tilt = Mathf.Lerp(wR.Tilt, -camTilt, camTiltTime * Time.deltaTime);
                    break;
                case -1:
                    wR.Tilt = Mathf.Lerp(wR.Tilt, camTilt, camTiltTime * Time.deltaTime);
                    break;
            }
        }

        if (Input.GetButtonDown("Crouch") && isGrounded)
            Slide();
        if (Input.GetButtonUp("Crouch"))
            StopSlide();
    }

    void ControlDrag()
    {
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = airDrag;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();

        if (jump && !dashMode)
        {
            Jump();
        }
    }

    void MovePlayer()
    {
        if (dS.isDashing || onRail)
            return;

        if (isSliding) //The slide til movement
        {
            if (horizontalMovement == 1)
            {
                rb.AddForce(Orientation.right * 25f, ForceMode.Force);
            }
            else if (horizontalMovement == -1)
            {
                rb.AddForce(-Orientation.right * 25f, ForceMode.Force);
            }
            return;
        }

        if (isGrounded)
        {
            if (Mathf.Abs(rb.velocity.x + rb.velocity.z) > 30)
            {
                if (!footsteps.IsPlaying())
                {
                    footsteps.Play();
                }
            }
            else
            {
                footsteps.Stop();
            }
            if (!OnSlope())
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
            }
            else if (OnSlope() && !isSliding)
            {
                rb.AddForce(slopeMoveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Force);
            }
        }
        else
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
            GetComponent<FMODUnity.StudioEventEmitter>().Stop();
        }

        if (isSlamming && OnSlope())
        {
            gameObject.GetComponent<CapsuleCollider>().material.dynamicFriction = 0;
            rb.AddForce(slopeMoveDirection.normalized * Time.deltaTime * 260);
            isSlamming = false;
            isGrounded = true;
        }

        if (isSlamming && rb.velocity.y < -slamSpeedCap)
        {
            rb.AddForce(slamForce / 1.5f * -transform.up, ForceMode.Acceleration);
        }

        if (wR.isWRunning)
        {
            if(footsteps.IsPlaying())
            {
                footsteps.Stop();
            }
            if (wR.wallLeft)
            {
                rb.AddForce(-Orientation.right * 6, ForceMode.Force);
            }
            if (wR.wallRight)
            {
                rb.AddForce(Orientation.right * 6, ForceMode.Force);
            }
        }

        Vector2 mag = FindVelRelativeToLook();

        CounterMovement(x, y, mag);
    }

    void Jump()
    {
        jump = false;
        isJumping = true;
        coyoteTimeCounter = 0;
        if (slideCoyoteCounter > 0f || isSliding)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * jumpHeight * 1.3f, ForceMode.Impulse);
            rb.AddForce(Orientation.transform.forward * moveSpeed * 2.5f, ForceMode.Impulse);
            StopSlide();
            return;
        }
        if(onRail)
        {
            onRail = false;
            currentRailScript.Aud.Stop();
            currentRailScript = null;
            dirCalculated = false;
            transform.position = new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z);
            rb.AddForce(Orientation.transform.forward * (grindSpeed * 1.2f), ForceMode.Impulse);
            rb.AddForce(transform.up * (jumpHeight * grindJumpMult), ForceMode.Impulse);
            wR.Tilt = 0;
            if (horizontalMovement == 1)
            {
                rb.AddForce(transform.right * jumpHeight * 2f, ForceMode.Impulse);
            }
            else if (horizontalMovement == -1)
            {
                rb.AddForce(-transform.right * jumpHeight * 2f, ForceMode.Impulse);
            }
            transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
            return;
        }

        rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);
    }


    public void Slide()
    {
        if (footsteps.IsPlaying())
        {
            footsteps.Stop();
        }
        groundDrag = 0;
        Vcam.m_Lens.FieldOfView = Mathf.Lerp(fov + 5f, Vcam.m_Lens.FieldOfView, 10f * Time.deltaTime);
        slideDetector.SetActive(true);
        transform.localScale = crouchScale;
        transform.position = new Vector3(transform.position.x, transform.position.y - crouchScale.y, transform.position.z);
        slideDir = Orientation.transform.forward;
        rb.velocity = (slideDir * (slideForce - 60)) + new Vector3(0, rb.velocity.y, 0);
        gameObject.GetComponent<CapsuleCollider>().material.dynamicFriction = 0.1f;
        isSliding = true;
    }

    void StopSlide()
    {
        if (isSliding)
        {
            if (groundDrag == 0)
                groundDrag = baseGroundDrag;
            slideDetector.SetActive(false);
            Vcam.m_Lens.FieldOfView = fov;
            transform.localScale = playerScale;
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);
            isSliding = false;
            gameObject.GetComponent<CapsuleCollider>().material.dynamicFriction = 0.3f;
            wR.Tilt = Mathf.Lerp(wR.Tilt, 0, camTiltTime * Time.deltaTime);
            slideCoyoteCounter = slideCoyoteTime;
        }
    }


    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!isGrounded && !isSliding || isSlamming)
            return;

        if(onRail)
        {
            return;
        }

        if (Mathf.Abs(mag.x) > threshold && Mathf.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0))
        {
            rb.AddForce(moveSpeed * Orientation.transform.right * Time.deltaTime * -mag.x * counterMovement);
        }
        if (Mathf.Abs(mag.y) > threshold && Mathf.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0))
        {
            rb.AddForce(moveSpeed * Orientation.transform.forward * Time.deltaTime * -mag.y * counterMovement);
        }
    }

    public Vector2 FindVelRelativeToLook()
    {
        float lookAngle = Orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitude = rb.velocity.magnitude;
        float yMag = magnitude * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitude * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }

    private bool IsGrounded
    {
        get { return isGrounded; }
        set
        {
            if (isGrounded == false && value == true)
            {
                coyoteTimeCounter = coyoteTime;
                doubleJump = true;
                if(!isSlamming)
                {
                    StartCoroutine(uiShake(jumpShake));
                }
                if (wR.isWRunning)
                {
                    wR.StopWallRun();
                    Vcam.m_Lens.FieldOfView = Mathf.Lerp(fov, Vcam.m_Lens.FieldOfView, wR.wallRunfovTime * Time.deltaTime);
                }
            }
            isGrounded = value;
        }
    }
    float bounceTime;
    float waitTime;
    Vector3 originalPos;
    float beforePos;
    IEnumerator uiShake(float shakeAmount)
    {
        bounceTime = 0;
        waitTime = 2f;
        originalPos = uiHolder.transform.localPosition;
        uiHolder.transform.localPosition = new Vector3(uiHolder.transform.localPosition.x, uiHolder.transform.localPosition.y - shakeAmount, uiHolder.transform.localPosition.z);
        
        camPos.transform.localPosition = new Vector3(camPos.transform.localPosition.x, camPos.transform.localPosition.y - grappleShake, camPos.transform.localPosition.z);
        while (bounceTime < waitTime)
            {
            uiHolder.transform.localPosition = new Vector3(uiHolder.transform.localPosition.x, Mathf.Lerp(uiHolder.transform.localPosition.y, originalPos.y, (bounceTime / waitTime)), uiHolder.transform.localPosition.z);
            camPos.transform.localPosition = new Vector3(camPos.transform.localPosition.x, Mathf.Lerp(camPos.transform.localPosition.y, beforePos, (bounceTime / waitTime)), camPos.transform.localPosition.z);
                bounceTime += Time.deltaTime;
                yield return null;
            }
        uiHolder.transform.localPosition = new Vector3(uiHolder.transform.localPosition.x, originalPos.y, uiHolder.transform.localPosition.z);
        camPos.transform.localPosition = new Vector3(camPos.transform.localPosition.x, beforePos, camPos.transform.localPosition.z);
        yield return null;
    }

    IEnumerator slam()
    {
        isSlamming = true;
        dS.isDashing = false;
        BounceDetector.SetActive(true);
        rb.velocity = new Vector3(rb.velocity.x * 0.95f, rb.velocity.y * 0.15f, rb.velocity.z * 0.95f);
        Vcam.m_Lens.FieldOfView = Mathf.Lerp(Vcam.m_Lens.FieldOfView, Vcam.m_Lens.FieldOfView - 3f, 1f * Time.deltaTime);
        rb.velocity = new Vector3(rb.velocity.x, slamForce * 0.5f, rb.velocity.z);
        yield break;
    }

    public void ResetSlam()
    {
        Vcam.m_Lens.FieldOfView = fov;
        isSlamming = false;
    }

    private Renderer getClosestTargetableEnemy()
    {
        Renderer targetRenderer = null;
        Vector3 closestEnemyDist = new Vector3(2600, 2600, 2600);
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
            {
                Vector3 enemyDist = transform.position - enemies[i].transform.position;
                if (enemyDist.magnitude < closestEnemyDist.magnitude && enemies[i].GetComponent<enemyHitScript>().targetable && enemies[i].gameObject.tag == "Enemy")
                {
                    closestEnemyDist = transform.position - enemies[i].transform.position;
                    targetRenderer = enemies[i].GetComponent<Renderer>();
                }
            }
            else
            {
                return null;
            }
        }

        return targetRenderer;
    }
    private Renderer isInCameraView(GameObject target)
    {
        Plane[] cameraFrustum = GeometryUtility.CalculateFrustumPlanes(Cam);
        if (GeometryUtility.TestPlanesAABB(cameraFrustum, target.GetComponent<Collider>().bounds))
        {
            return target.GetComponent<Renderer>();
        }
        else
        {
            return null;
        }
    }
    void autoLock(Renderer targetRenderer)
    {
        float distance = Vector3.Distance(targetRenderer.gameObject.transform.position, gameObject.transform.position);
        if (distance < targetDist)
        {
            pL.targeting = true;
            target = targetRenderer.gameObject;
            camHolder.transform.LookAt(target.transform);
            Orientation.LookAt(target.transform);
            timeSlow = true;
            Time.timeScale = 0.6f;
        }
        else
        {
            pL.targeting = false;
        }
    }

    void MovePlayerAlongRail()
    {
        if (currentRailScript != null && onRail)
        {
            float progress = elapsedTime / timeForFullSpline;

            if (progress < 0 || progress > 1)
            {
                ThrowOffRail();
                return;
            }
            float nextTimeNormalized;
            if (currentRailScript.normalDir)
                nextTimeNormalized = (elapsedTime + Time.deltaTime) / timeForFullSpline;
            else
                nextTimeNormalized = (elapsedTime - Time.deltaTime) / timeForFullSpline;

            float3 pos, tangent, up;
            float3 nextPosFloat, nextTan, nextUp;
            SplineUtility.Evaluate(currentRailScript.railSpline.Spline, progress, out pos, out tangent, out up);
            SplineUtility.Evaluate(currentRailScript.railSpline.Spline, nextTimeNormalized, out nextPosFloat, out nextTan, out nextUp);

            Vector3 worldPos = currentRailScript.LocalToWorldConversion(pos);
            Vector3 nextPos = currentRailScript.LocalToWorldConversion(nextPosFloat);

            transform.position = worldPos + (transform.up * (heightOffset + currentRailScript.transform.lossyScale.y/2));
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(nextPos - worldPos), lerpSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up, up) * transform.rotation, lerpSpeed * Time.deltaTime);

            if (currentRailScript.normalDir)
                elapsedTime += Time.deltaTime;
            else
                elapsedTime -= Time.deltaTime;
        }
    }
    void ThrowOffRail()
    {
        currentRailScript.Aud.Stop();
        onRail = false;
        currentRailScript = null;
        dirCalculated = false;
        isJumping = true;
        rb.AddForce(transform.forward * grindSpeed, ForceMode.Impulse);
        transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, transform.rotation.w);
        rb.AddForce(transform.up * jumpHeight * 0.9f, ForceMode.Impulse);
    }

    void CalculateAndSetRailPosition()
    {
        timeForFullSpline = currentRailScript.totalSplineLength / grindSpeed;

        Vector3 splinePoint;

        float normalisedTime = currentRailScript.CalculateTargetRailPoint(transform.position, out splinePoint);
        elapsedTime = timeForFullSpline * normalisedTime;

        float3 pos, forward, up;
        SplineUtility.Evaluate(currentRailScript.railSpline.Spline, normalisedTime, out pos, out forward, out up);

        if(!dirCalculated)
        {
            currentRailScript.CalculateDirection(forward, Orientation.forward);
            dirCalculated = true;
        }


        transform.position = splinePoint + (transform.up * (heightOffset + currentRailScript.transform.lossyScale.y/1.5f));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Rail")
        {
            if (isSlamming)
            {
                ResetSlam();
            }
            onRail = true;
            currentRailScript = collision.gameObject.GetComponent<railGrindScript>();
            if(!currentRailScript.Aud.IsPlaying())
            {
                currentRailScript.Aud.Play();
            }
            CalculateAndSetRailPosition();
        }
    }

    private Vector3 ledgeCheck()
    {
        RaycastHit hit, hit2;
        Vector3 feetCheck = new Vector3(groundCheck.position.x, groundCheck.position.y + 1.5f, groundCheck.position.z);
        Debug.DrawRay((feetCheck - Orientation.forward * 2f), Orientation.forward * 29, Color.yellow);
        Debug.DrawRay((Cam.gameObject.transform.position + (transform.up * 1.1f) - (Orientation.forward * 2f)), Orientation.forward * 30, Color.green);

        if (Physics.Raycast((feetCheck - Orientation.forward * 2f), Orientation.forward, out hit, 29, groundMask))
        {
            if (Physics.Raycast((Cam.gameObject.transform.position + (transform.up * 2f) - (Orientation.forward * 2f)), Orientation.forward, out hit2, 30, groundMask))
            {
            }
            else
            {
                if(ledgeEquation(hit.transform.rotation.eulerAngles))
                {
                    atLedge = true;
                    Debug.Log(hit.transform.gameObject.name);
                    return hit.point;
                }
            }
        }
        atLedge = false;
        return Vector3.zero;
    }

    bool ledgeEquation(Vector3 angle)
    {
        if (angle.x % 90 <= 5f && angle.y % 90 <= 5f && angle.z % 90 <= 5f)
        {
            return true;

        }
        else 
            return false;
    }

IEnumerator ledgeClimb(Vector3 hitPoint)
    {
        atLedge = false;
        wR.enabled = false;
        rb.useGravity = false;
        Vector3 storedVel = rb.velocity;
        if(storedVel.y < 0)
        {
            storedVel.y = 0;
        }
        rb.velocity = Vector3.zero;
        float timeElapsed = 0;
        Vector3 newPoint = new Vector3(hitPoint.x, hitPoint.y + 12f, hitPoint.z) + (Orientation.transform.forward * 12f);
        float distance = Vector3.Distance(newPoint, transform.position);
        float trackingdistance = 5;
        while (timeElapsed < 0.25 || trackingdistance > 2.5)
        {
            transform.position = Vector3.Lerp(transform.position, newPoint, timeElapsed / Mathf.Min((distance * 0.018f), 0.5f));
            trackingdistance = Vector3.Distance(newPoint, transform.position);
            timeElapsed += Time.deltaTime;
            yield return null; //prevents instant tp to target.
        }
        //transform.position += Orientation.forward * 2f;
        yield return ledgeHop(storedVel);
    }

    IEnumerator ledgeHop(Vector3 storedVel)
    {
        rb.velocity = storedVel;
        rb.AddForce(transform.up * 1.5f);
        rb.AddForce(Orientation.forward * 3f);
        rb.useGravity = true;
        wR.enabled = true;
        ledgeDelayCounter = ledgeDelay;
        yield return null;
    }
}
// https://www.youtube.com/watch?v=XAC8U9-dTZU