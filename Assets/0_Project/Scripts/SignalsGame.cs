using CriminalTown.Entities;

namespace CriminalTown
{
    public struct SignalAddMoney
    {
        public int money;
    }

    public struct SignalIslandPurchased { }

    public struct SignalPlayerCaught { }

    public struct SignalPoliceStatus
    {
        public bool activated;
        public bool caught;

        public static SignalPoliceStatus ActiveState() => new() {activated = true, caught = false};
        public static SignalPoliceStatus InactiveState() => new() {activated = false, caught = false};
        public static SignalPoliceStatus CaughtState() => new() {activated = false, caught = true};
    }
    
    public struct SignalCitizenPanic
    {
        public bool activated;
        public EntityCitizen citizen;

        public static SignalCitizenPanic Activate(EntityCitizen citizen) => new() {activated = true, citizen = citizen};
        
        public static SignalCitizenPanic Deactivate(EntityCitizen citizen) => new() {activated = false, citizen = citizen};
    }

    public struct SignalTryDestroyCitizen
    {
        public EntityCitizen citizen;

        public static SignalTryDestroyCitizen Destroy(EntityCitizen citizen) => new() {citizen = citizen};
    }

    public struct SignalNewTool { }

    public struct SignalClarifyPursuitPosition { }

}