using System.IO;
using Erdcsharp.Domain.Serializer;

namespace Erdcsharp.Domain
{
    public class Code
    {
        public string Value { get; }

        public Code(byte[] bytes)
        {
            Value = Converter.ToHexString(bytes);
        }

        public static Code FromFilePath(string filePath)
        {
            var fileBytes = File.ReadAllBytes(filePath);
            return new Code(fileBytes);
        }
    }
}