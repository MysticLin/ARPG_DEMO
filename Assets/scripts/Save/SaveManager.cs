using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
  public static SaveManager Instance {  get; private set; }
    public SaveData Data {  get; private set; }
    public const int CurrentVersion = 2;
    public const int SlotCount = 3;
    public int CurrentSlot { get; private set; }
    private bool applyOnNextLoad;
    public bool HasSave => Data != null;
    private string SavePath(int slot) => Path.Combine(Application.persistentDataPath, $"save_{slot}.json");
    [SerializeField] private ItemDatabase itemDatabase;
    private void Awake()
    {
        if (itemDatabase == null)
            itemDatabase = Resources.Load<ItemDatabase>("ItemDatabase");
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        SelectSlot(0);
    }
    private void OnDestroy()
    {
        if(Instance==this)
            Instance = null;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
            Save();
        if(Input.GetKeyDown(KeyCode.F9))
            LoadAndReload();
    }
    public void Save()
    {
        Save(CurrentSlot);
    }
    public void Save(int slot)
    {
        GameObject player = GameObject.FindWithTag("Player");
        StatsManager stats = StatsManager.Instance;
        InventoryManager inventory= InventoryManager.Instance;
        if (SceneManager.GetActiveScene().name == "Boot")
            return;
        if (player == null || stats == null || inventory == null)
            return;
        Data = new SaveData
        {
            sceneName = SceneManager.GetActiveScene().name,
            spwanId = "",
            playerX = player.transform.position.x,
            playerY = player.transform.position.y,
            currentHealth = stats.CurrentHealth,
            maxHealth = stats.MaxHealth,
            gold = inventory.gold,
            version = CurrentVersion,
            hasSlotIndex = true
        };
        Data.savedAt = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        Data.items = new List<ItemSaveEntry>();
        for(int i = 0; i < inventory.SlotCount; i++)
        {
            InventorySlotView slotView=inventory.GetSlot(i);
            if(slotView.IsEmpty)continue;
            Data.items.Add(new ItemSaveEntry
            {
                itemId = slotView.itemSO.name,
                quantity = slotView.Quantity,
                slotIndex = i
            });
        }
        if (QuestManager.Instance != null)
        {
            Data.activeQuests = QuestManager.Instance.ExportActiveQuests();
            Data.completedQuestIds=QuestManager.Instance.ExportCompletedQuests();
        }
        if (GameManager.Instance != null)
        {
            if(GameManager.Instance.DialogueHistoryTracker != null) 
                Data.spokenActorIds=GameManager.Instance.DialogueHistoryTracker.ExportSokenActorIds();
            if(GameManager.Instance.LocatioHistroyTracker!=null)
                Data.visitedLocationIds=GameManager.Instance.LocatioHistroyTracker.ExportVisitedLocationIds();
        }
        Data.worldStates = WorldStateManager.Instance != null
    ? WorldStateManager.Instance.Export()
    : new List<SceneWorldState>();
        File.WriteAllText(SavePath(slot),JsonUtility.ToJson(Data,true));
        if (slot == CurrentSlot)
            Data = Data;
        Debug.Log($"Saved:{SavePath(slot)}");
    }
    public bool HasSaveInSlot(int slot)
    {
       SaveData data=ReadSlot(slot);
        return data != null;
    }
    public void SelectSlot(int slot)
    {
        CurrentSlot = Math.Clamp(slot, 0, SlotCount - 1);
        LoadFile(CurrentSlot);
    }
    public void LoadAndReload()
    {
        LoadAndReload(CurrentSlot);
    }
    public void LoadAndReload(int slot)
    {
        SelectSlot(slot);
        if (Data == null) return;
        RequestApplyAfterLoad();
        if (SceneLoader.Instance != null)
            SceneLoader.Instance.LoadScene(Data.sceneName, Data.spwanId);
        else
            SceneManager.LoadScene(Data.sceneName);
    }
    public void RequestApplyAfterLoad()
    {
        applyOnNextLoad=true;
    }
    public void ApplyAfterSceneLoad()
    {
        if (!applyOnNextLoad || Data == null) return;
        applyOnNextLoad=false;
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            player.transform.position = new Vector2(Data.playerX, Data.playerY);
        if (StatsManager.Instance != null)
            StatsManager.Instance.RestoreHealth(Data.currentHealth, Data.maxHealth);
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.ClearInventory();
            if (itemDatabase != null && Data.items != null)
            {
                foreach(ItemSaveEntry entry in Data.items)
                {
                    ItemSO item = itemDatabase.FindItem(entry.itemId);
                    if (item == null) continue;

                    if (Data.hasSlotIndex)
                    {
                        InventoryManager.Instance.SetSlot(
                            entry.slotIndex,
                            item,
                            entry.quantity);
                    }
                    else
                    {
                        InventoryManager.Instance.AddItem(
                            item,
                            entry.quantity);
                    }
                }
            }
            InventoryManager.Instance.SetGold(Data.gold);
        }
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.DialogueHistoryTracker != null)
                GameManager.Instance.DialogueHistoryTracker.ImportSpokenActorIds(Data.spokenActorIds, itemDatabase);
            if (GameManager.Instance.LocatioHistroyTracker != null)
            {
                GameManager.Instance.LocatioHistroyTracker.ImportVisitedLocationIds(Data.visitedLocationIds, itemDatabase);
            }
        }
        if (QuestManager.Instance != null)
            QuestManager.Instance.ImportSaveData(Data.activeQuests, Data.completedQuestIds, itemDatabase);
        QuestLogUI questLogUI=FindObjectOfType<QuestLogUI>(true);
        if(questLogUI!=null)
            questLogUI.RefreshQuestList();
        if (WorldStateManager.Instance != null)
        {
            WorldStateManager.Instance.Import(Data.worldStates);
            WorldStateManager.Instance.ApplyCurrentScene();
        }
    }
    private void OnApplicationQuit()
    {
        Save();
    }
    public SaveData ReadSlot(int slot)
    {
        if(slot<0||slot>=SlotCount) return null;
        string path = SavePath(slot);
        if(!File.Exists(path))return null;
        SaveData data=JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
        return data!=null&&data.version==CurrentVersion?data:null;
    }
    private void LoadFile(int slot)
    {
        if (!File.Exists(SavePath(slot)))
        {
            Data = null;
            return;
        }
        SaveData loaded = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath(slot)));
        if (loaded == null || loaded.version != CurrentVersion)
        {
            Data = null;
            return;
        }
        Data = loaded;
    }
    public void DeleteSave()
    {
        DeleteSlot(CurrentSlot);
    }
    public void DeleteSlot(int slot)
    {
        if(File.Exists(SavePath(slot)))
            File.Delete(SavePath(slot));
        if(slot==CurrentSlot)
        {
            Data = null;
            applyOnNextLoad=false;
        }
    }
}
