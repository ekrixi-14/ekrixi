using System.Numerics;
using Content.Server._FTL.FTLPoints.Components;
using Content.Server._FTL.FTLPoints.Effects;
using Content.Server._FTL.FTLPoints.Prototypes;
using Content.Server.Shuttles.Components;
using Content.Server.Shuttles.Systems;
using Content.Server.UserInterface;
using Content.Shared._FTL.FtlPoints;
using Content.Shared.Dataset;
using Content.Shared.Parallax;
using Content.Shared.Random;
using Content.Shared.Random.Helpers;
using Content.Shared.Salvage;
using Robust.Server.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server._FTL.FTLPoints.Systems;

/// <summary>
/// This handles the generation of FTL points
/// </summary>
public sealed partial class FtlPointsSystem : SharedFtlPointsSystem
{
    [Dependency] private readonly EntityManager _entManager = default!;
    [Dependency] private readonly IMapManager _mapManager = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly MetaDataSystem _metaDataSystem = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly ShuttleConsoleSystem _consoleSystem = default!;
    [Dependency] private readonly UserInterfaceSystem _userInterface = default!;
    [Dependency] private readonly ShuttleSystem _shuttleSystem = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<StarMapComponent, ComponentStartup>(OnInit);
        SubscribeLocalEvent<StarmapConsoleComponent, AfterActivatableUIOpenEvent>(OnToggleInterface);
        SubscribeLocalEvent<StarmapConsoleComponent, WarpToStarMessage>(OnWarpToStarMessage);
    }

    public void GenerateSector(Vector2 starRange)
    {
        GenerateSector((int) starRange.X, (int) starRange.Y);
    }

    /// <summary>
    /// Generates a sector with varying distance
    /// </summary>
    /// <param name="minStars"></param>
    /// <param name="maxStars"></param>
    public void GenerateSector(int minStars, int maxStars)
    {
        var preferredPointAmount = _random.Next(minStars, maxStars);

        for (var i = 0; i < preferredPointAmount; i++)
        {
            var prototype = _prototypeManager.Index<FtlPointPrototype>(_prototypeManager.Index<WeightedRandomPrototype>("FTLPoints").Pick());
            Log.Info($"Picked {prototype} as point type.");
            if (!_random.Prob(prototype.Probability))
                continue;
            var mapId = GeneratePoint(prototype);
            var mapUid = _mapManager.GetMapEntityId(mapId);
            TryAddPoint(mapId, new Vector2(
                _random.NextFloat(-10, 10),
                _random.NextFloat(-10, 10)
            ), MetaData(mapUid).EntityName);
        }

        Log.Debug("Generated a brand new sector.");
    }

    /// <summary>
    /// Generates a temporary disposable FTL point.
    /// </summary>
    public MapId GeneratePoint(FtlPointPrototype prototype)
    {
        // create map

        var mapId = _mapManager.CreateMap();
        var mapUid = _mapManager.GetMapEntityId(mapId);

        // make it ftlable
        EnsureComp<FTLDestinationComponent>(mapUid);
        _metaDataSystem.SetEntityName(mapUid, $"[{Loc.GetString(prototype.Tag)}] {
            SharedSalvageSystem.GetFTLName(_prototypeManager.Index<DatasetPrototype>("names_borer"), _random.Next())}");
        _consoleSystem.RefreshShuttleConsoles();

        // add parallax
        var parallaxes = new[]
        {
            "AspidParallax",
            "KettleStation",
            "Default",
            "Blank",
            "BagelStation",
            "Blue_Nebula_01",
            "Blue_Nebula_02",
            "Blue_Nebula_03",
            "Blue_Nebula_04",
            "Green_Nebula_01",
            "Green_Nebula_02",
            "Green_Nebula_03",
            "Green_Nebula_04",
            "Green_Nebula_06",
            "Green_Nebula_07",
            "Green_Nebula_08",
            "Purple_Nebula_01",
            "Purple_Nebula_02",
            "Purple_Nebula_03",
            "Purple_Nebula_04",
            "Purple_Nebula_05",
            "Purple_Nebula_08"
        };
        var parallax = EnsureComp<ParallaxComponent>(mapUid);
        parallax.Parallax = _random.Pick(parallaxes);

        // spawn the stuff
        foreach (var effect in prototype.FtlPointEffects)
        {
            if (_random.Prob(effect.Probability))
            {
                effect.Effect(new FtlPointEffect.FtlPointEffectArgs(mapUid, mapId, _entManager, _mapManager));
            }
        }

        return mapId;
    }
}
