using Content.Shared.Damage;
using Content.Shared.DoAfter;
using JetBrains.Annotations;
using Robust.Shared.Containers;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared._FTL.Wounds;

[Serializable, NetSerializable]
public sealed partial class WoundTreatmentDoAfterEvent : DoAfterEvent
{
    public NetEntity Entity;
    public NetEntity WoundHolder;

    public WoundTreatmentDoAfterEvent(NetEntity entity, NetEntity woundHolder)
    {
        Entity = entity;
        WoundHolder = woundHolder;
    }

    public override DoAfterEvent Clone() => this;
}

/// <summary>
/// Handles the adding and managing of wounds.
/// </summary>
public sealed class SharedWoundsSystem : EntitySystem
{
    [Dependency] private readonly SharedContainerSystem _containerSystem = default!;
    [Dependency] private readonly DamageableSystem _damageableSystem = default!;
    public const string ContainerName = "wounds";

    // TODO: Integrate ALL of this with events

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<WoundsHolderComponent, ComponentInit>(OnComponentInit);
    }

    private void OnComponentInit(EntityUid uid, WoundsHolderComponent component, ComponentInit args)
    {
        component.Wounds = _containerSystem.EnsureContainer<Container>(uid, ContainerName);
    }

    /// <summary>
    /// Attempts to add a wound to an entity.
    /// </summary>
    /// <param name="woundPrototype"></param>
    /// <param name="uid"></param>
    /// <param name="component"></param>
    /// <returns></returns>
    public bool TryAddWound(EntProtoId woundPrototype, EntityUid uid, WoundsHolderComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return false;

        var entityId = woundPrototype.Id;
        if (component.Wounds.ContainedEntities.FirstOrNull(e => MetaData(e).EntityPrototype?.ID == entityId) != null)
            return false; // more than one...??!??g

        var wound = Spawn(woundPrototype);
        component.Wounds.Insert(wound);

        if (!TryComp<WoundComponent>(wound, out var woundComponent))
        {
            Log.Error($"Expected wound prototype ${woundPrototype} to have WoundComponent.");
        }
        else if (TryComp<DamageableComponent>(uid, out var damageableComponent))
        {
            _damageableSystem.DamageChanged(uid, damageableComponent, woundComponent.Damage, true, null);
        }

        return true;
    }

    /// <summary>
    /// Gets the total damage of all wounds from a WHC.
    /// </summary>
    /// <param name="component"></param>
    /// <returns></returns>
    [PublicAPI]
    public DamageSpecifier GetDamageFromWounds(WoundsHolderComponent component)
    {
        var damage = new DamageSpecifier();

        foreach (var entity in component.Wounds.ContainedEntities)
        {
            var wound = EnsureComp<WoundComponent>(entity);
            if (wound.Damage != null)
                damage = wound.Damage + damage;
        }

        return damage;
    }

    /// <summary>
    /// Attempts to get damage from wounds given an entity.
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="component"></param>
    /// <param name="spec"></param>
    /// <returns></returns>
    [PublicAPI]
    public bool TryGetDamageFromWounds(EntityUid uid, WoundsHolderComponent? component, out DamageSpecifier spec)
    {
        if (!Resolve(uid, ref component, false))
        {
            spec = new DamageSpecifier();
            return false;
        }

        spec = GetDamageFromWounds(component);

        return true;
    }
}
