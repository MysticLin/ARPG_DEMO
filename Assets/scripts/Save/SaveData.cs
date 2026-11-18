using System;
using System.Collections.Generic;
[Serializable]
public class SaveData
{
    public string sceneName;
    public string spwanId;

    public float playerX;
    public float playerY;

    public int currentHealth;
    public int maxHealth;
    public int gold;
    public List<ItemSaveEntry> items = new();
    public List<QuestSaveEntry> activeQuests=new();
    public List<string> completedQuestIds=new();
    public List<string> visitedLocationIds=new();
    public List<string> spokenActorIds=new();
}
[Serializable]
public class ItemSaveEntry
{
    public string itemId;
    public int quantity;
}
[Serializable]
public class QuestSaveEntry
{
    public string questId;
    public int[] objectiveProgress;
}
