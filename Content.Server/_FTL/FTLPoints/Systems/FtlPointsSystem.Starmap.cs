using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using Content.Server._FTL.FTLPoints.Components;
using Content.Server.UserInterface;
using Content.Shared._FTL.FtlPoints;
using JetBrains.Annotations;
using Robust.Server.GameStates;
using Robust.Shared.Map;

namespace Content.Server._FTL.FTLPoints.Systems;

/// <summary>
/// This handles managing the star map singleton, such as getting stars in range, and other stuff.
/// </summary>
public sealed partial class FtlPointsSystem
{
    [Dependency] private readonly PvsOverrideSystem _pvs = default!;

    private void OnInit(EntityUid uid, StarMapComponent component, ComponentStartup args)
    {
        _pvs.AddGlobalOverride(uid);
    }

    // ew i know singletons suck but this is the only thing that makes sense
    public bool TryGetStarMap([NotNullWhen(true)] ref StarMapComponent? component)
    {
        if (component != null)
            return true;

        var query = EntityQuery<StarMapComponent>().ToList();
        component = !query.Any() ? CreatePointManager() : query.First();
        return true;
    }

    private StarMapComponent CreatePointManager()
    {
        var manager = Spawn(null, MapCoordinates.Nullspace);
        return EnsureComp<StarMapComponent>(manager);
    }

    private void OnToggleInterface(EntityUid uid, StarmapConsoleComponent? component, AfterActivatableUIOpenEvent args)
    {
        UpdateUserInterface(uid, component);
    }

    private void UpdateUserInterface(EntityUid uid, StarmapConsoleComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return;

        var xform = Transform(uid);
        Log.Info("Tried to get transform");
        if (!xform.MapUid.HasValue)
            return; // huh???

        var star = GetStarWithMapId(xform.MapID);
        Log.Info("Tried to find star");
        if (!star.HasValue)
            return;
        Log.Info("Got star, sending state");

        var stars = GetStarsInRange(star.Value.Position, 30f);
        var state = new StarmapConsoleBoundUserInterfaceState(stars);

        _userInterface.TrySetUiState(uid, StarmapConsoleUiKey.Key, state);
    }

    #region Public API

    [PublicAPI]
    public List<Star> GetStarsInRange(Vector2 position, float range, StarMapComponent? component = null)
    {
        var list = new List<Star>();
        if (!TryGetStarMap(ref component))
            return list;

        list.AddRange(component.StarMap.Where(star => Vector2.Distance(position, star.Position) <= range));

        return list;
    }

    [PublicAPI]
    public Star? GetStarWithMapId(MapId map, StarMapComponent? component = null)
    {
        if (!TryGetStarMap(ref component))
            return default;

        foreach (var star in component.StarMap.Where(star => star.Map == map))
        {
            return star;
        }

        return null;
    }

    [PublicAPI]
    public Star? GetStarWithPosition(Vector2 pos, StarMapComponent? component = null)
    {
        if (!TryGetStarMap(ref component))
            return default;

        foreach (var star in component.StarMap.Where(star => star.Position == pos))
        {
            return star;
        }

        return null;
    }

    [PublicAPI]
    public void TryAddPoint(MapId mapId, Vector2 position, string name, StarMapComponent? component = null)
    {
        if (TryGetStarMap(ref component))
            component.StarMap.Add(new Star(position, mapId, name));
    }

    #endregion
}
