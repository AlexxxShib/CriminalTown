using CriminalTown.Entities;

namespace CriminalTown
{
    public struct SignalGameStart {}
    
    public struct SignalGameOver {}
    
    public struct SignalUpgradeCharacter {}

    public struct SignalAddMoney
    {
        public int money;
    }
    
    public struct SignalAddHardCurrency
    {
        public long hardCurrency;
    }
    
    public struct SignalPlayerCaught { }

    public struct SignalPoliceStatus
    {
        public bool activated;

        public static SignalPoliceStatus ActiveState() => new() {activated = true};
        public static SignalPoliceStatus InactiveState() => new() {activated = false};
    }

    public struct SignalTryDestroyCitizen
    {
        public EntityCitizen citizen;

        public static SignalTryDestroyCitizen Destroy(EntityCitizen citizen) => new() {citizen = citizen};
    }
    
}