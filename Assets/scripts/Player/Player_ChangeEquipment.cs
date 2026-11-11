using UnityEngine;

public class Player_ChangeEquipment : MonoBehaviour
{
    public Player_Combat combat;
    public Player_Bow bow;
    void Update()
    {
        if (StatsManager.Instance == null || StatsManager.Instance.isDead) return;
        if (Input.GetButtonDown("ChangeEquipment"))
        {
            combat.enabled=!combat.enabled;
            bow.enabled=!bow.enabled;
        }
    }
}
