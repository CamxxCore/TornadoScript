using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace AirSuperiority.Core.IO
{
    public class EncryptedFileStream
    {
        private readonly FileStream stream;

        private readonly string DataHash = "dkfcn7tz";
        private readonly string Salt = "Delta0xa44";
        private readonly string VIKey = "@pQsQDF6vpfJA84A";

        public EncryptedFileStream(string filePath)
        {
            stream = new FileStream(filePath, FileMode.OpenOrCreate);
        }

        public async void WriteValueAsync(string key, int value)
        {
            string str = Encrypt(string.Format("{0}-{1}", key, value));

            try
            {
                int seekPos = 0;

                byte[] buffer = new byte[24];

                while (seekPos < stream.Length)
                {
                    stream.Seek(seekPos, SeekOrigin.Begin);

                    await stream.ReadAsync(buffer, 0, 24);

                    var line = Decipher(Encoding.ASCII.GetString(buffer));

                    var keyVal = line.Substring(0, line.IndexOf('-'));

                    if (keyVal == key)
                    {
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            writer.BaseStream.Seek(seekPos, SeekOrigin.Begin);
                            writer.BaseStream.Write(Encoding.ASCII.GetBytes(str), 0, 24);
                        }

                        return;
                    }

                    seekPos += 24;
                }


                if (stream.CanWrite)
                    stream.Write(Encoding.ASCII.GetBytes(str), (int)stream.Length, 24);
            }

            catch (IOException)
            {
                Logger.Log("Failed to write to data file.");
            }
        }

        public async Task<int> ReadValueAsync(string key)
        {
            try
            {
                int seekPos = 0;
                byte[] buffer = new byte[24];

                while (seekPos < stream.Length)
                {
                    stream.Seek(seekPos, SeekOrigin.Begin);
                    await stream.ReadAsync(buffer, 0, 24);
                    var line = Decipher(Encoding.ASCII.GetString(buffer));
                    var keyVal = line.Substring(0, line.IndexOf('-'));
                    var value = line.Substring(line.IndexOf('-') + 1);
                    if (keyVal == key)
                        return Convert.ToInt32(value);
                    seekPos += 24;
                }

                return 0;
            }

            catch (IOException)
            {
                Logger.Log("Failed to read from stats file.");
                return 1;
            }
        }

        private string Encrypt(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] keyBytes = new Rfc2898DeriveBytes(DataHash, Encoding.ASCII.GetBytes(Salt)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }

        private string Decipher(string encryptedText)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = new Rfc2898DeriveBytes(DataHash, Encoding.ASCII.GetBytes(Salt)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }

    }
}
