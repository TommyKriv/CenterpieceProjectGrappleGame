using UnityEngine;


[System.Serializable]
public class Response
{
    [SerializeField] public bool unPauser;
    [SerializeField] private string responseText;
    [SerializeField] public DialogueObject dialogueObject;

    public string ResponseText => responseText;

    public DialogueObject DialogueObject => dialogueObject;
}
