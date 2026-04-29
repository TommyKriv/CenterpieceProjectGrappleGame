using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaScript : MonoBehaviour
{
    [SerializeField] GrappleGun1 Ggun;
    [SerializeField] PlayerMovement pM;
    [SerializeField] Animator teslaControl;
    [SerializeField] BullseyeFight1 BSControl;
    [SerializeField] GameObject XTesla;

    private bool beginQTE;

    public void Grappled() //Works because is tagged as interactable
    {
        //Ggun.GrappleLocked = true;

        //teslaControl.speed = 0;
        //XTesla.GetComponent<Animator>().speed = 0;
        //teslaControl.Play("PullIn");
        //XTesla.SetActive(true);
        //StartCoroutine(qte());
        Ggun.interactGrappled = true;
        beginQTE = true;
    }

    private void Update()
    {
        if(beginQTE == true)
        {
            if (Input.GetMouseButtonDown(1))
            {
                teslaControl.Play("PullIn");

                StartCoroutine(BSControl.finalLaser());
                this.gameObject.tag = "Untagged";
                gameObject.transform.GetChild(0).gameObject.SetActive(true);
                beginQTE = false;
                /*XTesla.GetComponent<Animator>().speed = 0.05f;
                teslaControl.speed += 0.05f;*/
            }

            /*if(teslaControl.GetCurrentAnimatorStateInfo(0).IsName("End"))
            {
                Ggun.GrappleLocked = false;

                StopCoroutine(qte());
            }*/
        }
    }

    /*private IEnumerator qte()
    {
        yield return new WaitForSeconds(16f);
        if(teslaControl.GetCurrentAnimatorStateInfo(0).IsName("PullIn"))
        {
            Ggun.GrappleLocked = false;
            StartCoroutine(BSControl.breakout());
        }

        yield return null;
    }*/
}
