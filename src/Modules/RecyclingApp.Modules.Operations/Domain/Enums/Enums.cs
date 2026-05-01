namespace RecyclingApp.Modules.Operations.Domain.Enums;

public enum TransactionType
{
    Purchase = 1,
    Sale = 2
}

public enum TransactionMode
{
    Vehicle = 1,
    Manual = 2
}

public enum TransactionStatus
{
    Created = 1,
    EntryWeighed = 2,
    InternalWeighingInProgress = 3,
    ExitWeighed = 4,
    PendingValidation = 5,
    Validated = 6,
    Completed = 7,
    Cancelled = 8
}

public enum WeightDirection
{
    Entry = 1,
    Exit = 2
}

public enum WeightSourceType
{
    Manual = 1,
    Device = 2
}

public enum CheckResult
{
    Pass = 1,
    Fail = 2,
    NeedsApproval = 3
}
