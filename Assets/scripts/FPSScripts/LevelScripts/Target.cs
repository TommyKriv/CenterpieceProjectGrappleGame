using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] PlayerMovement Pm;
    [SerializeField] GrappleGun1 Ggun;

    [SerializeField] int frequency;
    [SerializeField] float intensity;

    public bool destructable;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Pm.doubleJump = true;

            if(destructable)
            { 
                Destroy(this.gameObject);
            }
        }
    }

    void Grappled()
    {
        Ggun.enemyRb = this.GetComponent<Rigidbody>();
        Ggun.enemyGrappled = true;
    }
}
