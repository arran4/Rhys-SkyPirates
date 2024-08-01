using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Interface class to allow iteration on diffrent highlight needs.
public interface IHealth
{
    public void TakeDamage(int damage);
    public void Heal(int healing);
    public void HealPercent(double healing);
    public int ReturnHealth();
}
