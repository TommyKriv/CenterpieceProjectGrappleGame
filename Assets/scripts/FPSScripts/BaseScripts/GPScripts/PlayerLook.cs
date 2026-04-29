using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] WallRun wallRun;

    [SerializeField] private float sensX = 100f;
    [SerializeField] private float sensY = 100f;

    [SerializeField] Transform camHolder, Orientation;

    float mouseX;
    float mouseY;

    float multiplier = 0.01f;

    public float xRotation;
    public float yRotation;

    public bool targeting;
    private gameMaster gm;

    private void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<gameMaster>();
        if(gm.checkPoints)
        {
            yRotation = gm.yRotation;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        yRotation += mouseX * sensX * multiplier;
        xRotation -= mouseY * sensY * multiplier;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        
        if(!targeting)
        {
            camHolder.transform.rotation = Quaternion.Euler(xRotation, yRotation, wallRun.Tilt);
            Orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);
        }
    }
}
