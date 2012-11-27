using ASC.Notify.Messages;

namespace ASC.Notify.Patterns
{
    class NullPatternFormatter : IPatternFormatter
    {
        public ITag[] GetTags(IPattern pattern)
        {
            return new ITag[0];
        }

        public void FormatMessage(INoticeMessage message, ITagValue[] tagsValues)
        {
        }
    }
}