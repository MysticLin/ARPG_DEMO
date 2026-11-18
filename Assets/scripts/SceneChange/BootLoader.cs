using UnityEngine;
using UnityEngine.SceneManagement;

public class BootLoader : MonoBehaviour
{
    [SerializeField] private string firstScene = "RPG";
    private void Start()
    {
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadScene(firstScene, "Boot");
            return;
        }
        SceneManager.LoadScene(firstScene);
    }
}
