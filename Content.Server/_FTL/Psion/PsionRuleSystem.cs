using System.Linq;
using Content.Server.Chat.Managers;
using Content.Server.GameTicking;
using Content.Server.GameTicking.Rules;
using Content.Server.GameTicking.Rules.Components;
using Content.Server.Mind;
using Content.Server.Objectives.Interfaces;
using Content.Server.Players;
using Content.Shared.CCVar;
using Content.Shared.Roles;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server._FTL.Psion;

public sealed class PsionRuleSystem : GameRuleSystem<PsionRuleComponent>
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly IObjectivesManager _objectivesManager = default!;
    [Dependency] private readonly IChatManager _chatManager = default!;
    [Dependency] private readonly SharedAudioSystem _audioSystem = default!;
    [Dependency] private readonly MindSystem _mindSystem = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RoundStartAttemptEvent>(OnStartAttempt);
        SubscribeLocalEvent<RulePlayerJobsAssignedEvent>(OnPlayersSpawned);
        SubscribeLocalEvent<RoundEndTextAppendEvent>(OnRoundEndText);
    }

    private void OnStartAttempt(RoundStartAttemptEvent ev)
    {
        var query = EntityQueryEnumerator<PsionRuleComponent, GameRuleComponent>();
        while (query.MoveNext(out var uid, out var psion, out var gameRule))
        {
            if (!GameTicker.IsGameRuleAdded(uid, gameRule))
                continue;

            var minPlayers = _cfg.GetCVar(CCVars.StowawayMinPlayers);
            if (!ev.Forced && ev.Players.Length < minPlayers)
            {
                _chatManager.SendAdminAnnouncement(Loc.GetString("psions-not-enough-ready-players",
                    ("readyPlayersCount", ev.Players.Length), ("minimumPlayers", minPlayers)));
                ev.Cancel();
                continue;
            }

            if (ev.Players.Length == 0)
            {
                _chatManager.DispatchServerAnnouncement(Loc.GetString("psions-no-one-ready"));
                ev.Cancel();
            }
        }
    }

    private void OnPlayersSpawned(RulePlayerJobsAssignedEvent ev)
    {
        var query = EntityQueryEnumerator<PsionRuleComponent, GameRuleComponent>();
        while (query.MoveNext(out var uid, out var psionRule, out var gameRule))
        {
            if (!GameTicker.IsGameRuleAdded(uid, gameRule))
                continue;

            var everyone = new List<IPlayerSession>(ev.Players);
            var prefList = new List<IPlayerSession>();

            foreach (var player in everyone)
            {
                if (!ev.Profiles.ContainsKey(player.UserId))
                {
                    continue;
                }

                var profile = ev.Profiles[player.UserId];
                if (profile.AntagPreferences.Contains(psionRule.RolePrototype))
                {
                    prefList.Add(player);
                }
            }

            var psionPlayer = _random.Pick(prefList);

            if (!psionPlayer.AttachedEntity.HasValue)
                return;

            var mind = psionPlayer.GetMind();
            var psionAntag = _prototypeManager.Index<AntagPrototype>(psionRule.RolePrototype);

            if (mind == null)
                return;

            _mindSystem.AddRole(mind, new PsionRole(mind, psionAntag));

            if (_mindSystem.TryGetSession(mind, out var session))
            {
                // Notify player about new role assignment
                _audioSystem.PlayGlobal(psionRule.GreetSoundNotification, session);
            }

            var maxPicks = psionRule.ObjectiveMaxPicks;
            for (var pick = 0; pick < maxPicks; pick++)
            {
                var objective = _objectivesManager.GetRandomObjective(mind, "PsionObjectiveGroups");

                if (objective == null)
                    continue;
                _mindSystem.TryAddObjective(mind, objective);
            }

            AddComp<PsionComponent>(psionPlayer.AttachedEntity.Value);
        }
    }

    private void OnRoundEndText(RoundEndTextAppendEvent ev)
    {
        // a
    }
}
