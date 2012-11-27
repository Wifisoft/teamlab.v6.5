using System;
using ASC.Common.Utils;
using ASC.Notify.Messages;
using NVelocity;
using NVelocity.App.Events;

namespace ASC.Notify.Patterns
{
    public sealed class NVelocityPatternFormatter : PatternFormatter
    {
        public const string DefaultPattern = @"(^|[^\\])\$[\{]{0,1}(?<tagName>[a-zA-Z0-9_]+)";
        private VelocityContext _nvelocityContext;

        public NVelocityPatternFormatter()
            : base(DefaultPattern)
        {
        }

        protected override void BeforeFormat(INoticeMessage message, ITagValue[] tagsValues)
        {
            _nvelocityContext = new VelocityContext();
            _nvelocityContext.AttachEventCartridge(new EventCartridge());
            _nvelocityContext.EventCartridge.ReferenceInsertion += EventCartridgeReferenceInsertion;
            foreach (ITagValue tagValue in tagsValues)
            {
                _nvelocityContext.Put(tagValue.Tag.Name, tagValue.Value);
            }
            base.BeforeFormat(message, tagsValues);
        }

        protected override string FormatText(string text, ITagValue[] tagsValues)
        {
            if (String.IsNullOrEmpty(text)) return text;
            return VelocityFormatter.FormatText(text, _nvelocityContext);
        }

        protected override void AfterFormat(INoticeMessage message)
        {
            _nvelocityContext = null;
            base.AfterFormat(message);
        }

        private static void EventCartridgeReferenceInsertion(object sender, ReferenceInsertionEventArgs e)
        {
            var originalString = e.OriginalValue as string;
            if (originalString == null) return;
            var lines = originalString.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length == 0) return;
            e.NewValue = string.Empty;
            for (var i = 0; i < lines.Length - 1; i++)
            {
                e.NewValue += string.Format("nostyle{0}/nostyle\n", lines[i]);
            }
            e.NewValue += string.Format("nostyle{0}/nostyle", lines[lines.Length - 1]);
        }
    }
}