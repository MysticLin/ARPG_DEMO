using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour
{
    [SerializeField] private int slotIndex;
    [SerializeField] private Button slotButton;
    [SerializeField] private TMP_Text slotTitle;
    [SerializeField] private TMP_Text slotInfo;
    private SaveSlotsCanvas owner;
    public void Bind(SaveSlotsCanvas owner,int slot)
    {
        this.owner = owner;
        slotIndex = slot;
        slotButton.onClick.RemoveAllListeners();
        slotButton.onClick.AddListener(() => owner.OnslotClicked(slotIndex));
    }
    public void Refresh()
    {
        if (SaveManager.Instance == null)
        {
            slotInfo.text = "Loading...";
            return;
        }
        SaveData data = SaveManager.Instance.ReadSlot(slotIndex);
        bool hasSave = data != null;
        slotTitle.text = $"Slot {slotIndex + 1}";
        slotInfo.text = hasSave ? $"{data.sceneName}\n{data.savedAt}" : "Empty\nNew Game";
    }
}
