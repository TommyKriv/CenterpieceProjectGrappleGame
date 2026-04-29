using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dashHitScript : MonoBehaviour
{
    [SerializeField] dashScript ds;
    [SerializeField] Rigidbody PlayerRb;
    [SerializeField] float bumpUpForce;
    [SerializeField] float bumpForce;

    [SerializeField] LayerMask groundMask;

    [SerializeField] PlayerMovement Pm;


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Enemy" || other.gameObject.tag == "Light")
        {
            other.gameObject.SetActive(false);
            PlayerRb.AddForce(Vector3.up * bumpUpForce, ForceMode.Impulse);
            PlayerRb.AddForce(Pm.Orientation.transform.forward * bumpForce, ForceMode.Impulse);
        }
/*        else if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Vector3 dir = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position) - transform.position;

            dir = -dir.normalized;
            Debug.Log("bonk");
            PlayerRb.velocity = new Vector3(0, 0, 0);

            PlayerRb.AddForce(dir * bumpForce, ForceMode.Impulse);
            PlayerRb.AddForce(Vector3.up * bumpUpForce, ForceMode.Impulse);
            ds.ResetDash();
        }*/
    }
}
