using Robust.Shared.Audio;

namespace Content.Server._FTL.Psion;

[RegisterComponent, Access(typeof(PsionRuleSystem))]
public sealed class PsionRuleComponent : Component
{
    [DataField("rolePrototype")] public string RolePrototype = "Psion";
    [DataField("maxPicks")] public int ObjectiveMaxPicks = 3;

    /// <summary>
    ///     Path to antagonist alert sound.
    /// </summary>
    [DataField("greetSoundNotification")] public SoundSpecifier GreetSoundNotification = new SoundPathSpecifier("/Audio/_FTL/Ambience/Antag/psion_start.ogg");
}
