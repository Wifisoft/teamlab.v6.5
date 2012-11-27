namespace ASC.CRM.Core
{

    public enum TaskSortedByType
    {
        Title,
        Category,
        DeadLine
    }

    public enum DealSortedByType
    {
        Title,
        Responsible,
        Stage,
        BidValue,
        DateAndTime
    }

    public enum RelationshipEventByType
    {
        Created,
        CreateBy,
        Category,
        Content
    }

    public enum ContactSortedByType
    {
        DisplayName,
        Created,
        ContactType
    }

    public enum SortedByType
    {
        DateAndTime,
        Title,
        CreateBy
    }
}
