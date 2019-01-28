using UnityEngine;

public interface ISoldier
{
    void Death();
    
    void DoDamage(IStat enemy);

    void GoInAttack();
    
    Transform FindEnemy();
}

