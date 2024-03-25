using Content.Shared.Tools;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._FTL.Wounds;

[DataDefinition, Serializable, NetSerializable]
public abstract partial class BaseTreatmentPath
{
    /// <summary>
    /// The quality required to start treatment.
    /// </summary>
    [ViewVariables, DataField("quality")]
    public ProtoId<ToolQualityPrototype> ToolQuality = "Prying";

    /// <summary>
    /// How long does it take to perform this (seconds)?
    /// </summary>
    [ViewVariables, DataField("length")]
    public float TreatmentLength = 3;

    /// <summary>
    /// The sound played when treatment is began
    /// </summary>
    [DataField("grabSound")]
    public SoundSpecifier TreatmentSound = new SoundPathSpecifier("/Audio/Effects/chop.ogg");

    [DataField("beginMessage"), ViewVariables]
    public LocId BeganMessage = "popup-wound-generic-began";

    [DataField("endMessage"), ViewVariables]
    public LocId EndedMessage = "popup-wound-generic-ended";

    public virtual bool TreatmentCheck(IEntityManager entMan, NetEntity activeHand)
    {
        return true;
    }

    public virtual void OnTreatmentEnd(EntityManager entityManager)
    {
        // noop
    }

    public virtual string GetVerbText(WoundComponent currentWound)
    {
        return "Treat current wound";
    }
}
