using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class bullseyeStompScript : MonoBehaviour
{
    [SerializeField] PlayableDirector cutscenetoStart, cutscenetoUnpause;
    [SerializeField] GameObject bounceDetector;

    public void pause()
    {
        cutscenetoUnpause.playableGraph.GetRootPlayable(0).SetSpeed(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Wah");
            cutscenetoUnpause.playableGraph.GetRootPlayable(0).SetSpeed(1);
            cutscenetoStart.enabled = true;
            other.GetComponent<PlayerMovement>().ResetSlam();
            Rigidbody rb = other.GetComponent<Rigidbody>();
            rb.velocity = new Vector3 (rb.velocity.x, 0, rb.velocity.z);
            other.GetComponent<Rigidbody>().AddForce(transform.up * 2400);
            gameObject.SetActive(false);
        }
    }
}
