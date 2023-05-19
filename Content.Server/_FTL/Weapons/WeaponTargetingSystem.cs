using Content.Shared._FTL.Weapons;
using Robust.Server.GameObjects;

namespace Content.Server._FTL.Weapons;

/// <inheritdoc/>
public sealed class WeaponTargetingSystem : SharedWeaponTargetingSystem
{
    [Dependency] private readonly UserInterfaceSystem _ui = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<WeaponTargetingUserComponent, EntParentChangedMessage>(OnUserParentChanged);
        SubscribeLocalEvent<WeaponTargetingComponent, BoundUIOpenedEvent>(OnStationMapOpened);
        SubscribeLocalEvent<WeaponTargetingComponent, BoundUIClosedEvent>(OnStationMapClosed);
    }

    private void OnStationMapClosed(EntityUid uid, WeaponTargetingComponent component, BoundUIClosedEvent args)
    {
        if (!Equals(args.UiKey, WeaponTargetingUiKey.Key) || args.Session.AttachedEntity == null)
            return;

        RemCompDeferred<WeaponTargetingUserComponent>(args.Session.AttachedEntity.Value);
    }

    private void OnUserParentChanged(EntityUid uid, WeaponTargetingUserComponent component, ref EntParentChangedMessage args)
    {
        if (TryComp<ActorComponent>(uid, out var actor))
        {
            _ui.TryClose(component.Map, WeaponTargetingUiKey.Key, actor.PlayerSession);
        }
    }

    private void OnStationMapOpened(EntityUid uid, WeaponTargetingComponent component, BoundUIOpenedEvent args)
    {
        if (args.Session.AttachedEntity == null)
            return;

        var comp = EnsureComp<WeaponTargetingUserComponent>(args.Session.AttachedEntity.Value);
        var state = new WeaponTargetingUserInterfaceState(component.CanFire);
        _ui.TrySetUiState(uid, WeaponTargetingUiKey.Key, state);
        comp.Map = uid;
    }
}
