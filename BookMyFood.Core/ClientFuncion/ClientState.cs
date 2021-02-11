namespace BookMyFood.ClientFuncion
{
    public enum ClientState
    {
        Offline = 0,
        Connecting = 1,
        DelivererSet = 2,
        DelivererWait = 3,
        OrderSet = 4,
        WaitingForCalculation = 5,
        Calculated = 6
    }
}