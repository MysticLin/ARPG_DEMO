using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BootMenu : MonoBehaviour
{
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private string firstScene = "RPG";
    private void Start()
    {
        newGameButton.onClick.AddListener(NewGame);
        continueButton.onClick.AddListener(ContinueGame);
        quitButton.onClick.AddListener(Quit);
        continueButton.interactable = SaveManager.Instance != null && SaveManager.Instance.HasSave;
    }
    public void NewGame()
    {
        SaveManager.Instance?.DeleteSave();
        Load(firstScene, "Boot");
    }
    public void ContinueGame()
    {
        if (SaveManager.Instance == null || !SaveManager.Instance.HasSave)
            return;
        SaveData data=SaveManager.Instance.Data;
        SaveManager.Instance.RequestApplyAfterLoad();
        Load(data.sceneName, data.spwanId);
    }
   private void Load(string sceneName,string spawnId)
    {
        if (SceneLoader.Instance != null)
            SceneLoader.Instance.LoadScene(sceneName, spawnId);
        else
            SceneManager.LoadScene(sceneName);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
