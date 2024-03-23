using Content.Shared.Damage;
using JetBrains.Annotations;
using Robust.Shared.Containers;
using Robust.Shared.Prototypes;

namespace Content.Shared._FTL.Wounds;

public sealed class WoundsSystem : EntitySystem
{
    [Dependency] private SharedContainerSystem _containerSystem = default!;
    [Dependency] private DamageableSystem _damageableSystem = default!;
    public const string ContainerName = "wounds";

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

    public bool TryAddWound(EntProtoId woundPrototype, EntityUid uid, WoundsHolderComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return false;

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

    [PublicAPI]
    public DamageSpecifier GetDamageFromWounds(WoundsHolderComponent component)
    {
        var damage = new DamageSpecifier();

        foreach (var entity in component.Wounds.ContainedEntities)
        {
            var wound = EnsureComp<WoundComponent>(entity);
            damage = wound.Damage + damage;
        }

        return damage;
    }

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
