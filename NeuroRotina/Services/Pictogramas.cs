namespace NeuroRotina.Services;

/// <summary>
/// Conjunto fixo de pictogramas usados quando o cuidador não escolhe uma foto.
/// Pictogramas simples e de alto contraste são mais fáceis de reconhecer do que
/// fotografias com muitos detalhes.
/// </summary>
public static class Pictogramas
{
    public static readonly string[] Atividades =
    {
        "🪥", "🚿", "🍽️", "🥣", "👕", "🎒", "🚌", "🏫", "📚", "✏️",
        "🧩", "🧸", "⚽", "🎨", "🎵", "📺", "🛏️", "💊", "🦷", "🧼",
        "🚻", "🐕", "🌳", "🚗", "🍎", "💧", "😴", "⭐"
    };

    public static readonly string[] Rotinas =
    {
        "🌅", "☀️", "🌇", "🌙", "🏫", "🏠", "🍽️", "🛁", "🚗", "🎉"
    };

    public static readonly string[] Cartoes =
    {
        "✅", "❌", "💧", "🍎", "🚻", "🤕", "😴", "🙋", "🧸", "🎵",
        "🔊", "🤗", "😊", "😢", "😠", "🥱", "❓", "💬", "🛑", "👍"
    };

    public static readonly string[] Avatares =
    {
        "🧒", "👦", "👧", "🐻", "🐰", "🦊", "🐨", "🦁", "🐼", "🐸"
    };

    public static readonly string[] Comportamentos =
    {
        "Tranquila", "Participativa", "Agitada", "Crise"
    };

    public static readonly string[] NiveisSuporte =
    {
        "Nível 1", "Nível 2", "Nível 3"
    };
}
