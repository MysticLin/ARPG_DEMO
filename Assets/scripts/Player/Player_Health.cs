using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player_Health : MonoBehaviour
{
    public TMP_Text healthText;
    public Animator HealthAnim;
    private void Start()
    {
        healthText.text = "HP:" + StatsManager.Instance.currentHealth + "/" + StatsManager.Instance.maxHealth;
    }

  
    public void ChangeHealth(int amount)
    {
        StatsManager.Instance.currentHealth += amount;
        HealthAnim.Play("HealthChange");
        healthText.text = "HP:" + StatsManager.Instance.currentHealth + "/" + StatsManager.Instance.maxHealth;
        if (StatsManager.Instance.currentHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
