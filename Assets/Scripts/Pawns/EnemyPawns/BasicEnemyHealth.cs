using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyHealth : MonoBehaviour, IHealth
{
    public int BaseHealth;
    private int ExtraHealth;
    private int TotalHealth;
    private int CurrentHealth;

    public void Start()
    {
        Attributes SetExtras = GetComponentInParent<Attributes>();
        ExtraHealth = (SetExtras.Grit + (SetExtras.Chutzpah / 2)) * 5;
        TotalHealth = ExtraHealth + BaseHealth;
        CurrentHealth = TotalHealth;
    }

    public void Heal(int healing)
    {
        throw new System.NotImplementedException();
    }

    public void HealPercent(double healing)
    {
        throw new System.NotImplementedException();
    }

    public int ReturnHealth()
    {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        throw new System.NotImplementedException();
    }
}
