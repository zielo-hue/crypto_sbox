using System;
using System.Security.Cryptography;
using System.IO;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text;
using System.Text.RegularExpressions;

namespace crypto2
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO: implement signing and verifying
            // TODO: use openSSL
            // TODO: sequence diagram, find difference between tls and openssl
            // TODO: export Pkcs8 public key
            // Pkcs1v15, OAEP, etc...?
            
            RSA rsa = new RSACryptoServiceProvider();
            var rootCommand = new RootCommand("A cryptography tool");
            var generateCommand = new Command("generate")
            {
                new Option<string>(
                    "--publicKey",
                    "Specify the private key to use for the public key"),
                new Option<int>(
                    "--keySize",
                    () => 2048,
                    "Specify the key size. Defaults to 2048"),
            };
            var encryptCommand = new Command("encrypt")
            {
                new Option<string>(
                    "--filePath",
                    "Path to the file to encrypt."),
                new Option<string>(
                    "--keyPath",
                    "Path to the public key to use in encryption."),
                new Option<string>(
                    "--padding",
                    "Choose ")
            };
            var decryptCommand = new Command("decrypt")
            {
                new Option<string>(
                    "--filePath",
                    "Path to the file to decrypt."),
                new Option<string>(
                    "--keyPath",
                    "Path to the private key to use in encryption.")
            };
            var signCommand = new Command("sign")
            {
                new Option<string>(
                    "--filePath",
                    "Path to the file to sign."),
                new Option<string>(
                    "--keyPath",
                    "Path to the private key to sign with")
            };
            var verifyCommand = new Command("verify")
            {
                new Option<string>(
                    "--filePath",
                    "Path to the file."),
                new Option<string>(
                    "--signPath",
                    "Path to the file's sign"),
                new Option<string>(
                    "--keyPath",
                    "Path to the public key to verify")
            };
            rootCommand.Add(generateCommand);
            rootCommand.Add(encryptCommand);
            rootCommand.Add(decryptCommand);
            rootCommand.Add(signCommand);

            generateCommand.Handler = CommandHandler
                .Create<string, int>((publicKey, keySize) =>
                {
                    if (publicKey is null)
                    {
                        var rsaGen = RSA.Create(keySize);
                        File.WriteAllText("privkey", PemPrivateKey(rsaGen.ExportPkcs8PrivateKey()));
                        Console.WriteLine($"exported privkey with size {keySize}");
                    }
                    else
                    {
                        rsa.ImportPkcs8PrivateKey(PemToDer(File.ReadAllLines(publicKey)), out _);
                        // File.WriteAllBytes("pubkey.pub", rsa.ExportRSAPublicKey());
                        File.WriteAllText("pubkey.pem", 
                            PemPublicCertificate(rsa.ExportRSAPublicKey()));
                        Console.WriteLine($"used {publicKey} to generate public key pubkey.pem");
                    }
                });
            
            encryptCommand.Handler = CommandHandler
                .Create<string, string>((filePath, keyPath) =>
                {
                    var inputFile = File.ReadAllBytes(filePath);
                    rsa.ImportPkcs8PrivateKey(PemToDer(File.ReadAllLines(keyPath)), out _);
                    var encryptedFile = rsa.Encrypt(inputFile, RSAEncryptionPadding.Pkcs1);
                    File.WriteAllBytes(filePath + ".enc", encryptedFile);
                    Console.WriteLine($"encrypted and exported file. {encryptedFile.Length} bytes written");
                });

            decryptCommand.Handler = CommandHandler
                .Create<string, string>((filePath, keyPath) =>
                {
                    var inputFile = File.ReadAllBytes(filePath);
                    rsa.ImportPkcs8PrivateKey(PemToDer(File.ReadAllLines(keyPath)), out _);
                    File.WriteAllBytes(filePath + ".dec", rsa.Decrypt(inputFile, RSAEncryptionPadding.Pkcs1));
                    Console.WriteLine("decrypted and exported file");
                });
            
            signCommand.Handler = CommandHandler
                .Create<string, string>((filePath, keyPath) =>
                {
                    var inputFile = File.ReadAllBytes(filePath);
                    rsa.ImportPkcs8PrivateKey(PemToDer(File.ReadAllLines(keyPath)), out _);
                    var signedData = rsa.SignData(inputFile, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                    File.WriteAllBytes(filePath + ".sig", signedData);
                });

            verifyCommand.Handler = CommandHandler
                .Create<string, string, string>((filePath, signPath, keyPath) =>
                {
                    var inputFile = File.ReadAllBytes(filePath);
                    var signatureData = File.ReadAllBytes(signPath);
                    string message = "not verified";
                    rsa.ImportFromPem(keyPath);
                    if (rsa.VerifyData(inputFile, signatureData, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1)
                    message = "verified"
                    Console.WriteLine($"File {filePath} is {message}");
                });

            rootCommand.InvokeAsync(args).Wait();
        }

        private static byte[] PemToDer(string[] pem)
            => Convert.FromBase64String(String.Join("", pem[1..^1]));

        private static string PemPublicCertificate(byte[] keyContents)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("-----BEGIN PUBLIC KEY-----\n");
            builder.Append(Regex.Replace(Convert.ToBase64String(keyContents), ".{68}", "$0\n"));
            builder.Append("\n-----END PUBLIC KEY-----");
            return builder.ToString();
        }
        
        private static string PemPrivateKey(byte[] keyContents)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("-----BEGIN RSA PRIVATE KEY-----\n");
            builder.AppendLine(Regex.Replace(Convert.ToBase64String(keyContents), ".{68}", "$0\n"));
            builder.AppendLine("\n-----END RSA PRIVATE KEY-----");
            return builder.ToString();
        }
    }
}