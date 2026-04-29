using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class punchHitScript : MonoBehaviour
{
    [SerializeField] Rigidbody PlayerRb;
    [SerializeField] float boingForce;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "Light")
        {
            PlayerRb.AddForce(PlayerRb.transform.up * boingForce, ForceMode.Impulse);
        }
    }
}
