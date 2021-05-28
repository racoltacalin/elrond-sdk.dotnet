﻿using System.Linq;
using System.Numerics;
using System.Text;
using dotnetstandard_bip32;

namespace Erdcsharp.Domain.Helper
{
    public static class Converter
    {
        private const byte UnsignedByte = 0x00;
        public static BigInteger ToBigInteger(byte[] bytes, bool isUnsigned = false, bool isBigEndian = false)
        {
            if (isUnsigned)
            {
                if (bytes.FirstOrDefault() != UnsignedByte)
                {
                    bytes = bytes.Prepend(UnsignedByte).ToArray();
                }
            }

            if (isBigEndian)
            {
                bytes = bytes.Reverse().ToArray();
            }

            return new BigInteger(bytes);
        }

        public static byte[] FromBigInteger(BigInteger bigInteger, bool isUnsigned = false, bool isBigEndian = false)
        {
            var bytes = bigInteger.ToByteArray();
            if (isBigEndian)
            {
                bytes = bytes.Reverse().ToArray();
            }

            if (!isUnsigned) 
                return bytes;
            
            if (bytes.FirstOrDefault() == UnsignedByte)
            {
                bytes = bytes.Slice(1);
            }

            return bytes;
        }

        public static string ToHexString(byte[] bytes)
        {
            var result = new StringBuilder(bytes.Length * 2);
            const string hexAlphabet = "0123456789ABCDEF";

            foreach (var b in bytes)
            {
                result.Append(hexAlphabet[b >> 4]);
                result.Append(hexAlphabet[b & 0xF]);
            }

            return result.ToString();
        }

        public static byte[] FromHexString(string hex)
        {
            var bytes = new byte[hex.Length / 2];
            var hexValue = new[]
            {
                0x00, 0x01, 0x02, 0x03, 0x04, 0x05,
                0x06, 0x07, 0x08, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F
            };

            for (int x = 0, i = 0; i < hex.Length; i += 2, x += 1)
            {
                bytes[x] = (byte) (hexValue[char.ToUpper(hex[i + 0]) - '0'] << 4 |
                                   hexValue[char.ToUpper(hex[i + 1]) - '0']);
            }

            return bytes;
        }
    }
}