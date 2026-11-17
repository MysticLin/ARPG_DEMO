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
            combat.enabled=!combat.enabled;
            bow.enabled=!bow.enabled;
        }
    }
}
