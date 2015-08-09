using System;
using System.Windows.Controls;
using System.Windows.Markup;

using QuestEditor.ValueConverters;
using QuestEditor.ViewModels;

namespace QuestEditor.MarkupExtensions
{
    public abstract class EqualsExtension<T> : MarkupExtension
    {
        protected EqualsExtension(T other)
        {
            this.Other = other;
        }

        [ConstructorArgument("other")]
        public T Other { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider) => new EqualityValueConverter(this.Other);
    }

    public sealed class EqualsMouseModeExtension : EqualsExtension<MouseMode> { public EqualsMouseModeExtension(MouseMode other) : base(other) { } }
    public sealed class EqualsBooleanExtension : EqualsExtension<bool> { public EqualsBooleanExtension(bool other) : base(other) { } }

    public sealed class Negate : MarkupExtension
    {
        private static readonly NegationValueConverter ConverterInstance = new NegationValueConverter();

        public override object ProvideValue(IServiceProvider serviceProvider) => ConverterInstance;
    }

    public sealed class MouseModeToCursorExtension : MarkupExtension
    {
        private static readonly MouseModeToCursorValueConverter ConverterInstance = new MouseModeToCursorValueConverter();

        public override object ProvideValue(IServiceProvider serviceProvider) => ConverterInstance;
    }

    public sealed class BooleanToVisibilityConverterExtension : MarkupExtension
    {
        private static readonly BooleanToVisibilityConverter ConverterInstance = new BooleanToVisibilityConverter();

        public override object ProvideValue(IServiceProvider serviceProvider) => ConverterInstance;
    }

    public sealed class NullToVisibilityValueConverterExtension : MarkupExtension
    {
        private static readonly NullToVisibilityValueConverter ConverterInstance = new NullToVisibilityValueConverter();

        public override object ProvideValue(IServiceProvider serviceProvider) => ConverterInstance;
    }
}
