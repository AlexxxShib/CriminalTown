namespace Template
{
    public struct SignalGameStart {}
    
    public struct SignalGameOver {}
    
    public struct SignalUpgradeCharacter {}

    public struct SignalAddMoney
    {
        public long Money;
    }
    
    public struct SignalAddHardCurrency
    {
        public long HardCurrency;
    }
    
}