namespace CriminalTown
{
    public struct SignalGameStart {}
    
    public struct SignalGameOver {}
    
    public struct SignalUpgradeCharacter {}

    public struct SignalAddMoney
    {
        public long money;
    }
    
    public struct SignalAddHardCurrency
    {
        public long hardCurrency;
    }
    
}