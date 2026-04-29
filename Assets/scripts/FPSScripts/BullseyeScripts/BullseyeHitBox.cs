using UnityEngine;
using UnityEngine.SceneManagement;

public class BullseyeHitBox : MonoBehaviour
{
    [SerializeField] GrappleGun1 g1;
    public float force, upForce; //whats upforce!
    public bool slamming, leftSwing, rightSwing;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && other.gameObject.layer == 9)
        {
            if(slamming)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            if(leftSwing)
            {
                g1.StopGrapple();
                other.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(-force, upForce, 0), ForceMode.Impulse);
            }
            if (rightSwing)
            {
                g1.StopGrapple();
                other.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(force, upForce, 0), ForceMode.Impulse);
            }
        }
    }

    public void resetBool()
    {
        slamming = false;
        leftSwing = false;
        rightSwing = false;
    }
}
