using System.Numerics;
using Content.Shared.Roles;
using Robust.Shared.Audio;
using Robust.Shared.Containers;

namespace Content.Server._FTL.SleeperCryopod;
[RegisterComponent]
public sealed class SleeperCryopodComponent : Component
{
    public ContainerSlot BodyContainer = default!;

    /// <summary>
    /// Whether or not spawns are routed through the cryopod.
    /// </summary>
    [DataField("doSpawns")] public bool DoSpawns = true;

    /// <summary>
    /// The sound that is played when a player spawns in the pod.
    /// </summary>
    [DataField("arrivalSound")] public SoundSpecifier ArrivalSound = new SoundPathSpecifier("/Audio/Effects/teleport_arrival.ogg");

    /// <summary>
    /// The sound that is played when a player leaves the game via a pod.
    /// </summary>
    [DataField("leaveSound")] public SoundSpecifier LeaveSound = new SoundPathSpecifier("/Audio/Effects/radpulse3.ogg");

    /// <summary>
    /// The maximum limit to being SSD/braindead before they are removed from the round.
    /// </summary>
    [DataField("ssdMaxTimer")] public float BraindeadMaxTimer = 60f;

    /// <summary>
    /// How much time since a braindead person was put inside?
    /// </summary>
    public float TimeSinceBraindeath = 0f;

    /// <summary>
    /// The job of the person inside of cryosleep.
    /// </summary>
    public JobPrototype? CryosleptJob;

    /// <summary>
    /// How long the entity initially is asleep for upon joining.
    /// </summary>
    [DataField("initialSleepDurationRange")]
    public Vector2 InitialSleepDurationRange = new (5, 10);
}
