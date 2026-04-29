using UnityEngine.SceneManagement;
using UnityEngine;

public class EndLevelScript : MonoBehaviour
{
    [SerializeField] string nextScene;

    private void OnEnable()
    {
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        GameObject.FindFirstObjectByType<PlayerLook>().enabled = false;
    }

    public void NextScene()
    {
        Time.timeScale = 1f;
        Cursor.visible = false ;
        Cursor.lockState = CursorLockMode.Locked;
        gameMaster.instance.checkPoints = false;
        SceneManager.LoadScene(nextScene);
    }
}
