using Robust.Shared.Serialization;

namespace Content.Shared._FTL.Economy;

[Serializable, NetSerializable]
public sealed class IdAtmUiState : BoundUserInterfaceState
{
    public string IdName { get; }
    public bool IdCardIn { get; }
    public int Bank { get; }
    public int Cash { get; }

    public IdAtmUiState(string name, bool idCardIn, int bank, int cash)
    {
        IdName = name;
        IdCardIn = idCardIn;
        Bank = bank;
        Cash = cash;
    }
}
