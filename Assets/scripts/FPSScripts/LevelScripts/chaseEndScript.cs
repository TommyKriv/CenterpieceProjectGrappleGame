using UnityEngine;

public class chaseEndScript : MonoBehaviour
{
    [SerializeField] GameObject earthShaker;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            earthShaker.SetActive(true);
        }
    }
}
