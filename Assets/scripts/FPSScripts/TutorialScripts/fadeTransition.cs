using UnityEngine;

public class fadeTransition : MonoBehaviour
{
    public float transitionNum;
    [SerializeField] Vector3 position;
    [SerializeField] GameObject fadeOutCutscene, grapple;
    private PlayerMovement player;
    private gameMaster gM;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerMovement>();
        gM = GameObject.FindGameObjectWithTag("GM").GetComponent<gameMaster>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            fadeOutCutscene.SetActive(true);
            //player.GetComponent<PlayerMovement>().enabled = false;
        }
    }

    public void movePlayer()
    {
        PlayerMovement pM = player.GetComponent<PlayerMovement>();
        pM.enabled = true;
        if (transitionNum == 1)
        {
            grapple.SetActive(true);
        }
        else
        {
            pM.canDj = true;
            DoubleJumpIndicator pmDj = player.GetComponent<DoubleJumpIndicator>();
            pmDj.enabled = true;
            pmDj.dJInd1.SetActive(true);
            pmDj.dJInd2.SetActive(true);
        }
        player.moveSpeed += 1.5f;
        player.jumpHeight += 10f;
        player.slideForce += 15f;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player.transform.position = position;
        gM.lastCheckPoint = position;
    }
}
