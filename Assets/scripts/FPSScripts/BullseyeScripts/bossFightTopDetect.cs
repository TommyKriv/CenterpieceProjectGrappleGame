using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossFightTopDetect : MonoBehaviour
{
    [SerializeField] BullseyeFight1 BullseyeFightScript;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            BullseyeFightScript.topDetect = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            BullseyeFightScript.topDetect = false;
        }
    }
}
