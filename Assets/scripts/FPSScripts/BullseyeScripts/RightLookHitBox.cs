using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightLookHitBox : MonoBehaviour
{
    [SerializeField] Animator bullseyeAnim;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            bullseyeAnim.Play("LookRight");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && bullseyeAnim.GetCurrentAnimatorStateInfo(0).IsName("LookRight"))
        {
            bullseyeAnim.Play("LookRightBack");
        }
    }
}
