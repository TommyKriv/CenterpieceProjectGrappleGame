using UnityEngine;

public class slideHitScript : MonoBehaviour
{
    [SerializeField] PlayerMovement pM;
    private FMODUnity.StudioEventEmitter aud;
    private float startTimer = 0.25f;

    private void Start()
    {
        aud = GetComponent<FMODUnity.StudioEventEmitter>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            //Debug.Log("AGH");
            other.SendMessage("Slidden");
        }
    }

    private void Update()
    {
        startTimer -= Time.deltaTime;
        if (!pM.isGrounded && startTimer < 0 && aud.IsPlaying())
        {
            aud.Stop();
        }
        if (pM.isGrounded && !aud.IsPlaying())
        {
            aud.Play();
        }
    }
}
