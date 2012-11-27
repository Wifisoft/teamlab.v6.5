using System;

namespace ASC.Core.Tenants
{
    public class TenantUtil
    {
        public static DateTime DateTimeFromUtc(DateTime dbDateTime)
        {
            return DateTimeFromUtc(CoreContext.TenantManager.GetCurrentTenant(), dbDateTime);
        }

        public static DateTime DateTimeFromUtc(Tenant tenant, DateTime dbDateTime)
        {
            return DateTimeFromUtc(tenant.TimeZone, dbDateTime);
        }

        public static DateTime DateTimeFromUtc(TimeZoneInfo timeZone, DateTime dbDateTime)
        {
            return DateTime.SpecifyKind(TimeZoneInfo.ConvertTimeFromUtc(dbDateTime, timeZone), DateTimeKind.Local);
        }

        public static DateTime DateTimeToUtc(DateTime dbDateTime)
        {
            return DateTimeToUtc(CoreContext.TenantManager.GetCurrentTenant(), dbDateTime);
        }

        public static DateTime DateTimeToUtc(Tenant tenant, DateTime dbDateTime)
        {
            return DateTimeToUtc(tenant.TimeZone, dbDateTime);
        }

        public static DateTime DateTimeToUtc(TimeZoneInfo timeZone, DateTime dbDateTime)
        {
            if (dbDateTime.Kind == DateTimeKind.Utc)
                return dbDateTime;

            var newDate = DateTime.SpecifyKind(dbDateTime, DateTimeKind.Unspecified);

            return TimeZoneInfo.ConvertTimeToUtc(newDate, timeZone);

        }

        public static DateTime DateTimeNow()
        {
            return DateTimeNow(CoreContext.TenantManager.GetCurrentTenant());
        }

        public static DateTime DateTimeNow(Tenant tenant)
        {
            return DateTimeNow(tenant.TimeZone);
        }

        public static DateTime DateTimeNow(TimeZoneInfo timeZone)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
        }
    }
}