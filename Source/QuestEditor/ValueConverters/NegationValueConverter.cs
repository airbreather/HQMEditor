using System.Globalization;

namespace QuestEditor.ValueConverters
{
    public sealed class NegationValueConverter : StronglyTypedValueConverterBase<bool, bool>
    {
        protected override bool TryConvertForward(bool fromValue, object parameter, CultureInfo culture, out bool toValue)
        {
            toValue = !fromValue;
            return true;
        }

        protected override bool TryConvertBackward(bool toValue, object parameter, CultureInfo culture, out bool fromValue)
        {
            fromValue = !toValue;
            return true;
        }
    }
}
