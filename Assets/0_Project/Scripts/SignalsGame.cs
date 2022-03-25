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
    
}