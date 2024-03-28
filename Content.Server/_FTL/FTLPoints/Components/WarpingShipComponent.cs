using Robust.Shared.Map;

namespace Content.Server._FTL.FTLPoints.Components;

/// <summary>
/// This is used for assigning a ship that is able to warp.
/// </summary>
[RegisterComponent]
public sealed partial class WarpingShipComponent : Component
{
    [ViewVariables] public MapId? TargetMap;
}
