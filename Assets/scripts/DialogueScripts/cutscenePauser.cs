using UnityEngine;
using UnityEngine.Playables;

public class cutscenePauser : MonoBehaviour
{
    public PlayableDirector currentDirector;
    public bool replied = false;
    public bool paused = false;

    private DialogueUI diaUI;

    private void Start()
    {
        diaUI = FindAnyObjectByType<DialogueUI>();
    }

    public void setDirector(string locationName)
    {
        currentDirector = GameObject.Find(locationName).GetComponent<PlayableDirector>();
    }

    public void pause()
    {
        if (!replied && diaUI.transform.GetChild(0).gameObject.activeInHierarchy)
        {
            currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(0);
            paused = true;
        }
        else
        {
            replied = false;
            return;
        }
    }
}
