using JetBrains.Annotations;
using Robust.Shared.Map;

namespace Content.Server._FTL.FTLPoints.Effects;

/// <summary>
/// The type of FTL point.
/// </summary>
///
[ImplicitDataDefinitionForInheritors, MeansImplicitUse]
public abstract partial class FtlPointEffect
{
    [DataField("probability")] public float Probability = 1f;
    public abstract void Effect(FtlPointEffectArgs args);

    public readonly record struct FtlPointEffectArgs(
        EntityUid MapUid,
        MapId MapId,
        IEntityManager EntityManager,
        IMapManager MapManager
    );
}
