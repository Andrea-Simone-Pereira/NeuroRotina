using System.Globalization;

namespace NeuroRotina.Converters;

/// <summary>Inverte um valor booleano em bindings de visibilidade.</summary>
public class InverterBooleanoConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool valor && !valor;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool valor && !valor;
}
