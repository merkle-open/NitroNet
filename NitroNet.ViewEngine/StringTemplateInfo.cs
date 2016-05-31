using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace NitroNet.ViewEngine
{
    public class StringTemplateInfo : TemplateInfo
    {
        private readonly string _content;
        private readonly string _eTag;

        public StringTemplateInfo(string id, string content)
            : base(id)
        {
            _content = content;
            _eTag = GetHash(new MD5CryptoServiceProvider(), _content);
        }

        public override Stream Open()
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(_content);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public override string ETag
        {
            get { return _eTag; }
        }

        private static string GetHash(HashAlgorithm md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash. 
            var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes 
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string. 
            for (var i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string. 
            return sBuilder.ToString();
        }
    }
}