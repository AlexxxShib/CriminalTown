namespace Mobiray.Entities
{
    
    public interface IVictim
    {
        void BulletAttack(float attack, int soldierId);

        bool IsDeath();
    }
}