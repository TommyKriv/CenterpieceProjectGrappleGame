using UnityEngine;

public class HookProjectile : MonoBehaviour
{
    public float speed;
    public float maxRange;
    [SerializeField] GameObject hookPoint;
    [SerializeField] LayerMask hookAble;

    private float distanceMoved;
    [SerializeField] Transform projTrans;
    Vector3 originalScale;
    public GameObject GrappleGun;
    public GrappleGun1 g1;
    [SerializeField] FMODUnity.StudioEventEmitter hookFire, hookImpact;

    void Start()
    {
        this.transform.parent = null;
        speed = speed + GameObject.Find("Player").GetComponent<PlayerMovement>().moveSpeed;
        maxRange = maxRange + GameObject.Find("Player").GetComponent<PlayerMovement>().moveSpeed;
        GrappleGun = GameObject.Find("grapplehook");
        g1 = GrappleGun.GetComponent<GrappleGun1>();
        hookFire.Play();
    }
    void Update()
    {
        float travelDistance = speed * Time.deltaTime;
        distanceMoved += travelDistance;
        Ray ray = new Ray(this.transform.position, this.transform.forward);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, travelDistance, hookAble))
        {
            travelDistance = hit.distance;
            var Point = Instantiate(hookPoint);
            Point.transform.position = this.transform.position;
            this.transform.position += this.transform.forward * (travelDistance - 0.95f);
            Point.transform.parent = hit.transform;
            this.transform.parent = Point.transform;

            if(hit.transform.gameObject.tag == "Enemy" || hit.transform.gameObject.tag == "Interactable")
            {
                var target = hit.transform.gameObject;
                target.SendMessage("Grappled");
                target.GetComponent<Rigidbody>().AddForce(-this.gameObject.transform.forward * 22f, ForceMode.Impulse);
            }

            SendMessage("Hit"); //sending message over to hookJoint connector;
            g1.isGrappled = true;
            speed = 0;
            this.GetComponent<BoxCollider>().enabled = false;
            //this.GetComponent<Rigidbody>().velocity = Vector3.zero;
            hookFire.Stop();
            hookImpact.Play();
            Destroy(this);
        }
        else
        {
            this.transform.position += this.transform.forward * travelDistance;
            if(distanceMoved > maxRange)
            {
                for (int j = 0; j < g1.points.Count; j++)
                {
                    Destroy(g1.points[j].gameObject);
                }
                g1.points.Clear();

                Destroy(this.gameObject);
            }
        }
    }
}
