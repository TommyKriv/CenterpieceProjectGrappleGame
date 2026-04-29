using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class GrappleGun1 : MonoBehaviour
{
    [SerializeField] PlayerMovement pM;
    [SerializeField] PlayerLook pL;
    [SerializeField] CinemachineVirtualCamera Cam;
    [SerializeField] dashScript dS;
    [SerializeField] Transform gunTip, Player;
    [SerializeField] Rigidbody PlayerRb, HookHolder;
    [SerializeField] GameObject hookProj, hookPoint, bolusProj, grappleObj;
    [SerializeField] float PullForce, baseFOV;
    public Rigidbody rootRigidbody, enemyRb;

    public List<GameObject> points;
    public List<Transform> RopeJoints;
    private GameObject lastHook, hook, bolus;
    private HookJointConnector proj;

    private LineRenderer ProjLine;
    private Transform hookEnd;

    private int i = 0;

    public bool isGrappled = false, sideWeaponOn = false, enemyGrappled = false, enemyModed = false, GrappleLocked, interactGrappled, ropeMode;
    private bool kinematicStatus = false;

    [SerializeField] FMODUnity.StudioEventEmitter fire, pull;

    private void Start()
    {
        //ropeMode = true;
        //RopeModeSwitch();
        //hookProj.GetComponent<SpringJoint>().spring = 20;
        //hookProj.GetComponent<SpringJoint>().damper = 20;
        //hookProj.GetComponent<SpringJoint>().minDistance = 2;
        //hookProj.GetComponent<SpringJoint>().maxDistance = 0;
    }

    private void Update()
    {
       /* if(Input.GetMouseButton(1) && !IsGrappling() && !isGrappled && !pm.dashMode)
        {
            SideWeaponPrep();
            if (Input.GetMouseButtonDown(0))
            {
                SideWeaponFire();
            }
        }*/

        /*if(sideWeaponOn)
        {
            if(Input.GetMouseButtonUp(1))
            {
                sideWeaponOn = false;
                Camera.fieldOfView = baseFOV;
            }
        }*/
        

        if (!IsGrappling())
        {
            if (Input.GetMouseButtonDown(0) && sideWeaponOn == false && !pL.targeting)
            {
                StartGrapple();
            }
            if (Input.GetMouseButtonDown(0) && sideWeaponOn == false && pL.targeting)
            {
                StartGrappleMode();
            }
        }        

        if (isGrappled)
        {
            if (Input.GetMouseButtonDown(1))
            {
                GrapplePull();
            }
        }
    }

    private void RopeModeSwitch()
    {
        var SpringJoint = hookProj.GetComponent<SpringJoint>();
        if (ropeMode)
        {
            SpringJoint.spring = 20;
            SpringJoint.damper = 20;
            SpringJoint.minDistance = 3;
            SpringJoint.maxDistance = 4;
            ropeMode = false;
            if (IsGrappling())
            {
                hook.GetComponent<SpringJoint>().spring = 20;
                hook.GetComponent<SpringJoint>().damper = 20;
                hook.GetComponent<SpringJoint>().minDistance = 2;
                hook.GetComponent<SpringJoint>().maxDistance = 4;
            }
        }
        else
        {
            SpringJoint.spring = 10;
            SpringJoint.damper = 100;
            SpringJoint.maxDistance = 6;
            SpringJoint.minDistance = 6;
            ropeMode = true;
            if (IsGrappling())
            {
                hook.GetComponent<SpringJoint>().spring = 10;
                hook.GetComponent<SpringJoint>().damper = 100;
                hook.GetComponent<SpringJoint>().minDistance = 6;
                hook.GetComponent<SpringJoint>().maxDistance = 6;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isGrappled)  //keep this in fixed update else it bugs out the editor.
        {
            RaycastHit hit;
            //Debug.DrawLine(gunTip.transform.position, proj.joints[i].transform.position, Color.red);
            if (Physics.Linecast(gunTip.transform.position, proj.joints[i].transform.position, out hit, 100))
            {
                if (hit.transform.gameObject.tag != "hookPoint" && hit.transform.gameObject.tag != "Player" && hit.point != proj.joints[i].transform.position)
                {
                    //Debug.DrawLine(gunTip.position, hit.point - new Vector3(hit.point.x, hit.point.y, 0.5f), Color.blue);
                    var Point = Instantiate(hookPoint, hit.transform);
                    Point.transform.position = hit.point;  //Hit.point is the actual location of impact, not hit.transform.position
                    Point.transform.position = Vector3.MoveTowards(Point.transform.position, gunTip.transform.position, 0.6f);
                    points.Add(Point);
                    proj.joints.Add(Point.GetComponent<SpringJoint>());
                    ProjLine.positionCount = 2 + i + 1;
                    RopeJoints.Insert(i + 1, Point.transform);
                    proj.joints[i + 1].connectedBody = PlayerRb;
                    proj.joints[i].connectedBody = HookHolder;

                    //Debug.Log("Hit");
                    i++;
                    //Debug.Log(i);
                }
            }
            if (points.Count > 1)
            {
                if (!Physics.Linecast(gunTip.transform.position, points[points.Count - 2].transform.position, out hit, 100))
                {
                    if (!Physics.Linecast(gunTip.transform.position, points[points.Count - 1].transform.position, out hit, 100))
                    {
                        Destroy(points[points.Count - 1]);
                        points.Remove(points[points.Count - 1]);
                        proj.joints.Remove(proj.joints[proj.joints.Count - 1]);
                        proj.joints[proj.joints.Count - 1].connectedBody = PlayerRb;
                        RopeJoints.Remove(RopeJoints[RopeJoints.Count - 2]);
                        ProjLine.positionCount -= 1;
                        i--;
                        //Debug.Log(i);
                    }
                }
            }
        }
    }
    void StartGrapple()
    {
        fire.Play();
        if (pM.isSlamming)
        {
            pM.ResetSlam();
        }
        for (int j = 0; j < points.Count; j++)
        {
            Destroy(points[j].gameObject);
        }
        RopeJoints.Clear();
        points.Clear();
        RopeJoints.Clear();
        i = 0;
        //Debug.Log(i);
        hook = Instantiate(hookProj, gunTip);
        grappleObj.SetActive(false);
        lastHook = hook;
        proj = hook.GetComponent<HookJointConnector>();
        ProjLine = hook.GetComponent<LineRenderer>();
        ProjLine.positionCount = 2;
        hookEnd = hook.transform.GetChild(0).transform;
        RopeJoints.Add(hookEnd);
        RopeJoints.Add(gunTip);
        points.Add(hook);

        if (proj != null)
        {
            proj.rootRigidbody = rootRigidbody;
        }
    }

    void StartGrappleMode()
    {
        GameObject target = pM.target2.gameObject;
        if (pM.isSlamming)
        {
            pM.ResetSlam();
        }
        for (int j = 0; j < points.Count; j++)
        {
            Destroy(points[j].gameObject);
        }
        RopeJoints.Clear();
        points.Clear();
        RopeJoints.Clear();
        i = 0;

        hook = Instantiate(hookPoint, target.transform);
        grappleObj.SetActive(false);
        lastHook = hook;
        proj = hook.GetComponent<HookJointConnector>();
        proj.joints.Add(hook.GetComponent<SpringJoint>());
        ProjLine = hook.GetComponent<LineRenderer>();
        ProjLine.positionCount = 2;
        RopeJoints.Add(hook.transform);
        RopeJoints.Add(gunTip);
        points.Add(hook);
        target.SendMessage("Grappled");
        proj.Hit();
        isGrappled = true;

        if (proj != null)
        {
            proj.rootRigidbody = rootRigidbody;
        }
    }

    public bool IsGrappling()
    {
        return hook != null;
    }

    public void StopGrapple()
    {
        fire.Stop();
        enemyGrappled = false;
        interactGrappled = false;
        grappleObj.SetActive(true);
        if (IsGrappling())
        {
            if (lastHook.transform.parent != null && lastHook.transform.parent.tag == "hookPoint")
            {
                Destroy(lastHook.transform.parent.gameObject);
            }
            else
                Destroy(lastHook);
            for (int p = i; p > 0;)
            {
                proj.joints.Remove(proj.joints[p]);
                p--;
            }
            for (int j = 0; j < points.Count; j++)
            {
                Destroy(points[j].gameObject);
            }
            isGrappled = false;
            RopeJoints.Clear();
            points.Clear();
            i = 0;
        }
    }

    IEnumerator GrapplePull()
    {
        if (pL.targeting == true)
        {
            pM.isJumping = false;
            pM.dashMode = true;
            kinematicStatus = false;
            PlayerRb.useGravity = false;
            StopGrapple();
            pL.targeting = false;
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
            PlayerRb.AddForce(transform.up * pM.jumpHeight, ForceMode.Impulse);
            pull.Play();
            StartCoroutine(delayedDashMode(pM.target));
        }
        else if (enemyGrappled)
        {
            enemyRb.AddForce((PlayerRb.transform.position - enemyRb.transform.position) * PullForce * 2f);
            enemyGrappled = false;
            pull.Play();
            return null;
        }
        else if (interactGrappled)
        {
            StopGrapple();
            pull.Play();
            return null;
        }
        else
        {
            Vector3 pullVel = ((lastHook.transform.position - PlayerRb.transform.position) * PullForce);
            PlayerRb.AddForce(pullVel);
            PlayerRb.AddForce(Vector3.up * (PullForce * 0.6f));
            StopGrapple();
            pull.Play();
            return null;
        }
        StopGrapple();
        pull.Play();
        return null;
    }
    IEnumerator delayedDashMode(GameObject target)
    {
        GameObject player = pM.gameObject;
        //this.GetComponent<PlayerLook>().targeting = false;
        Vector3 tempVel = PlayerRb.velocity;
        PlayerRb.velocity = Vector3.zero;
        Vector3 actualTarget = target.transform.position;
        var startPosition = player.transform.position;
        actualTarget.y += 12;
        float timeElapsed = 0;
        float distance = Vector3.Distance(target.transform.position, player.transform.position);
        pM.Orientation.LookAt(target.transform);
        while (timeElapsed < dS.dashingfovTime)
        {
            Cam.m_Lens.FieldOfView = Mathf.Lerp(Cam.m_Lens.FieldOfView, pM.fov + 4f, timeElapsed / dS.dashingfovTime);
            player.transform.position = Vector3.Lerp(startPosition, actualTarget, timeElapsed / Mathf.Min((distance * 0.004f), 0.5f)); //need to eventually make this based on distance rather than time
            timeElapsed += Time.deltaTime;
            yield return null; //prevents instant tp to target.
        }
        pM.doubleJump = true;
        PlayerRb.AddForce(pM.Orientation.forward * ((tempVel.magnitude * 0.8f) + dS.dashForce/* * 1.16f */), ForceMode.Impulse);
        PlayerRb.AddForce(pM.Orientation.up * (tempVel.normalized.y + 50f * 1.2f), ForceMode.Impulse);
        pM.dashMode = false;
        
        yield return waitForJump(tempVel);

        yield return dashModeAfter(target);
    }

    IEnumerator dashModeAfter(GameObject target)
    {
        PlayerRb.useGravity = true;
        //isDashing = false;
        float timeElapsed = 0;

        Vector3 actualTarget = transform.position;
        actualTarget.y -= 12f;
        //target.transform.position = actualTarget;
        target.GetComponent<Collider>().enabled = true;
        target.GetComponent<Rigidbody>().isKinematic = kinematicStatus;

        while (timeElapsed < dS.dashingfovTime)
        {
            Cam.m_Lens.FieldOfView = Mathf.Lerp(Cam.m_Lens.FieldOfView, pM.fov, timeElapsed / dS.dashingfovTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        //wR.wRunningAllowed = true;
        yield break;
    }

    IEnumerator waitForJump(Vector3 tempVel)
    {
        float timeElapsed = 0;

        do
        {
            if (Input.GetKey(KeyCode.Space))
            {
                PlayerRb.AddForce(pM.Orientation.up * ((tempVel.y * 0.7f) + dS.dashForce), ForceMode.Impulse);
                yield break;
            }
            timeElapsed += Time.deltaTime;
        }
        while (timeElapsed < 2f);
        yield return null;
    }

    void SideWeaponPrep()
    {
        sideWeaponOn = true;
    }
    void SideWeaponFire()
    {
        Cam.m_Lens.FieldOfView = 88f;
        bolus = Instantiate(bolusProj, gunTip.position, Player.rotation, null);
    }
    public bool SideWeaponFired()
    {
        return bolus != null;
    }

    private void LateUpdate()
    {
        if (IsGrappling())
        {
            for (int k = 0; k < RopeJoints.Count;)
            {
                ProjLine.SetPosition(k, RopeJoints[k].position);
                //Debug.Log(k);
                k++;
            }
        }
        if (Input.GetMouseButtonUp(0) && !GrappleLocked)
        {
            StopGrapple();
        }
    }
}


// credit https://www.youtube.com/watch?v=XAC8U9-dTZU
// credit https://www.youtube.com/c/WorldofzeroDevelopment