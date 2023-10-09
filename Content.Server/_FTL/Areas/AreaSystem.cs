using System.Linq;
using Content.Server.Power.Components;
using Content.Shared._FTL.Areas;

namespace Content.Server._FTL.Areas;

public sealed class AreaSystem : SharedAreasSystem
{
    [Dependency] private readonly IEntityManager _entityManager = default!;

    public List<Area> GetAreasOnGrid(EntityUid? gridUid)
    {
        var areas = new List<Area>();
        var query = EntityQueryEnumerator<ApcComponent>();

        while (query.MoveNext(out var entity, out var apc))
        {
            var xform = Transform(entity);
            if (xform.GridUid != gridUid)
                continue;
            var meta = MetaData(entity);
            var area = new Area
            {
                Entity = _entityManager.GetNetEntity(entity),
                Name = meta.EntityName,
                Enabled = apc.MainBreakerEnabled
            };
            areas.Add(area);
        }

        return areas;
    }
}
