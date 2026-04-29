using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossFightBottomDetector : MonoBehaviour
{
    [SerializeField] BullseyeFight1 BullseyeFightScript;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            BullseyeFightScript.botDetect = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            BullseyeFightScript.botDetect = false;
        }
    }
}
