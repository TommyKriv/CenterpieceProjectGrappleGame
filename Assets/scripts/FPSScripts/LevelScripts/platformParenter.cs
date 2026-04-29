using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class platformParenter : MonoBehaviour
{
    private GameObject player;
    private Vector3 offset;
    public bool delay;

    private void Start()
    {
        player = null;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            player = other.gameObject;
            offset = player.transform.position - transform.position;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            player = null;
        }
    }

   private void FixedUpdate() //feels better when in fixed but looks bad when platform being raised when not in late.
    {
        if(player != null && !delay)
        {
            player.transform.position = transform.position + offset;
        }
    }

    private void LateUpdate()
    {
        if (player != null && delay)
        {
            player.transform.position = transform.position + offset;
        }
    }

    public void swapper()
    {
        if (delay)
        {
            delay = false;
        }
        else
            delay = true;
    }
}
