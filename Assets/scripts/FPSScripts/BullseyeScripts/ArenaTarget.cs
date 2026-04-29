using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaTarget : MonoBehaviour
{
    [SerializeField] PlayerMovement Pm;
    [SerializeField] GrappleGun1 Ggun;
    [SerializeField] Material inactiveMat, activeMat;
    [SerializeField] GrappleGun1 g1;

    int inactiveLayer, activeLayer;
    public bool destructable, active;

    void Start()
    {
        activeLayer = LayerMask.NameToLayer("Ground");
        inactiveLayer = LayerMask.NameToLayer("Default");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && active)
        {
            Pm.doubleJump = true;
        }
    }

    void Grappled()
    {
        if(active)
        {
            Ggun.enemyRb = this.GetComponent<Rigidbody>();
            Ggun.enemyGrappled = true;
        }
    }

    public IEnumerator activate()
    {
        active = true;
        float startRotation = gameObject.transform.eulerAngles.z;
        float endRotation = 90f;
        this.gameObject.GetComponent<MeshRenderer>().material = activeMat;
        this.gameObject.tag = "Enemy";
        this.gameObject.layer = activeLayer;
        float timeElapsed = 0;
        while (timeElapsed < 0.45f)
        {
            float zRotation = Mathf.Lerp(startRotation, endRotation, timeElapsed / 0.45f);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, zRotation);

            yield return null;
            timeElapsed += Time.deltaTime;
        }
    }

    public IEnumerator deactivate()
    {
        active = false;
        if(gameObject.transform.childCount > 0)
        {
            g1.StopGrapple();
        }

        float startRotation = gameObject.transform.eulerAngles.z;
        float endRotation = 0f;
        this.gameObject.GetComponent<MeshRenderer>().material = inactiveMat;
        this.gameObject.tag = "Untagged";
        this.gameObject.layer = inactiveLayer;
        float timeElapsed = 0;
        while (timeElapsed < 0.45f)
        {
            float zRotation = Mathf.Lerp(startRotation, endRotation, timeElapsed / 0.45f);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, zRotation);

            yield return null;
            timeElapsed += Time.deltaTime;
        }
        
    }

    public void cutsceneAdjust()
    {
        this.gameObject.transform.position = new Vector3(165, 125, 5182);
    }
}
