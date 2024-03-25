using Content.Shared.Damage;
using Content.Shared.Tools;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._FTL.Wounds;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class WoundComponent : Component
{
    /// <summary>
    /// How much damage does this wound do permanently?
    /// </summary>
    [DataField, ViewVariables] public DamageSpecifier? Damage;
    /// <summary>
    /// What does it say when this wound is examined?
    /// </summary>
    [DataField, ViewVariables] public LocId WoundExamineMessage;

    /// <summary>
    /// What current treatment path are we on?
    /// </summary>
    [DataField, ViewVariables, AutoNetworkedField]
    public int CurrentTreatmentPath;

    /// <summary>
    /// A list of treatment paths.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField("paths", required: true)]
    public List<TreatmentPath> TreatmentPaths = new();
}

[DataDefinition, Serializable, NetSerializable]
public partial record struct TreatmentPath
{
    /// <summary>
    /// The quality required to start treatment.
    /// </summary>
    [ViewVariables, DataField("quality")]
    public ProtoId<ToolQualityPrototype> ToolQuality = "Prying";

    /// <summary>
    /// How long does it take to perform this?
    /// </summary>
    [ViewVariables, DataField("length")]
    public TimeSpan TreatmentLength = TimeSpan.FromSeconds(3);

    [DataField("beginMessage"), ViewVariables]
    public LocId BeganMessage = "popup-wound-generic-began";

    [DataField("endMessage"), ViewVariables]
    public LocId EndedMessage = "popup-wound-generic-ended";

    public TreatmentPath(ToolQualityPrototype tool, TimeSpan length, LocId beginMessage, LocId endMessage)
    {
        ToolQuality = tool.ID;
        TreatmentLength = length;
        BeganMessage = beginMessage;
        EndedMessage = endMessage;
    }
}
