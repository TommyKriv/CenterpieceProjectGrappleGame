using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bouncerScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            enemyHitScript enemyScript = other.GetComponent<enemyHitScript>();
            other.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            other.GetComponent<Rigidbody>().velocity += new Vector3(0, enemyScript.upForce * 0.8f);
            other.GetComponent<Rigidbody>().AddForce((other.transform.position - gameObject.transform.position) * (enemyScript.backForce * 0.05f), ForceMode.Impulse);
        }
    }
    private void OnEnable()
    {
        StartCoroutine(waitToEnd());
    }

    IEnumerator waitToEnd()
    {
        yield return new WaitForSeconds(0.8f);
        gameObject.SetActive(false);
        yield return null;
    }
}
