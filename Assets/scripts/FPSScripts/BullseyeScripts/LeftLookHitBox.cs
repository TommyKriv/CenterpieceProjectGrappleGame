using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftLookHitBox : MonoBehaviour
{
    [SerializeField] Animator bullseyeAnim;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !bullseyeAnim.GetBool("LeftLocked"))
        {
            bullseyeAnim.Play("LookLeft");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && bullseyeAnim.GetCurrentAnimatorStateInfo(0).IsName("LookLeft"))
        {
            bullseyeAnim.Play("LookLeftBack");
        }
    }
}
