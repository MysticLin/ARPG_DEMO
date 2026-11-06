using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using TMPro;

public class StatsManager : MonoBehaviour
{
    //单例模式，数值管理器实例。
    public static StatsManager Instance;
    public StatsUI statsUI;
    public TMP_Text healthText;

    [Header("武器属性")]
    public int damage;
    public float knockbackForce;
    public float weaponRange;
    public float knockbackTime;
    public float stunTime;
    [Header("移动属性")]
    public float speed;

    [Header("生命属性")]
    public int maxHealth;
    public int currentHealth;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    public void UpdateMaxHealth(int amount)
    {
        maxHealth += amount;
        healthText.text = "HP:" + currentHealth + "/" + maxHealth;
    }
    public void UpdateHealth(int amount)
    {
        currentHealth += amount;
        if(currentHealth>=maxHealth)
            currentHealth = maxHealth;
        healthText.text = "HP:" + currentHealth + "/" + maxHealth;
    }
    public void UpdateSpeed(int amount)
    {
        speed += amount;
        statsUI.UpdateAllStats();
    }
}
