using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneTransition : MonoBehaviour, IInteractable
{
    [SerializeField] Vector3 checkPointLoc;
    [SerializeField] float yRotation;
    [SerializeField] string nextScene;
    [SerializeField] private DialogueUI dialogueUI;
    private gameMaster gm;

    private GameObject intUI;

    private void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<gameMaster>();
        intUI = dialogueUI.transform.GetChild(1).gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.transform.parent.gameObject.TryGetComponent(out ThirdPersonMovement player))
        {
            player.Interactable = this;
            intUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //emAnim.SetBool("PurpleDia", false);
        //Emoter.SetActive(false);
        if (other.CompareTag("Player") && other.transform.parent.gameObject.TryGetComponent(out ThirdPersonMovement player))
        {
            if (player.Interactable is DialogueActivator dialogueActivator && dialogueActivator == this)
            {
                player.Interactable = null;
                intUI.SetActive(false);
            }
        }
    }

    public void Interact(PlayerMovement player)
    {
        gm.lastCheckPoint = checkPointLoc;
        gm.yRotation = yRotation;
        SceneManager.LoadScene(nextScene);
        intUI.SetActive(false);
    }

    public void Interact(ThirdPersonMovement player)
    {
        gm.lastCheckPoint = checkPointLoc;
        gm.yRotation = yRotation;
        SceneManager.LoadScene(nextScene);
        intUI.SetActive(false);
    }
}
