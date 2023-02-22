using System.Linq;
using Content.Server.Arcade.Components;
using Content.Server.UserInterface;
using Content.Shared.Arcade;
using Robust.Shared.Utility;
using Robust.Server.GameObjects;
using Robust.Server.Player;

namespace Content.Server.Arcade
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed partial class ArcadeSystem : EntitySystem
    {
        private readonly List<BlockGameMessages.HighScoreEntry> _roundHighscores = new();
        private readonly List<BlockGameMessages.HighScoreEntry> _globalHighscores = new();

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<BlockGameArcadeComponent, AfterActivatableUIOpenEvent>(OnAfterUIOpen);
            SubscribeLocalEvent<SpaceVillainArcadeComponent, AfterActivatableUIOpenEvent>(OnAfterUIOpenSV);
            SubscribeLocalEvent<BlockGameArcadeComponent, BoundUIClosedEvent>((_,c,args) => c.UnRegisterPlayerSession((IPlayerSession)args.Session));
            InitializeBlockGame();
            InitializeSpaceVillain();
        }

        private void OnAfterUIOpen(EntityUid uid, BlockGameArcadeComponent component, AfterActivatableUIOpenEvent args)
        {
            var actor = Comp<ActorComponent>(args.User);
            if (component.UserInterface?.SessionHasOpen(actor.PlayerSession) == true)
            {
                 component.RegisterPlayerSession(actor.PlayerSession);
            }
        }

        private void OnAfterUIOpenSV(EntityUid uid, SpaceVillainArcadeComponent component, AfterActivatableUIOpenEvent args)
        {
            component.Game ??= new SpaceVillainArcadeComponent.SpaceVillainGame(component);
        }

        public HighScorePlacement RegisterHighScore(string name, int score)
        {
            var entry = new BlockGameMessages.HighScoreEntry(name, score);
            return new HighScorePlacement(TryInsertIntoList(_roundHighscores, entry), TryInsertIntoList(_globalHighscores, entry));
        }

        public List<BlockGameMessages.HighScoreEntry> GetLocalHighscores() => GetSortedHighscores(_roundHighscores);

        public List<BlockGameMessages.HighScoreEntry> GetGlobalHighscores() => GetSortedHighscores(_globalHighscores);

        private List<BlockGameMessages.HighScoreEntry> GetSortedHighscores(List<BlockGameMessages.HighScoreEntry> highScoreEntries)
        {
            var result = highScoreEntries.ShallowClone();
            result.Sort((p1, p2) => p2.Score.CompareTo(p1.Score));
            return result;
        }

        private int? TryInsertIntoList(List<BlockGameMessages.HighScoreEntry> highScoreEntries, BlockGameMessages.HighScoreEntry entry)
        {
            if (highScoreEntries.Count < 5)
            {
                highScoreEntries.Add(entry);
                return GetPlacement(highScoreEntries, entry);
            }

            if (highScoreEntries.Min(e => e.Score) >= entry.Score) return null;

            var lowestHighscore = highScoreEntries.Min();

            if (lowestHighscore == null) return null;

            highScoreEntries.Remove(lowestHighscore);
            highScoreEntries.Add(entry);
            return GetPlacement(highScoreEntries, entry);

        }

        private int? GetPlacement(List<BlockGameMessages.HighScoreEntry> highScoreEntries, BlockGameMessages.HighScoreEntry entry)
        {
            int? placement = null;
            if (highScoreEntries.Contains(entry))
            {
                highScoreEntries.Sort((p1,p2) => p2.Score.CompareTo(p1.Score));
                placement = 1 + highScoreEntries.IndexOf(entry);
            }

            return placement;
        }

        public override void Update(float frameTime)
        {
            foreach (var comp in EntityManager.EntityQuery<BlockGameArcadeComponent>())
            {
                comp.DoGameTick(frameTime);
            }
        }

        public readonly struct HighScorePlacement
        {
            public readonly int? GlobalPlacement;
            public readonly int? LocalPlacement;

            public HighScorePlacement(int? globalPlacement, int? localPlacement)
            {
                GlobalPlacement = globalPlacement;
                LocalPlacement = localPlacement;
            }
        }
    }
}
