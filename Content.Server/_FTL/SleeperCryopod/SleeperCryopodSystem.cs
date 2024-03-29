using System.Linq;
using Content.Server.Mind;
using Content.Server.Players;
using Content.Server.Spawners.EntitySystems;
using Content.Server.Station.Systems;
using Content.Shared._FTL.SleeperCryopod;
using Content.Shared.ActionBlocker;
using Content.Shared.Bed.Sleep;
using Content.Shared.Climbing.Systems;
using Content.Shared.Destructible;
using Content.Shared.DragDrop;
using Content.Shared.Examine;
using Content.Shared.Mobs.Components;
using Content.Shared.Players;
using Content.Shared.Roles.Jobs;
using Content.Shared.StatusEffect;
using Content.Shared.Verbs;
using Robust.Server.Containers;
using Robust.Server.GameObjects;
using Robust.Shared.Containers;
using Robust.Shared.Player;
using Robust.Shared.Random;

namespace Content.Server._FTL.SleeperCryopod;

public sealed class SleeperCryopodSystem : EntitySystem
{
    [Dependency] private readonly ActionBlockerSystem _actionBlocker = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly ContainerSystem _container = default!;
    [Dependency] private readonly ClimbSystem _climb = default!;
    [Dependency] private readonly StationSpawningSystem _stationSpawning = default!;
    [Dependency] private readonly StatusEffectsSystem _statusEffects = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly AppearanceSystem _appearanceSystem = default!;
    [Dependency] private readonly StationJobsSystem _stationJobsSystem = default!;
    [Dependency] private readonly SharedJobSystem _sharedJobSystem = default!;
    [Dependency] private readonly StationSystem _stationSystem = default!;
    [Dependency] private readonly MindSystem _mindSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SleeperCryopodComponent, ComponentInit>(OnInit);
        SubscribeLocalEvent<SleeperCryopodComponent, GetVerbsEvent<AlternativeVerb>>(AddAlternativeVerbs);
        SubscribeLocalEvent<SleeperCryopodComponent, ExaminedEvent>(OnExamine);
        SubscribeLocalEvent<SleeperCryopodComponent, DestructionEventArgs>((e,c,_) => EjectBody(e, c));
        SubscribeLocalEvent<SleeperCryopodComponent, DragDropDraggedEvent>(OnDragDrop);
        SubscribeLocalEvent<SleeperCryopodComponent, DragDropTargetEvent>(OnDragDropTarget);
        SubscribeLocalEvent<PlayerSpawningEvent>(OnSpawning, before: new [] {typeof(SpawnPointSystem)});
    }

    private void OnDragDropTarget(EntityUid uid, SleeperCryopodComponent component, ref DragDropTargetEvent args)
    {
        if (args.Handled)
            return;

        var body = InsertBody(uid, args.Dragged, component);
        args.Handled = body;
    }

    private void OnDragDrop(EntityUid uid, SleeperCryopodComponent component, ref DragDropDraggedEvent args)
    {
        if (args.Handled)
            return;

        var body = InsertBody(uid, args.Target, component);
        args.Handled = body;
    }

    private void OnInit(EntityUid uid, SleeperCryopodComponent component, ComponentInit args)
    {
        component.BodyContainer = _container.EnsureContainer<ContainerSlot>(uid, "body_container");
        SetAppearance(uid, true);
    }

    private void OnSpawning(PlayerSpawningEvent args)
    {
        if (args.SpawnResult != null)
            return;

        var validPods = EntityQuery<SleeperCryopodComponent>().Where(c => !IsOccupied(c)).ToArray();
        _random.Shuffle(validPods);

        if (!validPods.Any())
            return;

        var pod = validPods.First();
        var xform = Transform(pod.Owner);

        args.SpawnResult = _stationSpawning.SpawnPlayerMob(xform.Coordinates, args.Job, args.HumanoidCharacterProfile, args.Station);
        _audio.PlayPvs(pod.ArrivalSound, pod.Owner);

        InsertBody(pod.Owner, args.SpawnResult.Value, pod);
        var duration = _random.NextFloat(pod.InitialSleepDurationRange.X, pod.InitialSleepDurationRange.Y);
        _statusEffects.TryAddStatusEffect<SleepingComponent>(args.SpawnResult.Value, "ForcedSleep", TimeSpan.FromSeconds(duration), false);
    }

    private void AddAlternativeVerbs(EntityUid uid, SleeperCryopodComponent component, GetVerbsEvent<AlternativeVerb> args)
    {
        if (!args.CanAccess || !args.CanInteract)
            return;

        // Eject verb
        if (IsOccupied(component))
        {
            AlternativeVerb verb = new()
            {
                Act = () => EjectBody(uid, component),
                Category = VerbCategory.Eject,
                Text = Loc.GetString("medical-scanner-verb-noun-occupant")
            };
            args.Verbs.Add(verb);
        }

        // Self-insert verb
        if (!IsOccupied(component) &&
            _actionBlocker.CanMove(args.User))
        {
            AlternativeVerb verb = new()
            {
                Act = () => InsertBody(uid, args.User, component),
                Category = VerbCategory.Insert,
                Text = Loc.GetString("medical-scanner-verb-enter")
            };
            args.Verbs.Add(verb);
        }
    }

    private void OnExamine(EntityUid uid, SleeperCryopodComponent component, ExaminedEvent args)
    {
        var message = component.BodyContainer.ContainedEntity == null
            ? "cryopod-examine-empty"
            : "cryopod-examine-occupied";

        args.PushMarkup(Loc.GetString(message));
    }

    private bool InsertBody(EntityUid uid, EntityUid? toInsert, SleeperCryopodComponent component)
    {
        if (toInsert == null)
            return false;

        if (IsOccupied(component))
            return false;

        if (!HasComp<MobStateComponent>(toInsert.Value))
            return false;
        var inserted = component.BodyContainer.Insert(toInsert.Value, EntityManager);
        SetAppearance(uid, false);

        return inserted;
    }

    private bool EjectBody(EntityUid pod, SleeperCryopodComponent component)
    {
        if (!IsOccupied(component))
            return false;

        var toEject = component.BodyContainer.ContainedEntity;
        if (toEject == null)
            return false;

        component.BodyContainer.Remove(toEject.Value);
        _climb.ForciblySetClimbing(toEject.Value, pod);

        SetAppearance(pod, true);
        return true;
    }

    private bool IsOccupied(SleeperCryopodComponent component)
    {
        return component.BodyContainer.ContainedEntity != null;
    }

    private void SetAppearance(EntityUid uid, bool open)
    {
        _appearanceSystem.SetData(uid, SleeperCryopodVisuals.Open, open);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<SleeperCryopodComponent>();
        while (query.MoveNext(out var entity, out var component))
        {
            if (!component.BodyContainer.ContainedEntity.HasValue)
                continue;

            if (TryComp<ActorComponent>(component.BodyContainer.ContainedEntity.Value, out var actorComponent))
            {
                component.TimeSinceBraindeath = 0f;
                var mind = actorComponent.PlayerSession.GetMind();
                if (!_sharedJobSystem.MindTryGetJob(mind, out _, out var prototype))
                    return;
                component.CryosleptJob = prototype;
                continue;
            }

            component.TimeSinceBraindeath += frameTime;

            if (component.TimeSinceBraindeath < component.BraindeadMaxTimer)
                continue;

            var job = component.CryosleptJob;
            EntityManager.DeleteEntity(component.BodyContainer.ContainedEntity.Value);
            Log.Debug("Deleted entity after exceeding braindead timer");
            _audio.PlayPvs(component.LeaveSound, entity);
            SetAppearance(entity, true);

            component.TimeSinceBraindeath = 0f;

            // sets job slot
            var xform = Transform(entity);
            if (!xform.GridUid.HasValue)
                return;
            var station = _stationSystem.GetOwningStation(xform.GridUid.Value);
            if (!station.HasValue)
                return;
            if (job == null)
                return;
            _stationJobsSystem.TryGetJobSlot(station.Value, job, out var amount);
            if (!amount.HasValue)
                return;
            _stationJobsSystem.TrySetJobSlot(station.Value, job, (int) amount.Value + 1, true);
        }
    }
}
