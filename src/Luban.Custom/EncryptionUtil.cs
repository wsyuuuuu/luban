using NLog;

namespace Luban.Custom;

public static class EncryptionUtil
{
    public static bool EncryptEnabled()
    {
        return EnvManager.Current.TryGetOption("encryption", "primaryKey", false, out string _) &&
               EnvManager.Current.TryGetOption("encryption", "secondaryKey", false, out string __);
    }

    public static byte[] Encrypt(string keySuffix, byte[] source)
    {
        if (EnvManager.Current.TryGetOption("encryption", "primaryKey", false, out string primaryKey) &&
            EnvManager.Current.TryGetOption("encryption", "secondaryKey", false, out string secondaryKey))
        {
            return XXTEA.Encrypt(source, XXTEA.Encrypt(secondaryKey + keySuffix, primaryKey));
        }

        LogManager.GetCurrentClassLogger().Info("no encryption key options, skip encryption");
        return source;
    }
}