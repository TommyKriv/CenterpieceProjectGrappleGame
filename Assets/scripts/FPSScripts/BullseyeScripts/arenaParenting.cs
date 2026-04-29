using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arenaParenting : MonoBehaviour
{
    private GameObject player;
    private Vector3 offset;
    private bool delay;

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

    /*private void FixedUpdate() //feels better when in fixed but looks bad when platform being raised when not in late.
    {
        if(player != null && !delay)
        {
            player.transform.position = transform.position + offset;
        }
    }*/

    private void LateUpdate()
    {
        if (player != null && delay)
        {
            player.transform.position = transform.position + offset;
        }
    }

    public IEnumerator rightAttackDelay()
    {
        yield return new WaitForSeconds(4.2f);
        delay = true;
        yield return new WaitForSeconds(1.2f);
        delay = false;
        yield return new WaitForSeconds(0.8f);
        delay = true;
        yield return new WaitForSeconds(1.4f);
        yield return delay = false;
    }

    public IEnumerator upAttackDelay()
    {
        yield return new WaitForSeconds(4.2f);
        delay = true;
        yield return new WaitForSeconds(1.2f);
        delay = false;
        yield return new WaitForSeconds(0.8f);
        delay = true;
        yield return new WaitForSeconds(1.4f);
        yield return delay = false;
    }
}
