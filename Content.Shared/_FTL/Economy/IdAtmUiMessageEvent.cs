using Robust.Shared.Serialization;

namespace Content.Shared._FTL.Economy;

[Serializable, NetSerializable]
public sealed class IdAtmUiMessageEvent : BoundUserInterfaceMessage
{
    public readonly IdAtmUiAction Action;
    public readonly int Amount;
    public readonly EntityUid Entity;

    public IdAtmUiMessageEvent(EntityUid entity, IdAtmUiAction action, int amount)
    {
        Entity = entity;
        Action = action;
        Amount = amount;
    }
}

[Serializable, NetSerializable]
public enum IdAtmUiAction
{
    Withdrawal,
    Deposit
}

[NetSerializable, Serializable]
public enum IdAtmUiKey : byte
{
    Key,
}
