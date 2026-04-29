using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceThirdPerson : MonoBehaviour
{
    [SerializeField] ThirdPersonMovement Pm;
    [SerializeField] Rigidbody PlayerRb;
    [SerializeField] float bumpUpForce;
    [SerializeField] Camera Cam;
    [SerializeField] LayerMask groundMask;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Light")
        {
            return;
        }
        else if (other.gameObject.tag == "BouncePad")
        {
            PlayerRb.AddForce(Vector3.up * bumpUpForce * 10, ForceMode.Impulse);

            Pm.ResetSlam();
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Ground") && !Input.GetButton("Crouch"))
        {
            PlayerRb.AddForce(Vector3.up * bumpUpForce, ForceMode.Impulse);
            if (Input.GetKey(KeyCode.W))
            {
                PlayerRb.AddForce(Vector3.forward * 1.16f, ForceMode.Impulse);
            }
            Pm.ResetSlam();
        }
        else
        {
            Pm.Slide();
            Pm.ResetSlam();
        }
    }
}
