using System.Collections.Generic;
using System.IO;
using Unity.Content;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
  public static SaveManager Instance {  get; private set; }
    public SaveData Data {  get; private set; }
    public bool HasSave=>Data!=null;
    private bool applyOnNextLoad;
    private string SavePath => Path.Combine(Application.persistentDataPath, "save.json");
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
        LoadFile();
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
        GameObject player = GameObject.FindWithTag("Player");
        StatsManager stats = StatsManager.Instance;
        InventoryManager inventory= InventoryManager.Instance;
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
            gold=inventory.gold,
        };
        Data.items = new List<ItemSaveEntry>();
        for(int i = 0; i < inventory.SlotCount; i++)
        {
            InventorySlotView slot=inventory.GetSlot(i);
            if(slot.IsEmpty)continue;
            Data.items.Add(new ItemSaveEntry
            {
                itemId = slot.itemSO.name,
                quantity = slot.Quantity
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
        File.WriteAllText(SavePath,JsonUtility.ToJson(Data,true));
        Debug.Log($"Saved:{SavePath}");
    }
    public void LoadAndReload()
    {
        LoadFile();
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
                    if (item != null)
                        InventoryManager.Instance.AddItem(item, entry.quantity);
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
    }
    private void LoadFile()
    {
        if (!File.Exists(SavePath))
        {
            Data = null;
            return;
        }
        Data = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
    }

}
