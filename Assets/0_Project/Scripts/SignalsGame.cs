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

    public struct SignalPoliceActivated { }
    
    public struct SignalPoliceDeactivated { }
    
}