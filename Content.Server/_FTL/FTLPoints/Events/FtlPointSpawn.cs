using JetBrains.Annotations;
using Robust.Shared.Map;

namespace Content.Server._FTL.FTLPoints.Events;

/// <summary>
/// The type of FTL point.
/// </summary>
///
[ImplicitDataDefinitionForInheritors, MeansImplicitUse]
public abstract partial class FtlPointSpawn
{
    public abstract void Effect(FtlPointSpawnArgs args);

    public readonly record struct FtlPointSpawnArgs(
        IEntityManager EntityManager,
        IMapManager MapManager
    );
}

