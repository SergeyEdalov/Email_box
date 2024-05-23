using System.Security.Cryptography;

namespace RSATools.RSAKeyFolder
{
    public static class RsaToolsKeys
    {
        public static RSA GetPublicKey()
        {
            var publicKey = File.ReadAllText(Path.Combine(GetPathKeys(), "public_key.pem"));

            var rsa = RSA.Create();
            rsa.ImportFromPem(publicKey);
            return rsa;
        }

        public static RSA GetPrivateKey()
        {
            var privateKey = File.ReadAllText(Path.Combine(GetPathKeys(), "private_key.pem"));

            var rsa = RSA.Create();
            rsa.ImportFromPem(privateKey);
            return rsa;
        }

        private static string GetPathKeys()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            var directoryInfo = new DirectoryInfo(baseDirectory);

            while (directoryInfo != null)
            {
                var rsaKeysDirectory = Path.Combine(directoryInfo.FullName, "RSATools", "RSAKeyFolder");

                if (Directory.Exists(rsaKeysDirectory))
                {
                    return rsaKeysDirectory;
                }
                directoryInfo = directoryInfo.Parent;
            }
            throw new DirectoryNotFoundException("Directory RSATools/RSAKeyFolder not found.");
        }
    }
}
