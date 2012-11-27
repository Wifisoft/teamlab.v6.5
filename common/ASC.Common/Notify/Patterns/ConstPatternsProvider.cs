using System;
using System.Collections.Generic;

namespace ASC.Notify.Patterns
{
    public sealed class ConstPatternProvider : IPatternProvider
    {
        private readonly Dictionary<string, IPatternFormatter> formatters = new Dictionary<string, IPatternFormatter>();

        internal readonly Dictionary<string, IPattern> patterns = new Dictionary<string, IPattern>();

        public ConstPatternProvider(params KeyValuePair<IPattern, IPatternFormatter>[] patterns)
        {
            foreach (var kvp in patterns)
            {
                try
                {
                    this.patterns.Add(kvp.Key.ID, kvp.Key);
                    formatters.Add(kvp.Key.ID, kvp.Value);
                }
                catch (Exception exc)
                {
                    throw new ArgumentException("patterns", exc);
                }
            }
        }

        public IPattern GetPattern(string id)
        {
            IPattern pattern = null;
            patterns.TryGetValue(id, out pattern);
            return pattern;
        }

        public IPattern[] GetPatterns()
        {
            return new List<IPattern>(patterns.Values).ToArray();
        }

        public IPatternFormatter GetFormatter(IPattern pattern)
        {
            if (pattern == null) throw new ArgumentNullException("pattern");
            IPatternFormatter formatter = null;
            formatters.TryGetValue(pattern.ID, out formatter);
            return formatter;
        }
    }
}