using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CutsceneInteracter : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject CutsceneToPlay;
    private GameObject intUI;

    private void Start()
    {
        intUI = FindFirstObjectByType<DialogueUI>().transform.GetChild(1).gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.transform.parent.TryGetComponent(out ThirdPersonMovement player))
        {
            player.Interactable = this;
            intUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.transform.parent.TryGetComponent(out ThirdPersonMovement player))
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
        CutsceneToPlay.SetActive(true);
        intUI.SetActive(false);
    }

    public void Interact(ThirdPersonMovement player)
    {
        CutsceneToPlay.SetActive(true);
        intUI.SetActive(false);
    }
}
