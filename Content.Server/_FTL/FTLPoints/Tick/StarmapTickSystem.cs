namespace Content.Server._FTL.FTLPoints.Tick;

public abstract class StarmapTickSystem<T> : EntitySystem where T : Component
{
    private readonly float _tickInterval = 300f;
    private float _timeSinceLastTick = 300f; // SIN

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

    protected EntityQueryEnumerator<T> QueryStars()
    {
        return EntityQueryEnumerator<T>();
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        _timeSinceLastTick += frameTime;
        if (!(_timeSinceLastTick >= _tickInterval))
            return;
        _timeSinceLastTick = 0;
        var stars = QueryStars();
        while (stars.MoveNext(out var uid, out var component))
        {
            Ticked(uid, component, frameTime);
        }
        Log.Info("Tick!");
    }
}
