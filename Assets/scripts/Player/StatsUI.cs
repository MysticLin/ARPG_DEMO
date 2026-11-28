using TMPro;
using UnityEngine;

public class StatsUI : MonoBehaviour
{
    public GameObject[] statsSlots;
    public UIWindow window;
    // 0 Damage / 1 Speed / 2 MaxHealth / 3 Defense / 4 DamageReduction / 5 Dodge
    private const int VisibleSlotCount = 6;
    private void Awake()
    {
        if(window==null)
            window = GetComponentInParent<UIWindow>();
        if (window != null)
            window.OnShown += UpdateAllStats;
    }
    private void Start()
    {
        UpdateAllStats();
    }
    private void OnDestroy()
    {
        if(window!=null)
            window.OnShown -= UpdateAllStats;
    }
    private void Update()
    {
        if (DialogueManager.IsDialogueActive) return;

        if (Input.GetButtonDown("ToggleStats"))
        {
            if (window != null)
                UIWindowManager.Instance?.Toggle(window);
        }
    }

    public void UpdateDamage()
    {
        SetSlotText(0, "Damage: " + StatsManager.Instance.damage);
    }

    public void UpdateSpeed()
    {
        SetSlotText(1, "Speed: " + StatsManager.Instance.speed.ToString("0.##"));
    }

    public void UpdateMaxHealth()
    {
        SetSlotText(2, "Max Health: " + StatsManager.Instance.MaxHealth);
    }

    public void UpdateDefense()
    {
        SetSlotText(3, "Defense: " + StatsManager.Instance.defense);
    }

    public void UpdateDamageReduction()
    {
        int percent = Mathf.RoundToInt(
            Mathf.Clamp01(StatsManager.Instance.damageReduction) * 100f);

        SetSlotText(4, "Damage Reduction: " + percent + "%");
    }

    public void UpdateDodge()
    {
        int percent = Mathf.RoundToInt(
            Mathf.Clamp01(StatsManager.Instance.dodgeChance) * 100f);

        SetSlotText(5, "Dodge: " + percent + "%");
    }

    public void UpdateAllStats()
    {
        if (StatsManager.Instance == null)
            return;

        UpdateDamage();
        UpdateSpeed();
        UpdateMaxHealth();
        UpdateDefense();
        UpdateDamageReduction();
        UpdateDodge();
        SetVisibleSlots();
    }

    private void SetVisibleSlots()
    {
        if (statsSlots == null) return;

        for (int i = 0; i < statsSlots.Length; i++)
        {
            if (statsSlots[i] != null)
                statsSlots[i].SetActive(i < VisibleSlotCount);
        }
    }

    private void SetSlotText(int index, string value)
    {
        if (statsSlots == null ||
            index < 0 ||
            index >= statsSlots.Length ||
            statsSlots[index] == null)
            return;

        TMP_Text text = statsSlots[index].GetComponentInChildren<TMP_Text>(true);
        if (text != null)
            text.text = value;
    }
}
