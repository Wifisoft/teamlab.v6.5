using ASC.Notify.Messages;

namespace ASC.Common.Notify.Patterns
{
    public interface IPatternStyler
    {
        void ApplyFormating(NoticeMessage message);
    }
}