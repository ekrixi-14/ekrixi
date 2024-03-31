using Content.Server._FTL.ShipTracker.Components;

namespace Content.Server._FTL.ShipTracker.Events;

public sealed class ShipTrackerDestroyed : EntityEventArgs
{
    public EntityUid Ship;
    public ShipTrackerComponent Component;

    public ShipTrackerDestroyed(EntityUid ship, ShipTrackerComponent component)
    {
        Ship = ship;
        Component = component;
    }
}
