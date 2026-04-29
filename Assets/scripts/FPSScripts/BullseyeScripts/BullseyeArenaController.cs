using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BullseyeArenaController : MonoBehaviour
{
    [SerializeField] Animator platAnim;
    public void rightAttack()
    {
        platAnim.Play("RightAttackCont");
    }
}
