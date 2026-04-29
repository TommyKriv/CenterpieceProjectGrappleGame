using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class ThirdPersonMovement : MonoBehaviour
{
        public GameObject target;

        [SerializeField] GrappleThirdPerson g1;
        public ThirdPersonCam pCam;
        public CinemachineFreeLook Vcam;
        [SerializeField] ThirdPersonWallrun wR;
        [SerializeField] ThirdPersonDash dS;
        [SerializeField] Camera Cam;
        float playerHeight = 2f;

        public Transform Orientation;

        [SerializeField] private DialogueUI dialogueUI;

        public DialogueUI DialogueUI => dialogueUI;
        public IInteractable Interactable { get; set; }

        //Movement 
        public float fov;
        public float moveSpeed = 10f;

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
        Vector3 crouchScale = new Vector3(1.4f, 0.8f, 1.4f);

        //GroundCheck
        [SerializeField] Transform groundCheck;
        [SerializeField] LayerMask groundMask;
        float groundDistance = 0.2f;

        public bool isGrounded { get; private set; }
        public bool doubleJump;
        public bool isSlamming;
        public bool canSlam = true;
        public bool dashMode;
        public bool timeSlow = false;

        public bool isSliding;
        [SerializeField] float slideForce = 600f;
        [SerializeField] float slideCounterMovement = 0.2f;


        [SerializeField] float punchDuration;

        [SerializeField] float punchCd;
        [SerializeField] float punchCdTimer;

        public GameObject dashDetector, PunchDetector, BounceDetector, camHolder, slideDetector;

        [SerializeField] float slowDownCd;
        float slowDownTime;

        [SerializeField] float slamForce;

        Vector3 moveDirection;
        Vector3 slopeMoveDirection;

        public Rigidbody rb;

        RaycastHit slopeHit;

        public KeyCode dashKey = KeyCode.LeftShift;
        public GameObject[] enemies;
        [SerializeField] float targetDist;
        public Renderer target2;

        private gameMaster gm;
        private GameObject playerObj;

        private bool OnSlope()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.5f))
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

        private void Start()
        {
            playerObj = gameObject.transform.GetChild(0).gameObject;
            gm = gameMaster.instance;
            if(gm.checkPoints)
            {
                transform.position = gm.lastCheckPoint;
            }
            Vcam.m_Lens.FieldOfView = fov;
            slowDownTime = slowDownCd;
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;
            fov = gm.fov;

            playerScale = transform.GetChild(0).localScale;
        }

        private void Update()
        {
            MyInput();
            IsGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (Input.GetKeyDown("r"))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            if (Input.GetButtonDown("Jump") && isGrounded && !isSliding && !dashMode)
            {
                if (isSlamming)
                {
                    ResetSlam();
                }
                Jump();
            }

            if (Input.GetButtonDown("Jump") && isSliding && !dashMode)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(transform.up * jumpHeight * 1.1f, ForceMode.Impulse);
                rb.AddForce(Orientation.transform.forward * moveSpeed * movementMultiplier * airMultiplier * 8f, ForceMode.Acceleration);
                StopSlide();
            }

            else if (Input.GetButtonDown("Jump") && !isGrounded && !wR.isWRunning && doubleJump && !dashMode)
            {
                if (isSlamming)
                {
                    ResetSlam();
                }
                Jump();
                //Cam.fieldOfView = Mathf.Lerp(Cam.fieldOfView, fov - 6, 25f * Time.deltaTime);
                doubleJump = false;
                //Cam.fieldOfView = Mathf.Lerp(Cam.fieldOfView, fov, 25f * Time.deltaTime);
            }



            if (Input.GetKeyDown(dashKey) && !dashMode)
            {
                dS.Dash();
            }



            if (punchCdTimer > 0)
                punchCdTimer -= Time.deltaTime;

            slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);

            if (doubleJump == false && wR.isWRunning == true)
            {
                doubleJump = true;
            }

            if (Input.GetButtonDown("Crouch") && !isGrounded && !isSlamming && !dS.isDashing && !isSliding && !dashMode && canSlam) //Crouch slide detection in jumphitscript
            {
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
                    pCam.targeting = false;
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
                pCam.targeting = false;
                target = null;
                if (timeSlow)
                {
                    timeSlow = false;
                    Time.timeScale = 1f;
                }
            }

            if (Input.GetKeyDown(KeyCode.F) && dialogueUI.IsOpen == false)
            {
                if (Interactable != null)
                {
                    Interactable.Interact(player: this);
                }
            }
        }
        void MyInput()
        {
            horizontalMovement = Input.GetAxisRaw("Horizontal");
            verticalMovement = Input.GetAxisRaw("Vertical");

            moveDirection = Orientation.forward * verticalMovement + Orientation.right * horizontalMovement;

            if (Input.GetButtonDown("Crouch") && isGrounded)
                Slide();
            if (Input.GetButtonUp("Crouch"))
                StopSlide();

            if (Input.GetKeyDown("q"))
            {
                timeSlow = true;
                TimeSlow();
            }
            if (Input.GetKeyUp("q"))
            {
                timeSlow = false;
                Time.timeScale = 1f;
            }

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
            if (slowDownTime < slowDownCd && timeSlow == false)
            {
                slowDownTime++;
            }

            ControlDrag();
            MovePlayer();

            if (isSliding)
            {
                rb.AddForce(Vector3.down * Time.deltaTime * 30000);

                if (rb.velocity.magnitude < 10)
                {
                    StopSlide();
                }
            }

            if (isSlamming && rb.velocity.y < -250)
            {
                rb.AddForce(slamForce / 1.5f * transform.up, ForceMode.Acceleration);
            }
        }

        void MovePlayer()
        {
            if (dS.isDashing || isSliding)
                return;

            if (isGrounded && !OnSlope())
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
            }
            else if (isGrounded && OnSlope())
            {
                rb.AddForce(slopeMoveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
            }
            else if (!isGrounded)
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
            }

            Vector2 mag = FindVelRelativeToLook();

            CounterMovement(x, y, mag);
        }

        void Jump()
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);
        }

        public void Slide()
        {
            if (rb.velocity.magnitude > 12f)
            {
                slideDetector.SetActive(true);
                playerObj.transform.localScale = crouchScale;
                transform.position = new Vector3(transform.position.x, transform.position.y - 0.6f, transform.position.z);
                rb.AddForce(gameObject.transform.GetChild(0).gameObject.transform.forward * slideForce, ForceMode.Impulse);;
                isSliding = true;
            }
            else
                return;
        }

        void StopSlide()
        {
            if (isSliding)
            {
                slideDetector.SetActive(false);
                playerObj.transform.localScale = playerScale;
                transform.position = new Vector3(transform.position.x, transform.position.y + 0.6f, transform.position.z);
                isSliding = false;
            }
        }


        private void CounterMovement(float x, float y, Vector2 mag)
        {
            if (!isGrounded)
                return;

            if (isSliding)
            {
                rb.AddForce(moveSpeed * Time.deltaTime * -rb.velocity.normalized * slideCounterMovement);
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
                    doubleJump = true;
                    if (wR.isWRunning)
                    {
                        wR.StopWallRun();
                    }
                }

                isGrounded = value;
            }
        }

        void Punch()
        {
            if (punchCdTimer > 0)
                return;
            else punchCdTimer = punchCd;

            //var punchCount = 1;
            PunchDetector.SetActive(true);

            Invoke(nameof(ResetPunch), punchDuration);
        }

        IEnumerator slam()
        {
            dS.isDashing = false;
            BounceDetector.SetActive(true);
            rb.velocity = new Vector3(rb.velocity.x * 0.95f, rb.velocity.y * 0.15f, rb.velocity.z * 0.95f);
            /*rb.AddForce(4000 * transform.up);
            WaitForSeconds wait = new WaitForSeconds(0.12f);*/ //Up slam deal
            for (int i = 0; i <= 2; i++)
            {
                if (dS.isDashing)
                {
                    break;
                }
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                yield return new WaitForSeconds(0.02f);
            }
            rb.velocity = new Vector3(rb.velocity.x, slamForce * 0.5f, rb.velocity.z);
            isSlamming = true;
            yield break;
        }
        public void ResetSlam()
        {
            BounceDetector.SetActive(false);
            isSlamming = false;
        }

        void ResetPunch()
        {
            PunchDetector.SetActive(false);
            //punchCount = 0;
        }

        void TimeSlow()
        {
            if (slowDownTime > 0)
            {
                while (slowDownTime > 0 && timeSlow)
                {
                    Time.timeScale = 0.7f;
                    slowDownTime--;
                }
            }
            else
                return;
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
            float distance = Vector3.Distance(targetRenderer.gameObject.transform.position, this.gameObject.transform.position);
            if (distance < targetDist)
            {
                pCam.targeting = true;
                target = targetRenderer.gameObject;
                camHolder.transform.LookAt(target.transform);
                Orientation.LookAt(target.transform);
                timeSlow = true;
                Time.timeScale = 0.6f;
            }
            else
            {
                pCam.targeting = false;
            }
        }
    }
    // https://www.youtube.com/watch?v=XAC8U9-dTZU
