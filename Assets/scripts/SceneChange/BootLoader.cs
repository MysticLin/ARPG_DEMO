using UnityEngine;
using UnityEngine.SceneManagement;

public class BootLoader : MonoBehaviour
{
    [SerializeField] private string firstScene = "RPG";
    private void Start()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.HasSave)
        {
            SaveManager.Instance.RequestApplyAfterLoad();
            if (SceneLoader.Instance != null)
            {
                SceneLoader.Instance.LoadScene(SaveManager.Instance.Data.sceneName, SaveManager.Instance.Data.spwanId);
                return;
            }
        }
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadScene(firstScene, "Boot");
            return;
        }
        SceneManager.LoadScene(firstScene);
    }
}
