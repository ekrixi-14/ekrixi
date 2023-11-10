using Robust.Server.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Random;

namespace Content.Server._FTL.FTLPoints.Tick.Factory;

/// <summary>
/// This system spawns ships every tick.
/// </summary>
public sealed class FactoryTickSystem : StarmapTickSystem<FactoryTickComponent>
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly MapLoaderSystem _mapLoader = default!;

    protected override void Ticked(EntityUid uid, FactoryTickComponent component, float frameTime)
    {
        base.Ticked(uid, component, frameTime);

        var transform = Transform(uid);

        if (_mapLoader.TryLoad(transform.MapID, _random.Pick(component.MapPaths).ToString(), out _))
        {
            Log.Debug("Created a new ship!");
        }
    }
}
