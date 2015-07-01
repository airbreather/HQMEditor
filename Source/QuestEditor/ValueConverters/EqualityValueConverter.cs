using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

using static System.Convert;

namespace QuestEditor.ValueConverters
{
    [ValueConversion(typeof(object), typeof(bool))]
    public sealed class EqualityValueConverter : IValueConverter
    {
        public EqualityValueConverter()
        {
        }

        public EqualityValueConverter(object other)
        {
            this.Other = other;
        }

        public object Other { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => Equals(this.Other, value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                bool equal = ToBoolean(value, culture);
                if (equal)
                {
                    return this.Other;
                }
            }
            catch (InvalidCastException)
            {
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
