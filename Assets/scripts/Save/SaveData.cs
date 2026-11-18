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
}
[Serializable]
public class ItemSaveEntry
{
    public string itemId;
    public int quantity;
}
