using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ASC.Core.Common.Caching
{
    class AzRecordStore : IEnumerable<AzRecord>
    {
        private readonly Dictionary<string, List<AzRecord>> byObjectId = new Dictionary<string, List<AzRecord>>();


        public AzRecordStore(IEnumerable<AzRecord> aces)
        {
            foreach (var a in aces)
            {
                Add(a);
            }
        }


        public IEnumerable<AzRecord> Get(string objectId)
        {
            List<AzRecord> aces;
            byObjectId.TryGetValue(objectId ?? string.Empty, out aces);
            return aces ?? new List<AzRecord>();
        }

        public void Add(AzRecord r)
        {
            if (r == null) return;

            var id = r.ObjectId ?? string.Empty;
            if (!byObjectId.ContainsKey(id))
            {
                byObjectId[id] = new List<AzRecord>();
            }
            byObjectId[id].RemoveAll(a => a.SubjectId == r.SubjectId && a.ActionId == r.ActionId); // remove escape, see DbAzService
            byObjectId[id].Add(r);
        }

        public void Remove(AzRecord r)
        {
            if (r == null) return;

            var id = r.ObjectId ?? string.Empty;
            if (byObjectId.ContainsKey(id))
            {
                byObjectId[id].RemoveAll(a => a.SubjectId == r.SubjectId && a.ActionId == r.ActionId && a.Reaction == r.Reaction);
            }
        }

        public IEnumerator<AzRecord> GetEnumerator()
        {
            return byObjectId.Values.SelectMany(v => v).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}