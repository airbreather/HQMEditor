using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace QuestEditor.ValueConverters
{
    [ValueConversion(typeof(object), typeof(Visibility))]
    public sealed class NullToVisibilityValueConverter : StronglyTypedValueConverterBase<object, Visibility>
    {
        protected override bool TryConvertForward(object fromValue, object parameter, CultureInfo culture, out Visibility toValue)
        {
            toValue = fromValue == null
                ? Visibility.Collapsed
                : Visibility.Visible;
            return true;
        }

        protected override bool TryConvertBackward(Visibility toValue, object parameter, CultureInfo culture, out object fromValue)
        {
            fromValue = null;
            return toValue == Visibility.Collapsed;
        }
    }
}
