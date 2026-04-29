using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightTarget : MonoBehaviour
{
    [SerializeField] GameObject WeakPointBackR, headLaser;
    [SerializeField] Animator bullseyeAnim, rightWPController;
    [SerializeField] float laserDelay, laserDeacDelay;
    [SerializeField] PlayerMovement Pm;
    [SerializeField] GrappleGun1 g1;
    [SerializeField] dashScript ds;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Player"  && bullseyeAnim.GetBool("RightLocked") == false)
        {
            GetComponent<MeshCollider>().enabled = false;
            Pm.doubleJump = true;
            bullseyeAnim.SetBool("RightLocked", true);
            bullseyeAnim.Play("RightShoulderLock");
            rightWPController.enabled = true;
            StartCoroutine(laserVFX());
            StartCoroutine(animFinish());
            if (gameObject.transform.childCount > 0)
            {
                g1.StopGrapple();
            }
        }
    }

    IEnumerator laserVFX()
    {
        yield return new WaitForSeconds(laserDelay);
        headLaser.SetActive(true);
        yield return new WaitForSeconds(laserDeacDelay);
        headLaser.SetActive(false);
    }

    IEnumerator animFinish()
    {
        yield return new WaitForSeconds(8.0f);
        WeakPointBackR.SetActive(true);
        Destroy(this.gameObject.transform.parent.gameObject);
        Destroy(this.gameObject);
    }
}
