using System;

namespace Subscription_Manager
{
    public enum SortMode
    {
        Name,
        Cost,
        DaysUntilBilling
    }

    public static class SortModeExtensions
    {
        public static Array GetValues => Enum.GetValues(typeof(SortMode));
    }
}