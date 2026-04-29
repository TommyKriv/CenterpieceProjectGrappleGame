using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleThirdPerson : MonoBehaviour
{
    [SerializeField] ThirdPersonMovement pm;
    [SerializeField] ThirdPersonCam pL;
    [SerializeField] Camera Camera;
    [SerializeField] Transform gunTip, Player;
    [SerializeField] Rigidbody PlayerRb, HookHolder;
    [SerializeField] GameObject hookProj, hookPoint, bolusProj;
    [SerializeField] float PullForce, baseFOV;
    public Rigidbody rootRigidbody, enemyRb;

    public List<GameObject> points;
    public List<Transform> RopeJoints;
    private GameObject lastHook, hook, bolus;
    private HookJointConnector proj;

    private LineRenderer ProjLine;

    private int i = 0;

    public bool isGrappled = false, sideWeaponOn = false, enemyGrappled = false, enemyModed = false, GrappleLocked, interactGrappled;

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

        if (IsGrappling() == true && !GrappleLocked)
        {
            if (Input.GetMouseButtonDown(1))
            {
                GrapplePull(lastHook.transform.position);
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
        if (pm.isSlamming)
        {
            pm.ResetSlam();
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
        lastHook = hook;
        proj = hook.GetComponent<HookJointConnector>();
        ProjLine = hook.GetComponent<LineRenderer>();
        ProjLine.positionCount = 2;
        RopeJoints.Add(hook.transform);
        RopeJoints.Add(gunTip);
        points.Add(hook);

        if (proj != null)
        {
            proj.rootRigidbody = rootRigidbody;
        }
    }

    void StartGrappleMode()
    {
        GameObject target = pm.target2.gameObject;
        if (pm.isSlamming)
        {
            pm.ResetSlam();
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
        enemyGrappled = false;
        interactGrappled = false;
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

    void GrapplePull(Vector3 pullLocation)
    {
        if (isGrappled)
        {
            Camera.fieldOfView = 88;
            if (enemyGrappled)
            {
                enemyRb.AddForce((PlayerRb.transform.position - enemyRb.transform.position) * PullForce * 0.7f * 2f);
                enemyGrappled = false;
            }
            else if (interactGrappled)
            {
                StopGrapple();
                return;
            }
            else
            {
                PlayerRb.AddForce((pullLocation - PlayerRb.transform.position) * PullForce);
                PlayerRb.AddForce(Vector3.up * (PullForce * 0.6f));
            }
            StopGrapple();
        }
        else return;
    }

    void SideWeaponPrep()
    {
        sideWeaponOn = true;
    }
    void SideWeaponFire()
    {
        Camera.fieldOfView = 88f;
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
