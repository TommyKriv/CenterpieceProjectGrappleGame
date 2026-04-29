using Cinemachine;
using System.Collections;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Playables;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private cutscenePauser cutscenePauser;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text textLabel;
    [SerializeField] KeyCode DialogueInputKey = KeyCode.F;

    public bool IsOpen { get; private set; } = false;
    private ResponseHandler responseHandler;
    private TypewriterEffect typewriterEffect;
    private bool running;

    private void Start()
    {
        typewriterEffect = GetComponent<TypewriterEffect>();
        responseHandler = GetComponent<ResponseHandler>();

        CloseDialogueBox();
    }

    public void ShowDialogue(DialogueObject dialogueObject, bool instant)
    {
        if (running == true)
        {
            typewriterEffect.Stop();
            StopAllCoroutines();
            CloseDialogueBox();

            running = false;
        }
        IsOpen = true;
        dialogueBox.SetActive(true);
        StartCoroutine(StepThroughDialogue(dialogueObject, instant));
    }

    public void AddResponseEvents(ResponseEvent[] responseEvents)
    {
        responseHandler.AddResponseEvents(responseEvents);
    }

    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject, bool instant)
    {
        running = true;
        for(int i=0; i <dialogueObject.Dialogue.Length; i++)
        {
            string dialogue = dialogueObject.Dialogue[i];

            if(!instant)
            {
                yield return RunTypingEffect(dialogue);

                yield return null;
                if (dialogueObject.interactable)
                {
                    yield return new WaitUntil(() => Input.GetKeyDown(DialogueInputKey));
                }
                else
                    yield return new WaitForSeconds(1.1f);
            }

            textLabel.text = dialogue;
            if (i == dialogueObject.Dialogue.Length - 1 && dialogueObject.HasResponses) break;
        }
        if (dialogueObject.HasResponses)
        {
            responseHandler.ShowResponses(dialogueObject.Responses);
        }
        else
        { 
            if (dialogueObject.unPause)
            {
                cutscenePauser.replied = true;
                if (cutscenePauser.paused)
                {
                    cutscenePauser.currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
                    cutscenePauser.paused = false;
                    cutscenePauser.replied = false;
                }
            }
            if(!dialogueObject.hold)
            {
                CloseDialogueBox();
            }
        }
        running = false;
    }

    private IEnumerator RunTypingEffect(string dialogue)
    {
        typewriterEffect.Run(dialogue, textLabel);

        while(typewriterEffect.IsRunning)
        {
            yield return null;

            if(Input.GetKeyDown(DialogueInputKey))
            {
                typewriterEffect.Stop();
                textLabel.text = dialogue;
            }
        }
    }

    public void CloseDialogueBox()
    {
        GameObject Player = GameObject.Find("Player");
        if (Player.GetComponent<ThirdPersonMovement>() && Player.GetComponent<ThirdPersonMovement>().enabled == false)
        {
            ThirdPersonMovement tPM = Player.GetComponent<ThirdPersonMovement>();
            tPM.enabled = true;
            tPM.pCam.GetComponent<ThirdPersonCam>().enabled = true;
            tPM.pCam.gameObject.GetComponent<CinemachineFreeLook>().LookAt = Player.transform;
        }
            
        IsOpen = false;
        dialogueBox.SetActive(false);
        textLabel.text = string.Empty;
    }
}
