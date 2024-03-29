using System.Security.Cryptography;

namespace User.RSAKeys
{
    public class RSATools
    {
        public static RSA GetPublicKey()
        {
            var publicKey = File.ReadAllText("RSAKeys/public_key.pem");

            var rsa = RSA.Create();
            rsa.ImportFromPem(publicKey);
            return rsa;
        }

        public static RSA GetPrivateKey()
        {
            var privateKey = File.ReadAllText("RSAKeys/private_key.pem");

            var rsa = RSA.Create();
            rsa.ImportFromPem(privateKey);
            return rsa;
        }
    }
}
