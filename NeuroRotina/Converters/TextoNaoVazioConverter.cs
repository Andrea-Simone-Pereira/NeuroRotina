using System.Globalization;

namespace NeuroRotina.Converters;

/// <summary>Verdadeiro quando o texto tem conteúdo, usado para esconder rótulos vazios.</summary>
public class TextoNaoVazioConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => !string.IsNullOrWhiteSpace(value as string);

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
