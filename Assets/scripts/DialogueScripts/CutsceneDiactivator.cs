using UnityEngine;

public class CutsceneDiactivator : MonoBehaviour
{
    [SerializeField] private DialogueObject dialogueObject;
    /*[SerializeField] public Player player;
    public void UpdateDialogueObject(DialogueObject dialogueObject)
    {
        this.dialogueObject = dialogueObject;
    }
    public void OnEnable()
    {
        player.DialogueUI.ShowDialogue(dialogueObject);
    }
    public void OnDisable()
    {
        player.DialogueUI.CloseDialogueBox();
    }
    public void Activate(Player player)
    {
        foreach (DialogueResponseEvents responseEvents in GetComponents<DialogueResponseEvents>())
        {
            if (responseEvents.DialogueObject == dialogueObject)
            {
                player.DialogueUI.AddResponseEvents(responseEvents.Events);
                break;
            }
        }

        player.DialogueUI.ShowDialogue(dialogueObject);
    }*/
}
