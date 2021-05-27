using System;
using Erdcsharp.Cryptography;
using Erdcsharp.Domain.Serializer;

namespace Erdcsharp.Domain.Values
{
    public class AddressValue : BaseBinaryValue
    {
        public byte[] Buffer { get; }
        public string Bech32 { get; }
        public string Hex { get; }
        private const string Hrp = "erd";

        private AddressValue(string hex, string bech32)
            : base(TypeValue.AddressValue)
        {
            Bech32 = bech32.ToLowerInvariant();
            Hex = hex.ToUpperInvariant();
            Buffer = Converter.FromHexString(hex);
        }

        public static AddressValue FromBytes(byte[] data)
        {
            var hex = Converter.ToHexString(data);
            var bech32 = Bech32Engine.Encode(Hrp, data);
            return new AddressValue(hex, bech32);
        }

        public static AddressValue FromHex(string hex)
        {
            var bech32 = Bech32Engine.Encode(Hrp, Converter.FromHexString(hex));
            return new AddressValue(hex, bech32);
        }

        public static AddressValue FromBech32(string bech32)
        {
            Bech32Engine.Decode(bech32, out _, out var data);
            var hex = Converter.ToHexString(data);
            return new AddressValue(hex, bech32);
        }

        public byte[] PublicKey()
        {
            return Converter.FromHexString(Hex);
        }

        /// <summary>
        /// Smart contract deployment address
        /// </summary>
        /// <returns></returns>
        public static AddressValue Zero()
        {
            var hex = "0000000000000000000000000000000000000000000000000000000000000000";
            return FromHex(hex);
        }

        public override string ToString()
        {
            return Bech32;
        }
    }
}