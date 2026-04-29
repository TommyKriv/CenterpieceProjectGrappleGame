using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class pauseMenu : MonoBehaviour
{
    [SerializeField] Slider fovSlider, masterVolumeSlider;
    [SerializeField] TMP_Text fovText, mVolText;
    [SerializeField] GameObject mainMenu, settingsMenu;
    private GameObject theMenu;
    private PlayerMovement pM;
    private ThirdPersonMovement tpM;
    private PlayerLook pL;
    private GrappleGun1 g1;
    FMOD.Studio.Bus masterBus;

    private void Awake()
    {
        if(FindAnyObjectByType<PlayerMovement>())
        {
            Debug.Log("Fps");
            pM = FindAnyObjectByType<PlayerMovement>();
            pL = FindAnyObjectByType<PlayerLook>();
            if(FindAnyObjectByType<GrappleGun1>())
            {
                Debug.Log("hookFound");
                g1 = FindAnyObjectByType<GrappleGun1>();
            }
        }
        else
        {
            Debug.Log("TPS");
            pM = null;
            tpM = FindAnyObjectByType<ThirdPersonMovement>();
        }

        masterBus = FMODUnity.RuntimeManager.GetBus("Bus:/");
        fovSlider.value = gameMaster.instance.fov;
        fovText.SetText(fovSlider.value.ToString());
        masterVolumeSlider.value = gameMaster.instance.masterVolume;
        ChangeMasterVolumeSliderValue();

        theMenu = transform.GetChild(0).gameObject; 
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!theMenu.activeInHierarchy)
            {
                theMenu.SetActive(true);
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                if(pM != null)
                {
                    pL.enabled = false;
                    if (g1 != null)
                    {
                        g1.enabled = false;
                    }
                }
                else
                {
                    tpM.pCam.enabled = false;
                }
            }
            else
            {
                unPause();
            }
        }
    }

    public void unPause()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
        theMenu.SetActive(false);
        Time.timeScale = 1.0f;
        if (pM != null)
        {
            pL.enabled = true;
            if (g1 != null)
            {
                g1.enabled = true;
            }
        }
        else
        {
            tpM.pCam.enabled = true;
        }
    }

    public void ChangeFovSliderValue()
    {
        float speedValue = fovSlider.value;
        Debug.Log(speedValue);
        if(pM != null)
        {
            pM.fov = speedValue;
            pM.Vcam.m_Lens.FieldOfView = pM.fov;
        }
        else
        {
            tpM.fov = speedValue;
            tpM.Vcam.m_Lens.FieldOfView = tpM.fov;
        }
        fovText.SetText(speedValue.ToString());
        gameMaster.instance.fov = (speedValue);
    }

    public void ChangeMasterVolumeSliderValue()
    {
        float mVolumeValue = masterVolumeSlider.value;
        mVolText.SetText(mVolumeValue.ToString());
        gameMaster.instance.masterVolume = mVolumeValue;
        mVolumeValue /= 100;
        masterBus.setVolume(mVolumeValue);
    }
}
