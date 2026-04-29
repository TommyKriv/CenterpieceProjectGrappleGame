using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Playables;

public class ResponseHandler : MonoBehaviour
{
    [SerializeField] private cutscenePauser cutscenePauser;
    [SerializeField] private RectTransform responseBox;
    [SerializeField] private RectTransform responseButtonTemplate;
    [SerializeField] private RectTransform responseContainer;

    private DialogueUI dialogueUI;
    private ResponseEvent[] responseEvents;

    private List<GameObject> tempResponseButtons = new List<GameObject>();

    private void Start()
    {
        dialogueUI = GetComponent<DialogueUI>();
    }

    public void AddResponseEvents(ResponseEvent[] responseEvents)
    {
        this.responseEvents = responseEvents;
    }

    public void ShowResponses(Response[] responses)
    {
        Cursor.lockState = CursorLockMode.None;  //REQUIRES EVENTHANDLER TO HAVE WORKING BUTTONS SILLY!!
        Cursor.visible = true;
        float responseBoxHeight = 0;

        for (int i=0; i < responses.Length; i++)
        {
            Response response = responses[i];
            int responseIndex = i;

            GameObject responseButton = Instantiate(responseButtonTemplate.gameObject, responseContainer);
            responseButton.gameObject.SetActive(true);
            responseButton.GetComponentInChildren<TMP_Text>().text = response.ResponseText;
            if(response.unPauser)
            {
                responseButton.GetComponentInChildren<Button>().onClick.AddListener(() => OnPickedResponse(response, responseIndex, true));
            }
            else
                responseButton.GetComponentInChildren<Button>().onClick.AddListener(() => OnPickedResponse(response, responseIndex, false));

            tempResponseButtons.Add(responseButton);

            responseBoxHeight += responseButtonTemplate.sizeDelta.y;
        }

        responseBox.sizeDelta = new Vector2(responseBox.sizeDelta.x, responseBoxHeight);
        responseBox.gameObject.SetActive(true);
    }

    private void OnPickedResponse(Response response, int responseIndex, bool unPause)
    {
        if(unPause)
        {
            cutscenePauser.replied = true;
            if(cutscenePauser.paused)
            {
                cutscenePauser.currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
                cutscenePauser.paused = false;
                cutscenePauser.replied = false;
            }
        }

        responseBox.gameObject.SetActive(false);

        foreach (GameObject button in tempResponseButtons)
        {
            Destroy(button);
        }
        tempResponseButtons.Clear();

        if (responseEvents != null && responseIndex <= responseEvents.Length)
        {
            responseEvents[responseIndex].OnPickedResponse?.Invoke();
        }

        responseEvents = null;

        if(response.DialogueObject)
        {
            dialogueUI.ShowDialogue(response.DialogueObject, false);
        }
        else
        {
            dialogueUI.CloseDialogueBox();
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
