using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class StatsUI : MonoBehaviour
{
    public GameObject[] statsSlots;
    public CanvasGroup statsCanvas;
    private bool statsOpen=false;
    private void Start()
    {
       UpdateAllStats();
    }
    private void Update()
    {
        if (DialogueManager.IsDialogueActive) return;
        if (Input.GetButtonDown("ToggleStats"))
        {
            if (statsOpen)
            {
                PauseService.Instance?.Pop();
                UpdateAllStats();
                statsCanvas.alpha = 0;
                statsCanvas.blocksRaycasts = false;
                statsOpen = false;
            }
            else
            {
                PauseService.Instance?.Push();
                UpdateAllStats();
                statsCanvas.alpha = 1;
                statsCanvas.blocksRaycasts = true;
                statsOpen = true;
            }
           
        }
    }
    public void UpdateDamage()
    {
        statsSlots[0].GetComponentInChildren<TMP_Text>().text="Damage:"+StatsManager.Instance.damage;
    }
    public void UpdateSpeed()
    {
        statsSlots[1].GetComponentInChildren<TMP_Text>().text = "Speed:" + StatsManager.Instance.speed;
    }
    public void UpdateAllStats()
    {
        UpdateDamage();
        UpdateSpeed();
       
    }
    private void OnDisable()
    {
     if(statsOpen)
        {
            statsOpen = false;
            PauseService.Instance?.Pop();
            if (statsCanvas != null)
            {
                statsCanvas.alpha = 0;
                statsCanvas.blocksRaycasts = false;
            }
        }   
    }
}
