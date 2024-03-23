using Content.Shared.Damage;
using Robust.Shared.Random;

namespace Content.Server._FTL.ContantDamage;

/// <summary>
/// This handles...
/// </summary>
public sealed class ConstantDamageSystem : EntitySystem
{
    [Dependency] private readonly DamageableSystem _damageableSystem = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<ConstantDamageComponent>();
        while (query.MoveNext(out var entity, out var component))
        {
            if (component.TimeSinceLastCheck < component.CheckFrequency)
            {
                component.TimeSinceLastCheck += frameTime;
            }
            else
            {
                component.TimeSinceLastCheck = 0;
                if (_random.Prob(component.Probability))
                {
                    _damageableSystem.TryChangeDamage(entity, component.Damage);
                }
            }
        }
    }
}
