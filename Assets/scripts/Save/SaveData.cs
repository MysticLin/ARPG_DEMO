using System;
using System.Collections.Generic;
[Serializable]
public class SaveData
{
    public string sceneName;
    public string spwanId;
    public int version;
    public float playerX;
    public float playerY;
    public string savedAt;
    public int currentHealth;
    public int maxHealth;
    public int gold;
    public List<ItemSaveEntry> items = new();
    public List<QuestSaveEntry> activeQuests=new();
    public List<string> completedQuestIds=new();
    public List<string> visitedLocationIds=new();
    public List<string> spokenActorIds=new();
    public bool hasSlotIndex;
}
[Serializable]
public class ItemSaveEntry
{
    public string itemId;
    public int quantity;
    public int slotIndex;
}
[Serializable]
public class QuestSaveEntry
{
    public string questId;
    public int[] objectiveProgress;
}
