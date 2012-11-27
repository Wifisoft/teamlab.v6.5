using ASC.Notify.Messages;

namespace ASC.Notify.Patterns
{
    public interface IPatternFormatter
    {
        ITag[] GetTags(IPattern pattern);

        void FormatMessage(INoticeMessage message, ITagValue[] tagsValues);
    }
}