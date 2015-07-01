using System;
using System.Windows.Markup;

using QuestEditor.ValueConverters;
using QuestEditor.ViewModels;

namespace QuestEditor.MarkupExtensions
{
    public sealed class EqualsMouseModeExtension : MarkupExtension
    {
        public EqualsMouseModeExtension(MouseMode other)
        {
            this.Other = other;
        }

        [ConstructorArgument("other")]
        public MouseMode Other { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider) => new EqualityValueConverter(this.Other);
    }

    public sealed class MouseModeToCursorExtension : MarkupExtension
    {
        private static readonly MouseModeToCursorValueConverter ConverterInstance = new MouseModeToCursorValueConverter();

        public override object ProvideValue(IServiceProvider serviceProvider) => ConverterInstance;
    }
}
