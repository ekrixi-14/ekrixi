using System.Linq;

namespace Content.Server._FTL.FTLPoints.Tick;

public abstract class StarmapTickSystem<T> : EntitySystem where T : Component
{
    private readonly float _tickInterval = 60f;
    private float _timeSinceLastTick = 0f; // SIN

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<T, ComponentInit>(Added);
    }

    /// <summary>
    /// Called on entities when added
    /// </summary>
    private void Added(EntityUid uid, T component, ComponentInit args)
    {

    }

    /// <summary>
    /// Called on entities every tick
    /// </summary>
    protected virtual void Ticked(EntityUid uid, T component, float frameTime)
    {

    }

    protected EntityQueryEnumerator<T> QueryStarsEnumerator()
    {
        return EntityQueryEnumerator<T>();
    }

    protected IEnumerable<T> QueryStars()
    {
        return EntityQuery<T>();
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        _timeSinceLastTick += frameTime;
        if (!(_timeSinceLastTick >= _tickInterval))
            return;
        _timeSinceLastTick = 0;

        var stars = QueryStars().ToList();
        // since stars can be modified it's not exactly ideal
        while (stars.Count > 0)
        {
#pragma warning disable CS0618
            Ticked(stars[0].Owner, stars[0], frameTime);
#pragma warning restore CS0618
            stars.RemoveAt(0);
        }

        Log.Debug("Tick!");
    }
}
