using System.Security.Cryptography;

namespace RSATools.RSAKeys
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
            var directory = Directory.GetCurrentDirectory();
            var path = directory.Substring(0, directory.IndexOf("Total_Attectation"));
            path = Path.Combine(path, "Total_Attectation\\RSATools\\RSAKeys");
            return path;
        }
    }
}
