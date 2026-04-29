using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BullseyeFight1 : MonoBehaviour
{
    [SerializeField] Animator bullseyeAnim, platAnim, spearGunAnim;
    [SerializeField] float attackCD;
    [SerializeField] arenaParenting platformParent;
    [SerializeField] ArenaTarget topTarget;
    [SerializeField] BullseyeHitBox leftHitBox, rightHitBox;
    [SerializeField] GameObject hookShot, hookRift, HSExit, platRift, riftedPlat, teslaCoil, teslaRift, platRift2, riftedPlat2, ESERift, ESEExit, ESEntrance, ESRRift, ESRead, ESRExit, bigLaser, headGrapple, platRift3, riftedPlat3, platRift4, riftedPlat4, allOutObject, leftLook, rightLook, goUpArrow, allOutDia;

    private float attackCDTimer;
    public bool leftDetect, rightDetect, botDetect, topDetect;
    private bool introCleared, slammedDown, allOutted;
    private string lastAttack;

    private void OnEnable()
    {
        StartCoroutine(fightBegin());
        //StartCoroutine(finalLaser());
        //StartCoroutine(finalSectionBegin());
    }

    private void Update()
    {
        if (attackCDTimer > 0)
            attackCDTimer -= Time.deltaTime;

        if (canAttack() && bullseyeAnim.GetCurrentAnimatorStateInfo(0).IsName("RestingState") && introCleared)
        {
            determineAttack();
        }
    }

    void determineAttack()
    {
        if (bullseyeAnim.GetBool("LeftLocked") && bullseyeAnim.GetBool("RightLocked"))
        {
            StartCoroutine(finalSectionBegin());
            attackCDTimer = 100;
        }
        if (bullseyeAnim.GetBool("LeftLocked") && !allOutted || bullseyeAnim.GetBool("RightLocked") && !allOutted)
        {
            StartCoroutine(allOutAttack());
        }
        else if (leftDetect && !rightDetect && lastAttack != "leftAttack")
        {
            leftAttack();
        }
        else if (!leftDetect && rightDetect && lastAttack != "rightAttack") //debate on adding a connection to the respective colliders, letting them know what attack is currently going on, and make a respective add force to the correct direction in enterTrigger.
        {
            rightAttack();
        }
        else if (leftDetect && rightDetect && lastAttack != "sweep")
        {
            sweep();
        }
        else if (topDetect && lastAttack != "slamDown")
        {
            slamDown();
        }
        else if (botDetect && slammedDown)
        {
            bullseyeAnim.SetTrigger("BotCollide");
            StartCoroutine(platformParent.upAttackDelay());
            platAnim.SetTrigger("slamUp");
            attackCDTimer = attackCD;
            leftHitBox.slamming = true;
            rightHitBox.slamming = true;
            slammedDown = false;
            StartCoroutine(resetTriggers());
        }
        else if (botDetect || topDetect || rightDetect || leftDetect)
        {
            randomNormalAttack();
        }
    }

    bool canAttack()
    {
        if (attackCDTimer > 0)
        {
            return false;
        }
        else
        {
            bullseyeAnim.SetTrigger("canAttack");
            return true;
        }
    }

    private void leftAttack()
    {
        lastAttack = "leftAttack";
        bullseyeAnim.SetTrigger("LeftCollide");
        attackCDTimer = attackCD;
        leftHitBox.rightSwing = true;
        StartCoroutine(topTarget.activate());
        StartCoroutine(resetTriggers());
    }

    private void rightAttack()
    {
        lastAttack = "rightAttack";
        bullseyeAnim.SetTrigger("RightCollide");
        StartCoroutine(platformParent.rightAttackDelay());
        platAnim.SetTrigger("rightAttack");
        attackCDTimer = attackCD;
        StartCoroutine(resetTriggers());
    }

    private void sweep()
    {
        lastAttack = "sweep";
        bullseyeAnim.SetTrigger("LeftCollide");
        bullseyeAnim.SetTrigger("RightCollide");
        attackCDTimer = attackCD;
        leftHitBox.leftSwing = true;
        StartCoroutine(topTarget.activate());
        StartCoroutine(resetTriggers());
    }

    private void slamDown()
    {
        lastAttack = "slamDown";
        bullseyeAnim.SetTrigger("TopCollide");
        attackCDTimer = attackCD;
        leftHitBox.slamming = true;
        rightHitBox.slamming = true;
        slammedDown = true;
        StartCoroutine(resetTriggers());
    }

    private void randomNormalAttack()
    {
        var attack = Random.Range(0, 4);

        if (attack == 0)
        {
            leftAttack();
        }
        if (attack == 1)
        {
            rightAttack();
        }
        if (attack == 2)
        {
            sweep();
        }
        if (attack == 3 && attack == 4)
        {
            slamDown();
        }
    }

    IEnumerator fightBegin()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        StartCoroutine(topTarget.deactivate());
        yield return new WaitForSeconds(1f);
        bullseyeAnim.Play("FightIntro");
        introCleared = true;
        yield return null;
    }

    IEnumerator resetTriggers()
    {
        yield return new WaitForSeconds(2f);
        platAnim.ResetTrigger("rightAttack");
        bullseyeAnim.ResetTrigger("LeftCollide");
        bullseyeAnim.ResetTrigger("RightCollide");
        bullseyeAnim.ResetTrigger("BotCollide");
        yield return new WaitForSeconds(attackCD);
        StartCoroutine(topTarget.deactivate());
        leftHitBox.resetBool();
        rightHitBox.resetBool();
        yield return null;
    }

    IEnumerator finalSectionBegin()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(hookShotEntrance());
        yield return new WaitForSeconds(1f);
        bullseyeAnim.Play("LaserWindup");
    }

    IEnumerator hookShotEntrance()
    {
        hookRift.SetActive(true);
        yield return new WaitForSeconds(1f);
        hookShot.SetActive(true);

        yield return new WaitForSeconds(4f);
        hookRift.SetActive(false);

        yield return new WaitForSeconds(2f);
        platRift.SetActive(true);
        platRift2.SetActive(true);
        teslaRift.SetActive(true);

        yield return new WaitForSeconds(2f);
        riftedPlat.SetActive(true);
        riftedPlat2.SetActive(true);
        teslaCoil.SetActive(true);

        yield return new WaitForSeconds(2f);
        platRift3.SetActive(true);
        platRift4.SetActive(true);

        yield return null;
    }

    public IEnumerator finalLaser()
    {
        riftedPlat3.SetActive(true);
        riftedPlat4.SetActive(true);

        bullseyeAnim.Play("BackShock");

        HSExit.SetActive(true);
        hookShot.GetComponent<Animator>().Play("HookshotExit");
        spearGunAnim.Play("Exit");

        platRift.SetActive(false);
        platRift2.SetActive(false);
        riftedPlat.GetComponent<Rigidbody>().useGravity = true;
        riftedPlat2.GetComponent<Rigidbody>().useGravity = true;

        yield return new WaitForSeconds(5f);
        ESERift.SetActive(true);
        ESEExit.SetActive(true);

        yield return new WaitForSeconds(4f);
        platRift3.SetActive(false);
        platRift4.SetActive(false);
        bullseyeAnim.Play("FinalLaser");
        ESEntrance.SetActive(true);
        platAnim.Play("ESEntrance");

        yield return new WaitForSeconds(0.8f);
        Destroy(HSExit);
        hookShot.SetActive(false);
        Destroy(ESERift);

        yield return new WaitForSeconds(1.6f);
        riftedPlat3.GetComponent<Rigidbody>().useGravity = true;
        riftedPlat4.GetComponent<Rigidbody>().useGravity = true;
        ESEntrance.SetActive(false);
        Destroy(ESEExit);

        yield return new WaitForSeconds(0.5f);
        bigLaser.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        ESRRift.SetActive(true);
        ESRExit.SetActive(true);

        yield return new WaitForSeconds(3f);
        ESRead.SetActive(true);
        platAnim.Play("ESRead");

        yield return new WaitForSeconds(2f);
        Destroy(ESRRift);
        Destroy(ESRExit);
        ESRead.SetActive(false);
        bigLaser.SetActive(false);

        yield return new WaitForSeconds(2f);
        headGrapple.SetActive(true);

        yield return null;
    }

    public IEnumerator breakout()
    {


        yield return null;
    }

    public IEnumerator allOutAttack()
    {
        allOutted = true;
        leftLook.SetActive(false);
        rightLook.SetActive(false);
        StartCoroutine(topTarget.activate());
        bullseyeAnim.Play("Slam_Down");
        platAnim.Play("SlamDown");
        //leftHitBox.slamming = true;
        //rightHitBox.slamming = true;
        slammedDown = true;
        yield return new WaitForSeconds(4f);
        goUpArrow.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        goUpArrow.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        goUpArrow.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        goUpArrow.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        goUpArrow.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        goUpArrow.SetActive(false);
        allOutObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        allOutDia.SetActive(true);
        yield return new WaitForSeconds(70f);
        bullseyeAnim.Play("Slam_Up");
        platAnim.Play("SlamUp");

        StartCoroutine(resetTriggers());
        leftLook.SetActive(true);
        rightLook.SetActive(true);
        yield return null;
    }
}
