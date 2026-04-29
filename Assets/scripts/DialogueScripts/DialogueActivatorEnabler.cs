using UnityEngine;

public class DialogueActivatorEnabler : MonoBehaviour
{
    [SerializeField] private DialogueObject dialogueObject;
    [SerializeField] private DialogueUI dialogueUI;
    //private Animator emAnim;
    //public GameObject Emoter;
    public bool playOnce = false;

    //public Vector3 PlayerPosition;
    //public Vector3 ReturnPosition;

    private void OnEnable()
    {
        dialogueUI.ShowDialogue(dialogueObject, true);
        //player.Interactable = this;
        playOnce = true;
    }

    public void UpdateDialogueObject(DialogueObject dialogueObject)
    {
        this.dialogueObject = dialogueObject;
    }

    /*private void OnTriggerExit(Collider other)
    {
        emAnim.SetBool("PurpleDia", false);
        Emoter.SetActive(false);
        if (other.CompareTag("Player"))
        {
            if(player.Interactable is DialogueActivator dialogueActivator && dialogueActivator == this)
            {
                player.Interactable = null;
            }
        }
    }*/
    /*public void Interact(PlayerMovement player)
    {
        //Emoter.SetActive(false);
        foreach (DialogueResponseEvents responseEvents in GetComponents<DialogueResponseEvents>())
        {
            if (responseEvents.DialogueObject == dialogueObject)
            {
                player.DialogueUI.AddResponseEvents(responseEvents.Events);
                break;
            }
        }
        //PlayerPosition = GameObject.Find("Player").transform.position;
        //transform.localRotation = Quaternion.Euler(PlayerPosition);
        player.DialogueUI.ShowDialogue(dialogueObject);
        //transform.localRotation = Quaternion.Euler(ReturnPosition);
    }*/
}
