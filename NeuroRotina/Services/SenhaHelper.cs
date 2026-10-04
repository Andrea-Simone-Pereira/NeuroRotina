using System.Security.Cryptography;
using System.Text;

namespace NeuroRotina.Services;

/// <summary>
/// Converte a senha digitada em hash SHA-256 para que a senha original
/// nunca fique gravada no banco de dados.
/// </summary>
public static class SenhaHelper
{
    public static string Hash(string senha)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(senha));
        return Convert.ToHexString(bytes);
    }

    public static bool Conferir(string senhaDigitada, string hashArmazenado) =>
        Hash(senhaDigitada).Equals(hashArmazenado, StringComparison.OrdinalIgnoreCase);
}
