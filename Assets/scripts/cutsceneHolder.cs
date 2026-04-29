using UnityEngine;
using UnityEngine.Playables;

public class cutsceneHolder : MonoBehaviour
{
    [SerializeField] PlayableDirector cutsceneToHold;
    [SerializeField] bool screenShake;
    public bool hitMarker = false;
    public bool paused = false;

    public void pause()
    {
        if (!hitMarker)
        {
            cutsceneToHold.playableGraph.GetRootPlayable(0).SetSpeed(0);
            paused = true;
        }
        else
        {
            hitMarker = false;
            return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hitMarker = true;
            if (paused)
            {
                cutsceneToHold.playableGraph.GetRootPlayable(0).SetSpeed(1);
            }
            else
            {
                cutsceneToHold.time = 50;
            }

            if(screenShake)
            {
                other.GetComponent<Rigidbody>().AddForce(other.transform.up * 2000, ForceMode.Impulse);
            }
        }
    }
}
