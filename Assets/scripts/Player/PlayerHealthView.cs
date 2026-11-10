using System.ComponentModel;
using TMPro;
using UnityEngine;

public class PlayerHealthView : MonoBehaviour
{
    [SerializeField] private TMP_Text HealthText;
    private Health health;
    private void OnEnable() => Bind();
    private void Start()=>Bind();
    //以后扩展使用切换角色，或者更换health实例使用。
    private void Bind()
    {
        if(StatsManager.Instance==null)return;
        Health newhealth = StatsManager.Instance.PlayerHealth;
        if (newhealth == null) return;
        if (health != null && newhealth != null)
            health.OnChangeHealth -= UpdateHealthUI;
        health = newhealth;
        health.OnChangeHealth -= UpdateHealthUI;
        health.OnChangeHealth += UpdateHealthUI;
        UpdateHealthUI(health.CurrentHealth, health.MaxHealth);

    }
    private void OnDisable()
    {
        if(health!=null)
            health.OnChangeHealth -= UpdateHealthUI;
    }
    private void UpdateHealthUI(int cur,int max)
    {
        if(health!=null&&HealthText!=null)
        HealthText.text = "HP:" + cur + "/" +max;
    }
}
