using UnityEngine;
using UnityEngine.SceneManagement;

public class BootLoader : MonoBehaviour
{
    [SerializeField] private string firstScene = "RPG";
    private void Start()
    {
        SceneManager.LoadScene(firstScene);
    }
}
