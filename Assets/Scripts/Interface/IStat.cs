public interface IStat
{
    float GetHp();
    
    float GetDamage();
    
    void Damage(IStat enemy);
}