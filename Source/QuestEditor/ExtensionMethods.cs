using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace QuestEditor
{
    public static class ExtensionMethods
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ValidateNotNull<T>(this T value, string paramName) where T : class
        {
            ValidateNotNullCore(value, paramName);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum ValidateEnum<TEnum>(this TEnum value, string paramName, params TEnum[] validValues) where TEnum : struct
        {
            ValidateEnumCore(value, paramName, validValues);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ValidateMinAndMax<T>(this T value, string paramName, T min, T max) where T : struct, IComparable<T>
        {
            ValidateMinAndMaxCore(value, paramName, min, max);
            return value;
        }
        
        [Conditional("DEBUG")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ValidateNotNullCore<T>(T value, string paramName) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        [Conditional("DEBUG")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ValidateEnumCore<TEnum>(TEnum value, string paramName, TEnum[] validValues) where TEnum : struct
        {
            if (validValues == null || !validValues.Contains(value))
            {
                throw new ArgumentOutOfRangeException(paramName, value, "Unrecognized value.");
            }
        }

        [Conditional("DEBUG")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ValidateMinAndMaxCore<T>(T value, string paramName, T min, T max) where T : struct, IComparable<T>
        {
            if (value.CompareTo(min) < 0 || max.CompareTo(value) < 0)
            {
                throw new ArgumentOutOfRangeException(paramName, value, String.Format(CultureInfo.InvariantCulture, "Must be between {0} and {1}.", min, max));
            }
        }
    }
}
