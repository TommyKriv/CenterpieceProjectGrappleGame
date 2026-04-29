using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpIndicator : MonoBehaviour
{
    private PlayerMovement pM;
    public GameObject dJInd1;
    public GameObject dJInd2;
    // Update is called once per frame
    private void Start()
    {
        pM = gameObject.GetComponent<PlayerMovement>();
    }
    void Update()
    {
        if(pM.doubleJump == true)
        {
            dJInd1.SetActive(true);
            dJInd2.SetActive(false);
        }
        else
        {
            dJInd1.SetActive(false);
            dJInd2.SetActive(true);
        }
            
    }
}
