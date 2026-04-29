using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bolusScript : MonoBehaviour
{
    [SerializeField] LayerMask hookAble;
    [SerializeField] GameObject hookPoint;
    [SerializeField] float upForce, forwardForce;
    [SerializeField] SphereCollider sC, sT;
    Rigidbody rb;
    LineRenderer ProjLine;
    bool roped = false;
    private void Awake()
    {
        var cameraRot = GameObject.Find("grapplehook");
        this.transform.rotation = cameraRot.transform.rotation;
        rb = this.GetComponent<Rigidbody>();
        sC.enabled = false;
        sT.enabled = false;
    }
    private void Start()
    {
        StartCoroutine(Enabler());
        rb.AddForce(Vector3.up * upForce, ForceMode.Impulse);
        rb.velocity = transform.forward * forwardForce;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Hit");
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") && !roped)
        {
            var hit = collision.contacts[0].point;
            var Point = Instantiate(hookPoint, collision.transform);
            Point.transform.position = hit;
            Point.transform.parent = collision.transform;
            var joint = Point.GetComponent<SpringJoint>();
            joint.connectedBody = rb;
            ProjLine = Point.GetComponent<LineRenderer>();
            ProjLine.positionCount = 2;
            ProjLine.SetPosition(0, Point.transform.position);
            sC.enabled = false;
            roped = true;
            rb.velocity = transform.forward * forwardForce;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Boom!");
        ProjLine.positionCount = 0;
        Destroy(this.gameObject);
        Destroy(this);
    }

    private void LateUpdate()
    {
        if (roped)
        {
            ProjLine.SetPosition(1, this.transform.position);
        }
    }

    IEnumerator Enabler()
    {
        yield return new WaitForSeconds(0.2f);
        sC.enabled = true;
        sT.enabled = true;
    }    
}
