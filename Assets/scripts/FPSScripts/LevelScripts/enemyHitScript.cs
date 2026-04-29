using UnityEngine;

public class enemyHitScript : MonoBehaviour
{
    [SerializeField] PlayerMovement pm;
    [SerializeField] dashScript ds;
    [SerializeField] GrappleGun1 Ggun;
    [SerializeField] GameObject slideDetector;
    public int upForce, backForce;
    [SerializeField] float rotationSpeed;

    Vector3 colScale;
    Rigidbody rb;
    float height;
    public bool isGrounded;
    public bool targetable;
    public bool stationary;

    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        colScale = this.GetComponent<Collider>().transform.localScale;
        height = this.GetComponent<Collider>().bounds.extents.y;
        //originalRot = transform.rotation;
    }
    private void Update()
    {
        if(!stationary)
        {
            if (Physics.Raycast(transform.position, Vector3.down, height * 1.2f))
            {
                IsGrounded = true;
            }
            else
                IsGrounded = false;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Player" && !stationary && ds.isDashing)
        {
            Vector3 dir = collision.contacts[0].point - transform.position;
            dir = -dir.normalized;
            rb.constraints = RigidbodyConstraints.None;
            rb.AddForce(dir * backForce * 1.6f, ForceMode.Impulse);
            rb.transform.localScale = (colScale * 2f);
            targetable = true;
        }
    }

    void Grappled()
    {
        Ggun.enemyRb = rb;
        Ggun.enemyGrappled = true;
        if(!stationary)
        {
            rb.constraints = RigidbodyConstraints.None;
        }  
    }

    private bool IsGrounded
    {
        get { return isGrounded; }
        set
        {
            if (isGrounded == false && value == true)
            {
                targetable = false;
            }
            else if(isGrounded == true && value == false)
                {
                    targetable = true;
                }

            isGrounded = value;
        }
    }

    void Slidden()
    {
        //Debug.Log("Guh");
        Vector3 dir = pm.Orientation.transform.forward;
        rb.constraints = RigidbodyConstraints.None;

        rb.AddForce(dir * pm.gameObject.GetComponentInParent<Rigidbody>().velocity.magnitude, ForceMode.Impulse);
        rb.velocity += new Vector3(0, upForce);
        targetable = true;
    }
}
