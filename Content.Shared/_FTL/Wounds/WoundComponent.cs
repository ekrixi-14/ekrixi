using Content.Shared.Damage;
using Content.Shared.Tools;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._FTL.Wounds;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Serializable]
public sealed partial class WoundComponent : Component
{
    /// <summary>
    /// How much damage does this wound do permanently?
    /// </summary>
    [DataField, ViewVariables] public DamageSpecifier? Damage;

    /// <summary>
    /// How severe is this wound?
    /// </summary>
    [DataField, ViewVariables] public float Severity = 1f;

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
    public List<BaseTreatmentPath> TreatmentPaths = new();
}
