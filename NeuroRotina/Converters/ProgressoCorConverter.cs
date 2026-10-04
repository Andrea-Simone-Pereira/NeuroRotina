using System.Globalization;

namespace NeuroRotina.Converters;

/// <summary>
/// Pinta o bloco de progresso de verde quando o passo já foi concluído.
/// </summary>
public class ProgressoCorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var concluida = value is bool valor && valor;
        var chave = concluida ? "Sucesso" : "Borda";

        return Application.Current?.Resources[chave] as Color ?? Colors.Gray;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
