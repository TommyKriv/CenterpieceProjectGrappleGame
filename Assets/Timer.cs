using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    private void OnEnable()
    {
        float time = GameObject.FindGameObjectWithTag("GM").GetComponent<gameMaster>().timer;
        time = ((int)time);
        time /= 60;
        time = Mathf.Round(time * 100f) / 100f;

        GetComponent<TMP_Text>().text = "" + time;
    }
}
