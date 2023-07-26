using System.Linq;
using Content.Shared.Examine;
using Robust.Shared.Audio;
using Robust.Shared.Random;

namespace Content.Server._FTL.ShipTracker.Systems;

public sealed partial class ShipTrackerSystem
{
    /// <summary>
    /// Gets the ship health
    /// </summary>
    /// <param name="grid">Entity UID</param>
    /// <param name="shieldAmount">How many shields are active</param>
    /// <param name="shieldCapacity">Total amount</param>
    /// <param name="component">ShipTracker comp</param>
    /// <returns>True if success, false otherwise</returns>
    public bool TryGetShieldHealth(EntityUid grid, out int shieldAmount, out int shieldCapacity, ShipTrackerComponent? component = null)
    {
        shieldAmount = 0;
        shieldCapacity = 0;

        if (!Resolve(grid, ref component))
            return false;

        var capacityGenerators = EntityQuery<ShieldGeneratorComponent>().Where(shield => Transform(shield.Owner).GridUid == grid).ToList();
        var activeGenerators = EntityQuery<ShieldGeneratorComponent>()
            .Where(shield => shield.Enabled && Transform(shield.Owner).GridUid == grid).ToList();

        shieldCapacity = capacityGenerators.Count;
        shieldAmount = activeGenerators.Count;

        return true;
    }

    /// <summary>
    /// Deals damage to a grid
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="damage"></param>
    /// <param name="component"></param>
    /// <returns>True if all hits were tanked, false if none/partial were performed.</returns>
    public bool TryDamageShieldHealth(EntityUid grid, int damage, ShipTrackerComponent? component = null)
    {
        if (!Resolve(grid, ref component))
            return false;

        var activeGenerators = EntityQuery<ShieldGeneratorComponent>()
            .Where(shield => Transform(shield.Owner).GridUid == grid).ToList();

        for (var i = 0; i < Math.Clamp(damage, 0, activeGenerators.Count); i++)
        {
            var generator = _random.PickAndTake(activeGenerators);
            var owner = generator.Owner;
            _appearanceSystem.SetData(owner, ShieldGeneratorVisuals.State, false);
            _sharedAudioSystem.PlayPvs(generator.DamageSound, owner);
            generator.Enabled = false;
        }

        return damage > activeGenerators.Count;
    }

    public bool TryRegenerateShieldHealth(EntityUid grid, int health, ShipTrackerComponent? component = null)
    {
        if (!Resolve(grid, ref component))
            return false;

        var activeGenerators = EntityQuery<ShieldGeneratorComponent>()
            .Where(shield => Transform(shield.Owner).GridUid == grid && !shield.Enabled).ToList();

        for (var i = 0; i < Math.Clamp(health, 0, activeGenerators.Count); i++)
        {
            var generator = _random.PickAndTake(activeGenerators);
            var owner = component.Owner;
            _appearanceSystem.SetData(owner, ShieldGeneratorVisuals.State, true);
            _sharedAudioSystem.PlayPvs(generator.RechargeSound, owner);
            generator.Enabled = true;
        }

        return true;
    }

    private void OnShieldsExamined(EntityUid uid, ShieldGeneratorComponent component, ExaminedEvent args)
    {
        args.Message.AddMarkup("\n" + (component.Enabled
            ? Loc.GetString("ship-shield-examine-active-message")
            : Loc.GetString("ship-shield-examine-inactive-message")));
    }
}
