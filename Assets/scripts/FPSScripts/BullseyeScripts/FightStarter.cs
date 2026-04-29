using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;

public class FightStarter : MonoBehaviour
{
    public List<GameObject> EnvironmentList;

    [SerializeField] GameObject Environment;
    [SerializeField] CinemachineVirtualCamera CutsceneCamm;
    [SerializeField] GameObject CamHolder;
    [SerializeField] CinemachineVirtualCamera PlayerCam;
    [SerializeField] GameObject PlaceholderBullseye;
    [SerializeField] GameObject cutsceneToPlay;

    [SerializeField] PlayerMovement pM;
    [SerializeField] PlayerLook pL;
    [SerializeField] GrappleGun1 g1;
    [SerializeField] GameObject chaseEyeMesh, camHolder;
    [SerializeField] PlayableDirector chaseScene;
    
    private gameMaster gm;

    private void Start()
    {
        foreach (Transform child in Environment.transform)
        {
            EnvironmentList.Add(child.gameObject);
        }
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<gameMaster>();
        if(gm.bossFightStarted)
        {
            chaseEyeMesh.SetActive(true);
            chaseScene.enabled = true;
            PlaceholderBullseye.SetActive(false);
            StartCoroutine(StartTheFight());
        }
    }

    public void Grappled()
    {
        
        //SUGGESTION!!! WAIT A FEW SECONDS BEFORE CHANGING CAMERAS SO WHEN THE ROOM BREAKS THE PLAYER THEMSELVES GET LIFTED UP BY BULLSEYE BEFORE THE ACTUAL CUTSCENE STARTS, COULD BE COOL!!!
        g1.StopGrapple();
        pM.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        pM.gameObject.transform.position = new Vector3(-7.5f, 767, 5307);
        this.gameObject.GetComponent<MeshRenderer>().enabled = false;

        pL.enabled = false;
        pM.enabled = false;
        g1.enabled = false;
        

        for (int i = 0; i < EnvironmentList.Count; i++)
        {
            EnvironmentList[i].GetComponent<Rigidbody>().isKinematic = false;

            var storedVect = Vector3.MoveTowards(EnvironmentList[i].transform.position, this.gameObject.transform.position, -600.0f);

            EnvironmentList[i].GetComponent<Rigidbody>().AddForce(storedVect, ForceMode.Impulse);
        }

        cutsceneToPlay.SetActive(true);


        PlaceholderBullseye.SetActive(false);

        return;
    }


    public void Deactivate()
    {
        pM.gameObject.transform.position = new Vector3(-7.5f, 767, 5307);
        pL.yRotation = 180;
        pL.xRotation = 34.15f;
        gm.lastCheckPoint = new Vector3(-7.5f, 767, 5307);
        gm.yRotation = 180;
        gm.bossFightStarted = true;

        for (int i = 0; i < EnvironmentList.Count; i++)
        {
            Destroy(EnvironmentList[i]);
        }
        EnvironmentList.Clear();

        cutsceneToPlay.SetActive(false);
        chaseEyeMesh.SetActive(true);
        chaseScene.enabled = true;
        this.gameObject.SetActive(false);
        pL.enabled = true;
        pM.enabled = true;
        g1.enabled = true;
        g1.isGrappled = false;
    }

    private IEnumerator StartTheFight()
    {
        for (int i = 0; i < EnvironmentList.Count; i++)
        {
            Destroy(EnvironmentList[i]);
        }
        EnvironmentList.Clear();

        yield return new WaitForSeconds(0.1f);
    }
}
