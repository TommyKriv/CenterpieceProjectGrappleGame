using UnityEngine;

public class BullseyeHeadGrapple : MonoBehaviour
{
    [SerializeField] PlayerMovement pM;
    [SerializeField] PlayerLook pL;
    [SerializeField] GrappleGun1 Ggun;
    [SerializeField] GameObject finalCutscene;
    bool pullCheck;
    public void Grappled() //Works because is tagged as interactable
    {
        Ggun.interactGrappled = true;
        pullCheck = true;
    }

    private void Update()
    {
        if (pullCheck == true)
        {
            if (Input.GetMouseButtonDown(1))
            {
                pL.enabled = false;
                pM.enabled = false;
                Ggun.enabled = false;
                pM.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                pM.gameObject.SetActive(false);
                pL.gameObject.SetActive(false);
                finalCutscene.SetActive(true);
                pullCheck = false;
            }
        }
    }
}
