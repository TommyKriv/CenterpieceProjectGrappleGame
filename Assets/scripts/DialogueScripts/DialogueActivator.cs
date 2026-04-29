using UnityEngine;
using UnityEngine.Playables;
using System.Collections;
using System.Collections.Generic;

public class DialogueActivator : MonoBehaviour
{
    [SerializeField] float PauseLength;
    [SerializeField] private DialogueObject dialogueObject;
    [SerializeField] private DialogueUI dialogueUI;
    //private Animator emAnim;
    //public GameObject Emoter;
    public bool playOnce = false;
    public bool instant;

    private Vector3 PlayerPosition;
    private Vector3 ReturnPosition;
    public void Start()
    {
        //Emoter.SetActive(false);
        //emAnim = Emoter.GetComponent<Animator>();
        if (GetComponentInParent<PlayableDirector>())
        {
            PlayableDirector director;
            director = GetComponentInParent<PlayableDirector>();
        }
        ReturnPosition = this.transform.position;
    }
    public void UpdateDialogueObject(DialogueObject dialogueObject)
    {
        this.dialogueObject = dialogueObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !playOnce)
        {
            Activate();
        }

    }

    public void Activate() //Seperated from the onTrigger for signal emitters.
    {
        StartCoroutine(Pause(PauseLength));
        playOnce = true;
    }

    IEnumerator Pause(float length)
    {
        yield return new WaitForSeconds(length);
        dialogueUI.ShowDialogue(dialogueObject, instant);
        yield break;
    }

}
