using UnityEngine;

public class Player_ChangeEquipment : MonoBehaviour
{
    public Player_Combat combat;
    public Player_Bow bow;
    void Update()
    {
        if (PauseService.Instance != null && PauseService.Instance.IsPaused) return;
        if (DialogueManager.IsDialogueActive) return;
        if (StatsManager.Instance == null || StatsManager.Instance.isDead) return;
        if (Input.GetButtonDown("ChangeEquipment"))
        {
            if(combat==null||bow==null)return;
            if (combat.enabled)
            {
                combat.enabled = false;
                bow.enabled = true;
            }
            else if (bow.enabled && combat.unlocked)
            {
                bow.enabled = false;
                combat.enabled=true;
            }
        }
    }
}
