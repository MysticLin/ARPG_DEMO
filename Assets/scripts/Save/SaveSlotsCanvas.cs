using UnityEngine;

public class SaveSlotsCanvas : MonoBehaviour
{
    [SerializeField] private SaveSlotUI[] slots;
    [SerializeField] private GameObject bootMenuCanvas;
    private bool bound;
    private void OnEnable()
    {
        if(SaveManager.Instance==null)
            return;
        BindSlots();
        RefreshAll();
    }
    public void Show()
    {
        gameObject.SetActive(true);
        if(bootMenuCanvas!=null)
            bootMenuCanvas.SetActive(false);
        BindSlots();
        RefreshAll();
    }
    public void Hide()
    {
        gameObject.SetActive(false);
        if (bootMenuCanvas != null)
            bootMenuCanvas.SetActive(true);
    }
    private void BindSlots()
    {
        if (bound || slots == null) return;
        bound= true;
        for(int i=0;i<slots.Length;i++)
            slots[i].Bind(this, i);
    }
    private void RefreshAll()
    {
        if (slots == null) return;
        for(int i = 0; i < slots.Length; i++) 
            slots[i].Refresh();
    }
    public void OnslotClicked(int slot)
    {
        if (SaveManager.Instance == null) return;
        if (SaveManager.Instance.HasSaveInSlot(slot))
        {
            SaveManager.Instance.LoadAndReload(slot);
            return;
        }
        SaveManager.Instance.SelectSlot(slot);
        SaveManager.Instance.DeleteSlot(slot);
        if (SceneLoader.Instance != null)
            SceneLoader.Instance.LoadScene("RPG", "Boot");
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene("RPG");
    }
}
