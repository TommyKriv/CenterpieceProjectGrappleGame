using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkPoint : MonoBehaviour
{
    [SerializeField] Vector3 checkPointLoc;
    [SerializeField] float yRotation;
    private gameMaster gm;

    private void Start()
    {
        gm = gameMaster.instance;
        checkPointLoc = this.transform.position;
        checkPointLoc.y += 36f;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            gm.checkPoints = true;
            gm.lastCheckPoint = checkPointLoc;
            gm.yRotation = yRotation;
        }
    }
}
