using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace VD1
{
    //~ https://stackoverflow.com/a/55689151

    class RSAFileHelper
    {
        RSACryptoServiceProvider csp = new RSACryptoServiceProvider(2048);

        public string GetPubKey()
        {
            //public key ...
            RSAParameters pubKey = csp.ExportParameters(false);
            //converting the public key into a string representation

            string pubKeyString;
            {
                //we need some buffer
                var sw = new StringWriter();
                //we need a serializer
                var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                //serialize the key into the stream
                xs.Serialize(sw, pubKey);
                //get the string from the stream
                pubKeyString = sw.ToString();
            }
            return pubKeyString;
        }
        public string GetPrivKey()
        {
            //private key
            RSAParameters privKey = csp.ExportParameters(true);

            string privKeyString;
            {
                //we need some buffer
                var sw = new StringWriter();
                //we need a serializer
                var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                //serialize the key into the stream
                xs.Serialize(sw, privKey);
                //get the string from the stream
                privKeyString = sw.ToString();
            }
            return privKeyString;
        }
        public string EncryptString(string input, string PubKey)
        {
            //get a stream from the string
            var sr = new StringReader(PubKey);

            //we need a deserializer
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));

            //get the object back from the stream
            RSACryptoServiceProvider csp = new RSACryptoServiceProvider();
            csp.ImportParameters((RSAParameters)xs.Deserialize(sr));
            byte[] bytesPlainTextData = Encoding.ASCII.GetBytes(input);

            //apply pkcs#1.5 padding and encrypt our data 
            var bytesCipherText = csp.Encrypt(bytesPlainTextData, false);
            //we might want a string representation of our cypher text... base64 will do
            string encryptedText = Convert.ToBase64String(bytesCipherText);
            return encryptedText;
        }
        public string DecryptString(string input, string PrivKey)
        {
            //we want to decrypt, therefore we need a csp and load our private key
            RSACryptoServiceProvider csp = new RSACryptoServiceProvider();

            string privKeyString;
            {
                //get a stream from the string
                var sr = new StringReader(PrivKey);
                //we need a deserializer
                var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                //get the object back from the stream
                RSAParameters privKey = (RSAParameters)xs.Deserialize(sr);
                csp.ImportParameters(privKey);
            }
            byte[] bytesCipherText = Convert.FromBase64String(input);

            //decrypt and strip pkcs#1.5 padding
            byte[] bytesPlainTextData = csp.Decrypt(bytesCipherText, false);

            string plaintext = Encoding.Default.GetString(bytesPlainTextData);

            return plaintext;
        }
    }
}
