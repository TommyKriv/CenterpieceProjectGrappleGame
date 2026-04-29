using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleYCounterScript : MonoBehaviour
{
    [SerializeField] Animator yAnimator;
    [SerializeField] Animator plateAnimator;
    void Start()
    {
        yAnimator.SetBool("Counter", true);
        
    }

    private void OnTriggerEnter(Collider other)
    {
        yAnimator.SetBool("GoodMorning!", true);
        plateAnimator.SetBool("Counter", true);
    }
}
