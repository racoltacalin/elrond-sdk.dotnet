using System.Globalization;
using System.Numerics;
using Erdcsharp.Domain.Exceptions;

namespace Erdcsharp.Domain
{
    public class TokenAmount
    {
        private const long OneEgld = 1000000000000000000;
        private const int Denomination = 18;

        public BigInteger Number { get; }

        public TokenAmount(long value)
        {
            Number = new BigInteger(value);
        }

        public TokenAmount(string value)
        {
            Number = BigInteger.Parse(value);
            if (Number.Sign == -1)
                throw new InvalidBalanceException(value);
        }

        /// <summary>
        /// Returns the string representation of the value (as eGLD currency).
        /// </summary>
        /// <returns></returns>
        public string ToCurrencyString()
        {
            var denominated = ToDenominated();
            return $"{denominated} EGLD";
        }

        public string ToDenominated()
        {
            var padded = Number.ToString().PadLeft(Denomination, '0');

            var start = (padded.Length - Denomination);
            start = start < 0 ? 0 : start;

            var decimals = padded.Substring(start, Denomination);
            var integer = start == 0 ? "0" : padded.Substring(0, start);

            return $"{integer}.{decimals}";
        }

        /// <summary>
        /// Creates a balance object from an eGLD value (denomination will be applied).
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public static TokenAmount EGLD(string value)
        {
            var decimalValue = decimal.Parse(value, CultureInfo.InvariantCulture);
            var p = decimalValue * OneEgld;
            var bigGold = new BigInteger(p);

            return new TokenAmount(bigGold.ToString());
        }

        public static TokenAmount From(string value)
        {
            return new TokenAmount(value);
        }

        public static TokenAmount Zero()
        {
            return new TokenAmount(0);
        }

        public override string ToString()
        {
            return Number.ToString();
        }
    }
}