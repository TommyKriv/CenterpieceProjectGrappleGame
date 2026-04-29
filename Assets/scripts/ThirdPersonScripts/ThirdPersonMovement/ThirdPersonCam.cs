using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    public Transform orientation;
    [SerializeField] Transform playerTrans;
    [SerializeField] Transform playerObjTrans;
    [SerializeField] Rigidbody rb;

    [SerializeField] float rotSpeed;

    public bool targeting;
    private gameMaster gm;

    private void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<gameMaster>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Vector3 viewDir = playerTrans.position - new Vector3(transform.position.x, playerTrans.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (inputDir != Vector3.zero && !targeting)
            playerObjTrans.forward = Vector3.Slerp(playerObjTrans.forward, inputDir.normalized, Time.deltaTime * rotSpeed);
    }
}
