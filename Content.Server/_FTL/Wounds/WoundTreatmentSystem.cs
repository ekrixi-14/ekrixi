using Content.Server.DoAfter;
using Content.Server.Popups;
using Content.Shared._FTL.Wounds;
using Content.Shared.DoAfter;
using Content.Shared.Popups;
using Content.Shared.Tools;
using Content.Shared.Verbs;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Server._FTL.Wounds;

[Serializable, NetSerializable]
public sealed partial class WoundTreatmentDoAfterEvent : DoAfterEvent
{
    public EntityUid Entity;
    public WoundComponent Wound;

    public WoundTreatmentDoAfterEvent(EntityUid entity, WoundComponent component)
    {
        Entity = entity;
        Wound = component;
    }

    public override DoAfterEvent Clone() => this;
}

/// <summary>
/// This handles the treating of wounds.
/// </summary>
/// <remarks>
/// This is a shitty work around because putting this code in shared doesn't let me delete the entities.
/// </remarks>
public sealed class WoundTreatmentSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly DoAfterSystem _doAfterSystem = default!;
    [Dependency] private readonly PopupSystem _popupSystem = default!;
    [Dependency] private readonly IEntityManager _entMan = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<WoundsHolderComponent, GetVerbsEvent<AlternativeVerb>>(AddTreatVerb);
        SubscribeLocalEvent<WoundsHolderComponent, WoundTreatmentDoAfterEvent>(OnDoAfter);
    }

    private void OnDoAfter(EntityUid uid, WoundsHolderComponent component, WoundTreatmentDoAfterEvent args)
    {
        // If the current treatment path is more than the treatment paths available
        // We know the last treatment path is performed, so remove the wound
        args.Wound.CurrentTreatmentPath += 1;
        Dirty(args.Entity, args.Wound);

        var currentPath = args.Wound.TreatmentPaths[args.Wound.CurrentTreatmentPath];

        Log.Debug($"On {args.Wound.CurrentTreatmentPath} of {args.Wound.TreatmentPaths.Count}");

        if (args.Wound.CurrentTreatmentPath < args.Wound.TreatmentPaths.Count)
        {
            var startedMsgUser = Loc.GetString(currentPath.EndedMessage);
            var startedMsgOther = Loc.GetString(currentPath.EndedMessage + "-other", ("target", args.User));

            _popupSystem.PopupEntity(startedMsgUser, args.User, args.User);
            _popupSystem.PopupEntity(startedMsgOther, args.User, Filter.PvsExcept(args.User), true);
        }
        else
        {
            QueueDel(args.Entity);
            _popupSystem.PopupEntity(Loc.GetString("popup-wound-cured", ("target", uid), ("woundName", MetaData(args.Entity).EntityName)), uid);
        }
    }

    private void AddTreatVerb(EntityUid uid, WoundsHolderComponent component, GetVerbsEvent<AlternativeVerb> args)
    {
        if (!args.CanInteract)
            return;

        if (component.Wounds.ContainedEntities.Count <= 0)
            return; // why show a treat menu when theres nothing to treat

        for (var i = 0; i < component.Wounds.ContainedEntities.Count; i++)
        {
            var wound = component.Wounds.ContainedEntities[i];

            if (!TryComp<WoundComponent>(wound, out _))
                return;

            var meta = MetaData(wound);

            var i1 = i;
            args.Verbs.Add(new AlternativeVerb
            {
                Text = $"{meta.EntityName}",
                Act = () =>
                {
                    component.CurrentWoundTreating = i1;
                },
                Disabled = i == component.CurrentWoundTreating,
                Category = VerbCategory.SelectWound
            });
        }

        // Hamlet? From Don't Starve Hamlet (DLC?)
        // gardeners are RUINING ss14
        // i do not want to hear it... EVER
        // -flare

        // Get the currently selected wound
        var currentWoundEntity = component.Wounds.ContainedEntities[component.CurrentWoundTreating];
        if (!TryComp<WoundComponent>(currentWoundEntity, out var currentWound))
            return; // If it doesnt have a wound comp wtf is it doing here???

        // Get the current treatment path
        var currentPath = currentWound.TreatmentPaths[currentWound.CurrentTreatmentPath];

        if (args.Hands == null)
        {
            _popupSystem.PopupClient(Loc.GetString("popup-wound-need-hand"), uid, args.User);
            return; // you need hands
        }

        if (args.Hands.ActiveHand == null)
        {
            _popupSystem.PopupClient(Loc.GetString("popup-wound-need-hand"), uid, args.User);
            return; // you need A hand at least
        }

        var quality = _prototypeManager.Index<ToolQualityPrototype>(currentPath.ToolQuality.Id);

        args.Verbs.Add(new AlternativeVerb
        {
            Text = $"Treat current wound ({Loc.GetString(quality.Name)} tool needed)",
            Act = () =>
            {
                var currentlyHeld = args.Hands?.ActiveHand?.HeldEntity;
                if (!currentlyHeld.HasValue)
                {
                    _popupSystem.PopupClient(Loc.GetString("popup-wound-need-item", ("item", Loc.GetString(quality.Name))), uid, args.User);
                    return;
                }

                var ev = new WoundTreatmentDoAfterEvent(currentWoundEntity, currentWound);

                _doAfterSystem.TryStartDoAfter(new DoAfterArgs(_entMan, args.User, currentPath.TreatmentLength, ev, uid)
                {
                    BreakOnHandChange = true,
                    BreakOnDamage = true,
                    BreakOnWeightlessMove = true,
                    BreakOnTargetMove = true,
                    BreakOnUserMove = true,
                    NeedHand = true
                });
            }
        });
    }
}
