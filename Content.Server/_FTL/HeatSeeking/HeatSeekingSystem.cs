using System.Linq;
using System.Numerics;
using Content.Shared.Physics;
using Robust.Server.GameObjects;
using Robust.Shared.Physics;
using Robust.Shared.Random;

namespace Content.Server._FTL.HeatSeeking;

/// <summary>
/// This handles...
/// </summary>
public sealed class HeatSeekingSystem : EntitySystem
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly TransformSystem _transform = default!;
    [Dependency] private readonly PhysicsSystem _physics = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<HeatSeekingComponent, TransformComponent>();
        while (query.MoveNext(out var uid, out var comp, out var xform))
        {
            if (comp.TargetEntity.HasValue)
            {
                var entXform = Transform(comp.TargetEntity.Value);
                var angle = (
                    xform.Coordinates.ToMapPos(_entityManager, _transform) -
                    entXform.Coordinates.ToMapPos(_entityManager, _transform)
                ).ToWorldAngle();

                _transform.SetLocalRotationNoLerp(uid, angle, xform);
                _physics.ApplyForce(uid, xform.LocalRotation.RotateVec(new Vector2(0, 1)) * comp.Speed);
                return;
            }
            var ray = new CollisionRay(_transform.GetMapCoordinates(uid, xform).Position, xform.LocalRotation.ToWorldVec(),
                (int) (CollisionGroup.Impassable | CollisionGroup.BulletImpassable));
            var results = _physics.IntersectRay(xform.MapID, ray, comp.DefaultSeekingRange, uid).ToList();
            if (results.Count <= 0)
                return; // nothing to heatseek ykwim

            comp.TargetEntity = results[0].HitEntity;
        }
    }
}
