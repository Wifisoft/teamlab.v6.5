using System;
using System.Linq;
using System.Runtime.Serialization;

namespace ASC.Feed.Utils
{
    [Serializable]
    public struct DateTimeRange :
    IFormattable, ISerializable, IEquatable<DateTimeRange>
    {
        private const string FromName = "from";
        private const string ToName = "to";

        public DateTime From { get; set; }
        public DateTime To { get; set; }


        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null) throw new ArgumentNullException("format");
            return string.Format("{0}-{1}", From.ToString(format, formatProvider), To.ToString(format, formatProvider));
        }

        public string ToString(string format, ICustomFormatter formatter, IFormatProvider formatProvider)
        {
            if (format == null) throw new ArgumentNullException("format");
            if (formatter == null) throw new ArgumentNullException("formatter");
            if (formatProvider == null) throw new ArgumentNullException("formatProvider");
            return string.Format("{0}-{1}", formatter.Format(format, From, formatProvider), formatter.Format(format, To, formatProvider));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null) throw new ArgumentNullException("info");
            info.AddValue(FromName, From, typeof(DateTime));
            info.AddValue(ToName, To, typeof(DateTime));
        }

        public DateTimeRange(DateTime from, DateTime to)
            : this()
        {
            From = @from;
            To = to;
        }

        public DateTimeRange(DateTime from, TimeSpan offset)
            : this()
        {
            From = @from;
            To = @from + offset;
        }

        private DateTimeRange(SerializationInfo info, StreamingContext context)
            : this()
        {
            if (info == null)
                throw new ArgumentNullException("info");
            foreach (var serializationInfo in info)
            {
                if (serializationInfo.ObjectType == typeof(DateTime))
                {
                    if (FromName.Equals(serializationInfo.Name))
                    {
                        From = (DateTime)serializationInfo.Value;
                    }
                    if (ToName.Equals(serializationInfo.Name))
                    {
                        To = (DateTime)serializationInfo.Value;
                    }
                }
            }
        }

        public override bool Equals(object obj)
        {
            return obj is DateTimeRange && Equals((DateTimeRange)obj);
        }

        public bool Equals(DateTimeRange other)
        {
            return other.From.Equals(From) && other.To.Equals(To);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (From.GetHashCode() * 397) ^ To.GetHashCode();
            }
        }

        public static DateTimeRange operator +(DateTimeRange range, TimeSpan offset)
        {
            return range.Add(offset);
        }

        public static DateTimeRange operator -(DateTimeRange range, TimeSpan offset)
        {
            return range.Substract(offset);
        }

        public bool In(DateTime dateTime)
        {
            return From <= dateTime && To >= dateTime;
        }

        public DateTimeRange Add(TimeSpan offset)
        {
            return new DateTimeRange(From + offset, To + offset);
        }

        public DateTimeRange Substract(TimeSpan offset)
        {
            return new DateTimeRange(From - offset, To - offset);
        }

        public static bool operator ==(DateTimeRange left, DateTimeRange right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DateTimeRange left, DateTimeRange right)
        {
            return !Equals(left, right);
        }
    }

}