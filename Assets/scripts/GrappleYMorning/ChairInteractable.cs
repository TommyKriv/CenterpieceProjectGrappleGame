using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ChairInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueObject dialogueObject;
    [SerializeField] private DialogueUI dialogueUI;
    //private Animator emAnim;
    //public GameObject Emoter;
    public bool playOnce = false;
    public void UpdateDialogueObject(DialogueObject dialogueObject)
    {
        this.dialogueObject = dialogueObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.transform.parent.gameObject.TryGetComponent(out ThirdPersonMovement player))
        {
            player.Interactable = this;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //emAnim.SetBool("PurpleDia", false);
        //Emoter.SetActive(false);
        if (other.CompareTag("Player") && other.transform.parent.gameObject.TryGetComponent(out ThirdPersonMovement player))
        {
#pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (player.Interactable == this)
#pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                player.Interactable = null;
            }
        }
    }


    public void Interact(PlayerMovement player)
    {

        foreach (DialogueResponseEvents responseEvents in GetComponents<DialogueResponseEvents>())
        {
            if (responseEvents.DialogueObject == dialogueObject)
            {
                player.DialogueUI.AddResponseEvents(responseEvents.Events);
                break;
            }
        }
        player.DialogueUI.ShowDialogue(dialogueObject, false);
    }

    public void Interact(ThirdPersonMovement player)
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
        //PlayerPosition = player.gameObject.transform.position;
        //transform.localRotation = Quaternion.Euler(PlayerPosition);
        player.DialogueUI.ShowDialogue(dialogueObject, false);
        player.pCam.GetComponent<CinemachineFreeLook>().LookAt = gameObject.transform.parent.transform;
        player.gameObject.transform.GetChild(0).gameObject.transform.LookAt(gameObject.transform.parent.transform);
        player.pCam.GetComponent<ThirdPersonCam>().enabled = false;
        player.enabled = false;
        //transform.localRotation = Quaternion.Euler(ReturnPosition);
    }
}