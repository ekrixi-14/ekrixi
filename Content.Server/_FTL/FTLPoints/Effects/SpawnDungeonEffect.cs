using Content.Server._FTL.ShipTracker.Systems;
using Content.Server.Procedural;
using Content.Shared.Procedural;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

namespace Content.Server._FTL.FTLPoints.Effects;

[DataDefinition]
public sealed partial class SpawnDungeonEffect : FTLPointEffect
{
    [DataField("configPrototypes")]
    public List<string> ConfigPrototypes { set; get; } = new()
    {
        "Experiment",
        "LavaBrig",
    };

    [DataField("minSpawn")] public int MinSpawn = 1;
    [DataField("maxSpawn")] public int MaxSpawn = 2;
    [DataField("range")] public int SpawnRange = 200;

    // fauna spawn
    [DataField("faunaSpawns", customTypeSerializer: typeof(PrototypeIdListSerializer<EntityPrototype>))]
    public List<string> FaunaSpawns = new()
    {
        "MobXeno",
        "MobXenoPraetorian",
        "MobXenoDrone",
        "MobXenoRavager",
        "MobXenoRunner",
        "MobXenoSpitter"
    };
    [DataField("maxFaunaSpawn")] public int MaxFaunaSpawn = 5;
    [DataField("faunaSpawnMultiplier")] public int FaunaSpawnMultiplier = 10;

    public override void Effect(FTLPointEffectArgs args)
    {
        var random = IoCManager.Resolve<IRobustRandom>();
        var shipTracker = args.EntityManager.System<ShipTrackerSystem>();
        var amountToSpawn = random.Next(MinSpawn, MaxSpawn);

        for (var i = 0; i < amountToSpawn; i++)
        {
            var dungeon = args.EntityManager.System<DungeonSystem>();
            var prototype = IoCManager.Resolve<IPrototypeManager>();

            var position = new Vector2i(random.Next(-SpawnRange, SpawnRange), random.Next(-SpawnRange, SpawnRange));
            var dungeonUid = args.MapManager.GetMapEntityId(args.MapId);

            if (!args.EntityManager.TryGetComponent<MapGridComponent>(dungeonUid, out var dungeonGrid))
            {
                dungeonUid = args.EntityManager.CreateEntityUninitialized(null, new EntityCoordinates(dungeonUid, position));
                dungeonGrid = args.EntityManager.EnsureComponent<MapGridComponent>(dungeonUid);
                args.EntityManager.InitializeAndStartEntity(dungeonUid, args.MapId);
            }

            var seed = new Random().Next();

            if (!prototype.TryIndex<DungeonConfigPrototype>(random.Pick(ConfigPrototypes), out var dungeonProto))
            {
                return;
            }

            if (!args.EntityManager.TryGetComponent<MapGridComponent>(dungeonUid, out var _))
            {
                Logger.Warning($"Dungeon {dungeonUid} did not have a MapGridComponent.");
                return;
            }

            dungeon.GenerateDungeon(dungeonProto, dungeonUid, dungeonGrid, position, seed);

            if (FaunaSpawns.Count <= 0)
                return;

            var amountFaunaToSpawn = random.Next(1, MaxFaunaSpawn * FaunaSpawnMultiplier);

            for (var j = 0; j < amountFaunaToSpawn; j++)
            {
                var entityPrototype = random.Pick(FaunaSpawns);
                if (!shipTracker.TryFindRandomTile(dungeonUid, out var tile, out var targetCoords, false))
                    continue;

                args.EntityManager.SpawnEntity(entityPrototype, targetCoords);
            }
        }
    }
}
