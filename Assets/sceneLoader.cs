using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneLoader : MonoBehaviour
{
    [SerializeField] string sceneName;
    [SerializeField] Vector3 checkPointLoc;
    [SerializeField] float yRotation;

    private gameMaster gM;
    private void Awake()
    {
        gM = gameMaster.instance;
    }
    public void sceneLoad()
    {
        gM.lastCheckPoint = checkPointLoc;
        gM.yRotation = yRotation;
        gM.checkPoints = false;
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
