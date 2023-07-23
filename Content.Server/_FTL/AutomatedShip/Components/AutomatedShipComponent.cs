namespace Content.Server._FTL.AutomatedShip.Components;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent]
public sealed class AutomatedShipComponent : Component
{
    /// <summary>
    /// States that the AI can go under
    /// </summary>
    public enum AiStates
    {
        Cruising,
        Fighting
    }

    /// <summary>
    /// How long does it take to fire a weapon?
    /// </summary>
    [DataField("attackRepetition"), ViewVariables(VVAccess.ReadWrite)]
    public float AttackRepetition = 15f;

    /// <summary>
    /// The next point the ship would like to warp to.
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly)] public EntityUid? NextWarpPoint;

    /// <summary>
    /// The current state of the AI.
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly)] public AiStates AiState = AiStates.Cruising;
}
