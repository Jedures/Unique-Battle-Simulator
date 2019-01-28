using UnityEngine;

public class SoldierStat : IStat
{   
    private float m_Health;
    private readonly float m_Damage;

    public float GetHp() => m_Health;
    
    public float GetDamage() => m_Damage;

    public SoldierStat(float health, float damage)
    {
        m_Health = health;
        m_Damage = damage;
    }
    
    public void Damage(IStat enemy) => m_Health -= enemy.GetDamage();
}
