using System.Collections;
using UnityEngine;
using Cinemachine;

public class jumpHitScript : MonoBehaviour
{
    [SerializeField] PlayerMovement Pm;
    [SerializeField] Rigidbody PlayerRb;
    [SerializeField] float bumpUpForce;
    [SerializeField] CinemachineVirtualCamera Cam;
    [SerializeField] LayerMask groundMask;
    [SerializeField] GameObject bouncer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Light")
        {
            return;
        }
        else if(other.gameObject.layer == 13)
        {
            other.SendMessage("Snap");
            return;
        }
        else if (other.gameObject.tag == "BouncePad")
        {
            Pm.ResetSlam();
            PlayerRb.AddForce(Vector3.up * bumpUpForce * 10, ForceMode.Impulse);

            gameObject.SetActive(false);
            return;
        }
        if(other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if (Input.GetButton("Crouch") || Pm.OnSlope()) //Checking for crouch input to slide instead of bounce
            {
                Pm.ResetSlam();
                Pm.Slide();
                gameObject.SetActive(false);
            }
            else
            {
                bouncer.SetActive(true);
                StartCoroutine(cameraZoom());
                Pm.ResetSlam();
                PlayerRb.AddForce(Vector3.up * bumpUpForce, ForceMode.Impulse);
                if (Input.GetKey(KeyCode.W))
                {
                    PlayerRb.AddForce(Vector3.forward * 1.16f, ForceMode.Impulse);
                }
                gameObject.SetActive(false);
            }
        }
    }

    IEnumerator cameraZoom()
    {
        float timeElapsed = 0;
        while (timeElapsed < 0.45f) //ALWAYS LAST
        {
            Cam.m_Lens.FieldOfView = Mathf.Lerp(Cam.m_Lens.FieldOfView, 89.5f, timeElapsed / 0.45f);
            timeElapsed += Time.deltaTime;
        }
        yield return null;
    }
}