using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_StatusEffects : MonoBehaviour
{
    private Enemy_Health health;
    private Enemy_Move move;
    private float burnTimer;
    private float burnTickTimer;
    private int burnDamage;
    private float slowTimer;
    private float slowMultiplier = 1f;
    private void Awake()
    {
        health= GetComponent<Enemy_Health>();
        move= GetComponent<Enemy_Move>();
    }
    public void ApplyBurn(int damagePerTick,float duration)
    {
        burnDamage=damagePerTick;
        burnTimer = Mathf.Max(burnTimer, duration);
    }
    public void ApplySlow(float multiplier,float duration)
    {
        slowMultiplier = Mathf.Clamp(multiplier, 0.2f, 1f);
        slowTimer = Mathf.Max(slowTimer, duration);
        move.SetSpeedMultiplier(slowMultiplier);
    }
    private void Update()
    {
        if (burnTimer > 0f)
        {
            burnTimer -= Time.deltaTime;
            burnTickTimer-= Time.deltaTime;
            if (burnTickTimer <= 0f)
            {
                burnTickTimer = 1f;
                health.ChangeHealth(-burnDamage);
            }
        }
        if (slowTimer > 0f)
        {
            slowTimer -= Time.deltaTime;
            if (slowTimer <= 0f)
            {
                slowMultiplier = 1f;
                move.SetSpeedMultiplier(1f);
            }
        }
      
    }

}
