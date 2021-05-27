using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using dotnetstandard_bip32;
using Erdcsharp.Domain.Values;

namespace Erdcsharp.Domain.Codec
{
    public class NumericBinaryCodec : IBinaryCodec
    {
        public string Type => TypeValue.BinaryTypes.Numeric;

        public (IBinaryType Value, int BytesLength) DecodeNested(byte[] data, TypeValue type)
        {
            if (type.HasFixedSize())
            {
                const int offset = 0;
                var length = type.SizeInBytes();
                var payload = data.Slice(offset, offset + length);
                var result = DecodeTopLevel(payload, type);
                var decodedLength = length + offset;
                return (result, decodedLength);
            }
            else
            {
                const int offset = 4;
                var sizeInBytes = (int) BitConverter.ToUInt32(data.Slice(0, offset), 0);
                if (BitConverter.IsLittleEndian)
                {
                    sizeInBytes = (int) BitConverter.ToUInt32(data.Slice(0, offset).Reverse().ToArray(), 0);
                }

                var payload = data.Skip(offset).Take(sizeInBytes).ToArray();
                if (BitConverter.IsLittleEndian)
                {
                    payload = payload.Reverse().ToArray();
                }

                var bigNumber = new BigInteger(payload);
                //var bigNumber = new BigInteger(payload, !type.HasSign(), isBigEndian: true);
                return (new NumericValue(type, bigNumber), sizeInBytes + offset);
            }
        }

        public IBinaryType DecodeTopLevel(byte[] data, TypeValue type)
        {
            if (data.Length == 0)
            {
                return new NumericValue(type, new BigInteger(0));
            }

            if (!type.HasSign())
            {
                const byte unsigned = 0x00;
                data = data.Prepend(unsigned).ToArray();
            }

            if (BitConverter.IsLittleEndian)
            {
                data = data.Reverse().ToArray();
            }

            var bigNumber = new BigInteger(data);
            return new NumericValue(type, bigNumber);
        }

        public byte[] EncodeNested(IBinaryType value)
        {
            var numericValue = value.ValueOf<NumericValue>();
            if (value.Type.HasFixedSize())
            {
                var sizeInBytes = value.Type.SizeInBytes();
                var number = numericValue.Number;
                var fullArray = Enumerable.Repeat((byte) 0x00, sizeInBytes).ToArray();
                if (number.IsZero)
                {
                    return fullArray;
                }

                var hasSign = value.Type.HasSign();
                var payload = number.ToByteArray();
                var payloadLength = payload.Length;

                var buffer = new List<byte>();
                buffer.AddRange(fullArray.Slice(0, sizeInBytes - payloadLength));
                if (BitConverter.IsLittleEndian)
                {
                    payload = payload.Reverse().ToArray();
                }

                buffer.AddRange(payload);
                var data = buffer.ToArray();

                return data;
            }
            else
            {
                var payload = EncodeTopLevel(value);
                var sizeBytes = BitConverter.GetBytes(payload.Length).ToList();
                if (BitConverter.IsLittleEndian)
                {
                    sizeBytes.Reverse();
                }

                var buffer = new List<byte>();
                buffer.AddRange(sizeBytes);
                buffer.AddRange(payload);
                var data = buffer.ToArray();
                return data;
            }
        }

        public byte[] EncodeTopLevel(IBinaryType value)
        {
            var numericValue = value.ValueOf<NumericValue>();
            // Nothing or Zero:
            if (numericValue.Number.IsZero)
            {
                return new byte[0];
            }

            var isUnsigned = !value.Type.HasSign();
            var buffer = numericValue.Number.ToByteArray();
            if (BitConverter.IsLittleEndian)
            {
                buffer = buffer.Reverse().Skip(isUnsigned ? 1 : 0).ToArray();
            }

            return buffer;
        }
    }
}