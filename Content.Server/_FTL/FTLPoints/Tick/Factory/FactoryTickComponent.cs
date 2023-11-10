using Robust.Shared.Utility;

namespace Content.Server._FTL.FTLPoints.Tick.Factory;

/// <summary>
/// This is used for tracking the ResPaths to maps that we would like to spawn.
/// </summary>
[RegisterComponent]
public sealed partial class FactoryTickComponent : Component
{
    [DataField("mapPaths", required: true)]
    public List<ResPath> MapPaths { set; get; } = new();
}
