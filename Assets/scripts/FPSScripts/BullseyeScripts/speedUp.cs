using Cinemachine;
using UnityEngine;

public class speedUp : MonoBehaviour
{
    private PlayerMovement pM;
    [SerializeField] CinemachineVirtualCamera Vcam;
    [SerializeField] float zoom;
    [SerializeField] GameObject target, wall;
    private void OnTriggerEnter(Collider other)
    {
        pM = other.GetComponent<PlayerMovement>();
        if (other.CompareTag("Player"))
        {
            pM.moveSpeed = pM.moveSpeed * 1.8f;
            pM.grindSpeed = pM.grindSpeed * 1.8f;
            pM.jumpHeight = pM.jumpHeight * 2f;
            pM.grindJumpMult = pM.grindJumpMult * 1.4f;
            pM.fallSpeedCap = pM.fallSpeedCap * 1.3f;
            pM.targetDist = pM.targetDist * 1.5f;
            pM.fov = zoom;
            Vcam.m_Lens.FieldOfView = Mathf.Lerp(Vcam.m_Lens.FieldOfView, zoom, 1f * Time.deltaTime);
            wall.SetActive(false);
            target.SetActive(true);

            Destroy(this);
        }
    }
}
