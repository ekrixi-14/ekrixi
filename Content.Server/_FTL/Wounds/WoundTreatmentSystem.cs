using Content.Server.DoAfter;
using Content.Server.Popups;
using Content.Server.Tools;
using Content.Shared._FTL.Wounds;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.Popups;
using Content.Shared.Tools;
using Content.Shared.Tools.Components;
using Content.Shared.Verbs;
using Robust.Server.GameObjects;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Server._FTL.Wounds;

/// <summary>
/// This handles the treating of wounds.
/// </summary>
/// <remarks>
/// This is a shitty work around because putting this code in shared doesn't let me delete the entities.
/// </remarks>
public sealed class WoundTreatmentSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly DamageableSystem _damageableSystem = default!;
    [Dependency] private readonly DoAfterSystem _doAfterSystem = default!;
    [Dependency] private readonly AudioSystem _audioSystem = default!;
    [Dependency] private readonly PopupSystem _popupSystem = default!;
    [Dependency] private readonly ToolSystem _toolSystem = default!;
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
        if (args.Cancelled)
            return;

        var currentWoundEntity = _entMan.GetEntity(args.Entity);
        var user = args.Args.User;
        var woundHolder = _entMan.GetEntity(args.WoundHolder);

        if (args.Handled)
            return;

        if (!TryComp<DamageableComponent>(woundHolder, out var damageable))
            return;

        var currentWound = EnsureComp<WoundComponent>(currentWoundEntity);

        // If the current treatment path is more than the treatment paths available
        // We know the last treatment path is performed, so remove the wound
        currentWound.CurrentTreatmentPath += 1;
        Dirty(currentWoundEntity, currentWound);

        if (currentWound.CurrentTreatmentPath < currentWound.TreatmentPaths.Count)
        {
            var currentPath = currentWound.TreatmentPaths[currentWound.CurrentTreatmentPath];
            Log.Debug($"On {currentWound.CurrentTreatmentPath} of {currentWound.TreatmentPaths.Count}");

            var endedMsgUser = Loc.GetString(currentPath.EndedMessage);
            var endedMsgOther = Loc.GetString(currentPath.EndedMessage + "-other", ("target", woundHolder));

            _popupSystem.PopupEntity(endedMsgUser, woundHolder, user);
            _popupSystem.PopupEntity(endedMsgOther, woundHolder, Filter.PvsExcept(user), true);
        }
        else
        {
            _popupSystem.PopupEntity(Loc.GetString("popup-wound-cured", ("target", uid), ("woundName", MetaData(currentWoundEntity).EntityName)), uid);
            QueueDel(currentWoundEntity);
            _damageableSystem.DamageChanged(uid, damageable);
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
            Text = $"Treat current wound ({Loc.GetString(quality.Name)} needed, {currentWound.CurrentTreatmentPath}/{currentWound.TreatmentPaths.Count})",
            Act = () =>
            {
                var currentlyHeld = args.Hands?.ActiveHand?.HeldEntity;
                if (!currentlyHeld.HasValue)
                {
                    _popupSystem.PopupClient(Loc.GetString("popup-wound-need-item", ("item", Loc.GetString(quality.Name))), uid, args.User);
                    return;
                }

                if (!_toolSystem.HasQuality(currentlyHeld.Value, quality.ID))
                {
                    _popupSystem.PopupClient(Loc.GetString("popup-wound-need-item", ("item", Loc.GetString(quality.Name))), uid, args.User);
                    return;
                }

                var tool = EnsureComp<ToolComponent>(currentlyHeld.Value);

                _audioSystem.PlayPvs(currentWound.TreatmentPaths[currentWound.CurrentTreatmentPath].TreatmentSound, uid);

                var startedMsgUser = Loc.GetString(currentPath.BeganMessage);
                var startedMsgOther = Loc.GetString(currentPath.BeganMessage + "-other", ("target", args.User));

                _popupSystem.PopupEntity(startedMsgUser, uid, args.User);
                _popupSystem.PopupEntity(startedMsgOther, uid, Filter.PvsExcept(args.User), true);

                var ev = new WoundTreatmentDoAfterEvent(
                    _entMan.GetNetEntity(currentWoundEntity),
                    _entMan.GetNetEntity(uid)
                );

                _doAfterSystem.TryStartDoAfter(new DoAfterArgs(_entMan, args.User, TimeSpan.FromSeconds(currentPath.TreatmentLength * tool.SpeedModifier), ev, uid)
                {
                    BreakOnHandChange = true,
                    BreakOnDamage = true,
                    BreakOnWeightlessMove = true,
                    //BreakOnTargetMove = true, // idk why upstream doesnt allow this
                    BreakOnUserMove = true,
                    NeedHand = true
                });
            }
        });
    }
}
