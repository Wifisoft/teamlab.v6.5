using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using ASC.Api.Interfaces;
using ASC.Core;
using ASC.Core.Tenants;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ASC.Specific
{
    [DataContract(Name = "date", Namespace = "")]
    [JsonConverter(typeof(ApiDateTimeConverter))]
    [TypeConverter(typeof(ApiDateTimeTypeConverter))]
    public class ApiDateTime : IComparable<ApiDateTime>, IApiDateTime, IComparable
    {
        private static readonly string[] Formats = new[]
                                                       {
                                                           "o",
                                                           "yyyy'-'MM'-'dd'T'HH'-'mm'-'ss'.'fffffffK", 
                                                           "yyyy'-'MM'-'dd'T'HH'-'mm'-'ss'.'fffK", 
                                                           "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK",
                                                           "yyyy'-'MM'-'dd'T'HH'-'mm'-'ssK",
                                                           "yyyy'-'MM'-'dd'T'HH':'mm':'ssK",
                                                           "yyyy'-'MM'-'dd"
                                                       };

        public ApiDateTime()
            : this(null)
        {
        }

        public ApiDateTime(DateTime? dateTime)
            : this(dateTime, null)
        {
        }

        public ApiDateTime(DateTime? dateTime, TimeZoneInfo timeZone)
        {
            if (dateTime.HasValue && dateTime.Value > DateTime.MinValue && dateTime.Value < DateTime.MaxValue)
            {
                SetDate(dateTime.Value, timeZone);
            }
            else
            {
                UtcTime = DateTime.MinValue;
                TimeZoneOffset = TimeSpan.Zero;
            }
        }

        public ApiDateTime(DateTime utcTime, TimeSpan offset)
        {
            UtcTime = new DateTime(utcTime.Ticks, DateTimeKind.Utc);
            TimeZoneOffset = offset;
        }

        public static ApiDateTime Parse(string data)
        {
            return Parse(data, null);
        }

        public static ApiDateTime Parse(string data, TimeZoneInfo tz)
        {
            if (string.IsNullOrEmpty(data)) throw new ArgumentNullException("data");

            if (data.Length < 7) throw new ArgumentException("invalid date time format");

            var offsetPart = data.Substring(data.Length - 6, 6);
            DateTime dateTime;
            if (DateTime.TryParseExact(data, Formats, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dateTime))
            {
                //Parse time   
                TimeSpan tzOffset = TimeSpan.Zero;
                if (offsetPart.Contains(":") && TimeSpan.TryParse(offsetPart.TrimStart('+'), out tzOffset))
                {
                    return new ApiDateTime(dateTime, tzOffset);
                }
                if (!data.EndsWith("Z", true, CultureInfo.InvariantCulture))
                {
                    if (tz == null)
                    {
                        tz = GetTimeZoneInfo();
                    }
                    tzOffset = tz.GetUtcOffset(dateTime);
                    dateTime = dateTime.Subtract(tzOffset);
                }
                return new ApiDateTime(dateTime, tzOffset);

            }
            throw new ArgumentException("invalid date time format: " + data);
        }


        private void SetDate(DateTime value, TimeZoneInfo timeZone)
        {
            TimeZoneOffset = TimeSpan.Zero;
            UtcTime = DateTime.MinValue;

            if (timeZone == null)
            {
                timeZone = GetTimeZoneInfo();
            }

            if (value.Kind == DateTimeKind.Local)
            {
                value = TimeZoneInfo.ConvertTimeToUtc(new DateTime(value.Ticks, DateTimeKind.Unspecified), timeZone);
            }
            if (value.Kind == DateTimeKind.Unspecified)
            {
                //Assume it's utc
                value = new DateTime(value.Ticks, DateTimeKind.Utc);
            }

            if (value.Kind == DateTimeKind.Utc)
            {
                UtcTime = value; //Set UTC time
                TimeZoneOffset = timeZone.GetUtcOffset(value);
            }

        }

        private static TimeZoneInfo GetTimeZoneInfo()
        {
            var timeZone = TimeZoneInfo.Local;
            try
            {
                timeZone = CoreContext.TenantManager.GetCurrentTenant().TimeZone;
            }
            catch (Exception)
            {
                //Tenant failed
            }
            return timeZone;
        }

        private string ToRoundTripString(DateTime date, TimeSpan offset)
        {

            var dateString = date.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffff", CultureInfo.InvariantCulture);
            var offsetString = offset.Ticks == 0 ? "Z" : string.Format("{0}{1,2:00}:{2,2:00}", offset.Ticks > 0 ? "+" : "", offset.Hours, offset.Minutes);
            return dateString + offsetString;
        }

        public static explicit operator ApiDateTime(DateTime d)
        {
            var date = new ApiDateTime(d);
            return date;
        }

        public static explicit operator ApiDateTime(DateTime? d)
        {
            if (d.HasValue)
            {
                var date = new ApiDateTime(d);
                return date;
            }
            return null;
        }


        public static bool operator >(ApiDateTime left, ApiDateTime right)
        {
            if (ReferenceEquals(left, right)) return false;
            if (left == null) return false;

            return left.CompareTo(right) > 0;
        }
        public static bool operator >=(ApiDateTime left, ApiDateTime right)
        {
            if (ReferenceEquals(left, right)) return false;
            if (left == null) return false;

            return left.CompareTo(right) >= 0;
        }

        public static bool operator <=(ApiDateTime left, ApiDateTime right)
        {
            return !(left >= right);
        }

        public static bool operator <(ApiDateTime left, ApiDateTime right)
        {
            return !(left > right);
        }

        public static bool operator ==(ApiDateTime left, ApiDateTime right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ApiDateTime left, ApiDateTime right)
        {
            return !(left == right);
        }

        public static implicit operator DateTime(ApiDateTime d)
        {
            if (d == null) return DateTime.MinValue;
            return d.UtcTime;
        }

        public static implicit operator DateTime?(ApiDateTime d)
        {
            if (d == null) return null;
            return d.UtcTime;
        }

        public int CompareTo(DateTime other)
        {
            return this.CompareTo(new ApiDateTime(other));
        }

        public int CompareTo(ApiDateTime other)
        {
            if (other == null) return 1;
            return UtcTime.CompareTo(other.UtcTime);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(ApiDateTime)) return false;
            return Equals((ApiDateTime)obj);
        }

        public bool Equals(ApiDateTime other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return UtcTime.Equals(other.UtcTime)&& TimeZoneOffset.Equals(other.TimeZoneOffset);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return UtcTime.GetHashCode() * 397 + TimeZoneOffset.GetHashCode();
            }
        }

        public int CompareTo(object obj)
        {
            if (obj is DateTime)
                return CompareTo((DateTime)obj);
            return obj is ApiDateTime ? CompareTo((ApiDateTime)obj) : 0;
        }

        public override string ToString()
        {
            return ToRoundTripString(UtcTime.Add(TimeZoneOffset), TimeZoneOffset);
        }

        public DateTime UtcTime { get; private set; }
        public TimeSpan TimeZoneOffset { get; private set; }

    }

    public class ApiDateTimeTypeConverter : DateTimeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
                return value.ToString();
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                return ApiDateTime.Parse((string)value);
            }
            if (value is DateTime)
            {
                return new ApiDateTime((DateTime)value);
            }
            return base.ConvertFrom(context, culture, value);
        }
    }

    public class ApiDateTimeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is IApiDateTime)
            {
                writer.WriteValue(value.ToString());
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IApiDateTime).IsAssignableFrom(objectType);
        }
    }
}