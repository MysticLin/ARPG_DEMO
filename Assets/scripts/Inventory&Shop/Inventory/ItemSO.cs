using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="New Item")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    [TextArea]public string itemDesciption;
    public Sprite icon;

    public bool isGold;
    public bool isExp;
    public int stackSize = 3;

    [Header("状态")]
    public int currentHealth;
    public int maxHealth;
    public int speed;
    public int damage;

    [Header("临时效果物品")]
    public float duration;
}
