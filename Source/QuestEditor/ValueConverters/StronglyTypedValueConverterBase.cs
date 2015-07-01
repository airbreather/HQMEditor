using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

using static System.Convert;

namespace QuestEditor.ValueConverters
{
    public abstract class StronglyTypedValueConverterBase<TConvertFrom, TConvertTo> : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TConvertFrom fromValue;
            try
            {
                fromValue = Get<TConvertFrom>(value, culture);
            }
            catch (InvalidCastException)
            {
                return DependencyProperty.UnsetValue;
            }

            TConvertTo toValue;
            return TryConvertForward(fromValue, parameter, culture, out toValue)
                ? toValue
                : DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TConvertTo toValue;
            try
            {
                toValue = Get<TConvertTo>(value, culture);
            }
            catch (InvalidCastException)
            {
                return DependencyProperty.UnsetValue;
            }

            TConvertFrom fromValue;
            return TryConvertBackward(toValue, parameter, culture, out fromValue)
                ? fromValue
                : DependencyProperty.UnsetValue;
        }

        protected static T Get<T>(object boxedValue, CultureInfo culture)
        {
            Type underlyingNullableType = Nullable.GetUnderlyingType(typeof(T));
            if (underlyingNullableType == null)
            {
                // Not a nullable value-type.
                return (T)ChangeType(boxedValue, typeof(T));
            }

            // It's a nullable value-type -- if null, return default.
            if (boxedValue == null)
            {
                return default(T);
            }

            // Non-null, so just cast the non-null to nullable.
            return (T)ChangeType(boxedValue, underlyingNullableType);
        }

        protected abstract bool TryConvertForward(TConvertFrom fromValue, object parameter, CultureInfo culture, out TConvertTo toValue);

        protected virtual bool TryConvertBackward(TConvertTo toValue, object parameter, CultureInfo culture, out TConvertFrom fromValue)
        {
            fromValue = default(TConvertFrom);
            return false;
        }
    }
}
