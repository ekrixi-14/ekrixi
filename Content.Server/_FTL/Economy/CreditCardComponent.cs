namespace Content.Server._FTL.Economy;

/// <summary>
/// This is used for tracking credits
/// </summary>
[RegisterComponent, Access(typeof(EconomySystem))]
public sealed class CreditCardComponent : Component
{
    [DataField("balance"), ViewVariables(VVAccess.ReadWrite)]
    public int Balance;
}
