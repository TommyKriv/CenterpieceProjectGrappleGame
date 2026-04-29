using UnityEngine;

public class gameMaster : MonoBehaviour
{
    public static gameMaster instance;
    public Vector3 lastCheckPoint;
    public bool checkPoints;
    public float yRotation;
    public bool bossFightStarted = false;

    public float fov, masterVolume;

    public float timer = 0f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
    }
}
