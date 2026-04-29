using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class DialogueInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueObject dialogueObject;
    [SerializeField] private DialogueUI dialogueUI;
    //private Animator emAnim;
    //public GameObject Emoter;
    public bool playOnce = false;

    private Vector3 PlayerPosition;
    private Vector3 ReturnPosition;

    private GameObject intUI;

    public void Start()
    {
        intUI = dialogueUI.transform.GetChild(1).gameObject;
        //Emoter.SetActive(false);
        //emAnim = Emoter.GetComponent<Animator>();
        ReturnPosition = this.transform.position;
    }
    public void UpdateDialogueObject(DialogueObject dialogueObject)
    {
        this.dialogueObject = dialogueObject;
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
#pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (player.Interactable == this)
#pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                player.Interactable = null;
                intUI.SetActive(false);
            }
        }
    }


    public void Interact(PlayerMovement player)
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
        intUI.SetActive(false);
        player.DialogueUI.ShowDialogue(dialogueObject, false);
        //transform.localRotation = Quaternion.Euler(ReturnPosition);
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
        intUI.SetActive(false);
        player.DialogueUI.ShowDialogue(dialogueObject, false);
        player.pCam.GetComponent<CinemachineFreeLook>().LookAt = gameObject.transform.parent.transform;
        player.gameObject.transform.GetChild(0).gameObject.transform.LookAt(gameObject.transform.parent.transform);
        player.pCam.GetComponent<ThirdPersonCam>().enabled = false;
        player.enabled = false;
        //transform.localRotation = Quaternion.Euler(ReturnPosition);
    }
}
