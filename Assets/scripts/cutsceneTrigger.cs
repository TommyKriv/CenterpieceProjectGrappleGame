using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using Cinemachine;

public class cutsceneTrigger : MonoBehaviour
{
    [SerializeField] GameObject cutsceneToPlay;

    private void OnTriggerEnter(Collider other)
    {
        cutsceneToPlay.SetActive(true);
        //other.gameObject.SetActive(false);
    }
}
