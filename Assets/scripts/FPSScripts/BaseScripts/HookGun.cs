using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookGun : MonoBehaviour
{

    [SerializeField] Rigidbody PlayerRb;
    [SerializeField] float jSpring;
    [SerializeField] float jDamper;
    [SerializeField] float jMassScale;
    [SerializeField] float PullForce;
    [SerializeField] LayerMask grappleables, Pullable, MovingPlatform;
    [SerializeField] Transform gunTip, Camera, Player;

    private LineRenderer lineRend;
    private float maxDistance = 250f;
    private SpringJoint joint;
    private Vector3 grapplePoint;
    private float distanceToHook;
    private Vector3 directionToHook;

    void Awake()
    {
        lineRend = GetComponent<LineRenderer>();
    }

    private void Update()
    {

        if(Input.GetMouseButtonDown(0))
        {
            StartGrapple();
        }
        else if(Input.GetMouseButtonUp(0))
        {
            StopGrapple();
        }
        if(IsGrappling() && Input.GetMouseButtonDown(1))
        {
            GrapplePull();
        }
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    void StartGrapple()
    {
        RaycastHit hit;
        if(Physics.Raycast(Camera.position, Camera.forward, out hit, maxDistance, grappleables))
        {
            grapplePoint = hit.point;
            joint = Player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(Player.position, grapplePoint);

            joint.maxDistance = distanceFromPoint * 0.6f;
            joint.minDistance = distanceFromPoint * 0.25f;

            joint.spring = jSpring;
            joint.damper = jDamper;
            joint.massScale = jMassScale;

            lineRend.positionCount = 2;
            currentGrapplePosition = gunTip.position;
        }
    }

    private Vector3 currentGrapplePosition;

    void DrawRope()
    {
        if (!joint) return;
            currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);

            lineRend.SetPosition(0, gunTip.position);
            lineRend.SetPosition(1, grapplePoint);
    }

    void StopGrapple()
    {
        lineRend.positionCount = 0;
        Destroy(joint);
    }

    public bool IsGrappling()
    {
        return joint != null;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }

    void GrapplePull()
    {
            PlayerRb.AddForce((grapplePoint - PlayerRb.transform.position) * PullForce);
            StopGrapple();
    }
}

// credit https://www.youtube.com/watch?v=XAC8U9-dTZU