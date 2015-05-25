using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;

using QuestEditor.ViewModels;

namespace QuestEditor.ValueConverters
{
    [ValueConversion(typeof(MouseMode), typeof(Cursor))]
    public sealed class MouseModeToCursorValueConverter : StronglyTypedValueConverterBase<MouseMode, Cursor>
    {
        protected override bool TryConvertForward(MouseMode fromValue, object parameter, CultureInfo culture, out Cursor toValue)
        {
            switch (fromValue)
            {
                case MouseMode.EditQuests:
                    toValue = Cursors.Arrow;
                    return true;

                case MouseMode.CreateLink:
                    toValue = Cursors.Pen;
                    return true;

                case MouseMode.DeleteLink:
                    toValue = Cursors.Cross;
                    return true;
            }

            toValue = default(Cursor);
            return false;
        }
    }
}
