using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BootMenu : MonoBehaviour
{
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private SaveSlotsCanvas saveSlotsCanvas;
    [SerializeField] private string firstScene = "RPG";
    [SerializeField] private ConfirmDialog confirmDialog;
    private void Start()
    {
        newGameButton.onClick.AddListener(NewGame);
        continueButton.onClick.AddListener(ContinueGame);
        quitButton.onClick.AddListener(Quit);
    }
    public void NewGame()
    {
        if (SaveManager.Instance == null) return;
        if (SaveManager.Instance.HasSaveInSlot(0))
        {
            if (confirmDialog != null)
            {
                confirmDialog.Show("Start a new game? Slot 1 will be overwritten.",NewGameConfirmed);
                return;
            }
        }
        NewGameConfirmed();
    }
    private void NewGameConfirmed()
    {
        SaveManager.Instance.SelectSlot(0);
        SaveManager.Instance.DeleteSlot(0);
        Load(firstScene, "Boot");
    }
    public void ContinueGame()
    {
        if(saveSlotsCanvas!=null)
            saveSlotsCanvas.Show();
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
