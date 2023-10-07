namespace Content.Server._FTL.AutomatedShip.Components;

/// <summary>
/// This is used for tracking things in active combat
/// </summary>
[RegisterComponent]
public sealed partial class ActiveAutomatedShipComponent : Component
{
    [ViewVariables] public float TimeSinceLastRetarget = 0f;
}
