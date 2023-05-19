using Content.Server.Storage.Components;
using Content.Shared._FTL.Weapons;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Damage.Systems;
using Content.Shared.FixedPoint;
using Content.Shared.Storage.Components;
using Robust.Server.GameObjects;
using Robust.Shared.Containers;
using Robust.Shared.Prototypes;

namespace Content.Server._FTL.Weapons;

/// <inheritdoc/>
public sealed class WeaponTargetingSystem : SharedWeaponTargetingSystem
{
    [Dependency] private readonly UserInterfaceSystem _uiSystem = default!;
    [Dependency] private readonly SharedContainerSystem _containerSystem = default!;
    [Dependency] private readonly PhysicsSystem _physicsSystem = default!;
    [Dependency] private readonly DamageableSystem _damageableSystem = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly StaminaSystem _staminaSystem = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<WeaponTargetingUserComponent, EntParentChangedMessage>(OnUserParentChanged);
        SubscribeLocalEvent<WeaponTargetingComponent, BoundUIOpenedEvent>(OnStationMapOpened);
        SubscribeLocalEvent<WeaponTargetingComponent, BoundUIClosedEvent>(OnStationMapClosed);
        SubscribeLocalEvent<WeaponTargetingComponent, FireWeaponSendMessage>(OnFireWeaponSendMessage);

        SubscribeLocalEvent<FTLWeaponSiloComponent, StorageAfterCloseEvent>(OnClose);
        SubscribeLocalEvent<FTLWeaponSiloComponent, StorageAfterOpenEvent>(OnOpen);
    }

    private void OnClose(EntityUid uid, FTLWeaponSiloComponent component, ref StorageAfterCloseEvent args)
    {
        TryComp<EntityStorageComponent>(uid, out var container);
        if (container == null)
            return;
        component.ContainedEntities = new List<EntityUid>();
        foreach (var entity in container.Contents.ContainedEntities)
        {
            component.ContainedEntities.Add(entity);
        }
    }

    private void OnOpen(EntityUid uid, FTLWeaponSiloComponent component, ref StorageAfterOpenEvent args)
    {
        if (component.ContainedEntities == null)
            return;

        var transform = Transform(uid);
        foreach (var entity in component.ContainedEntities)
        {
            Logger.Debug(entity.ToString());
            Logger.Debug(transform.LocalRotation.ToWorldVec().ToString());
            _physicsSystem.ApplyLinearImpulse(entity, -(transform.LocalRotation.ToWorldVec() * 100000f));
            var damage = new DamageSpecifier(_prototypeManager.Index<DamageGroupPrototype>("Brute"),
                FixedPoint2.New(50));
            _damageableSystem.TryChangeDamage(entity, damage);
            _staminaSystem.TakeStaminaDamage(entity, 100f);
        }
    }

    private void OnFireWeaponSendMessage(EntityUid uid, WeaponTargetingComponent component, FireWeaponSendMessage args)
    {
        if (!Equals(args.UiKey, WeaponTargetingUiKey.Key) || args.Session.AttachedEntity == null)
            return;

        RemCompDeferred<WeaponTargetingUserComponent>(args.Session.AttachedEntity.Value);
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
            _uiSystem.TryClose(component.Map, WeaponTargetingUiKey.Key, actor.PlayerSession);
        }
    }

    private void OnStationMapOpened(EntityUid uid, WeaponTargetingComponent component, BoundUIOpenedEvent args)
    {
        if (args.Session.AttachedEntity == null)
            return;

        var comp = EnsureComp<WeaponTargetingUserComponent>(args.Session.AttachedEntity.Value);
        var state = new WeaponTargetingUserInterfaceState(component.CanFire);
        _uiSystem.TrySetUiState(uid, WeaponTargetingUiKey.Key, state);
        comp.Map = uid;
    }
}
