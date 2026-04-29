using UnityEngine;

public class TargetHallwayActivator : MonoBehaviour
{
    [SerializeField] GameObject target, wall;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            wall.SetActive(false);
            target.SetActive(true);
        }
    }
}
