using UnityEngine;
using UnityEngine.SceneManagement;

public class KillWall : MonoBehaviour
{
    [SerializeField] GrappleGun1 g1;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if (other.gameObject.tag == "HookProj")
        {
            g1.StopGrapple();
        }
    }
}
