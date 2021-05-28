using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Erdcsharp.Domain;
using Erdcsharp.Domain.Values;

namespace Erdcsharp.Manager
{
    public interface IEsdtTokenManager
    {
        Task<string> IssueNonFungibleToken(Wallet wallet, string tokenName, string tokenTicker);

        Task<List<string>> GetSpecialRole(string tokenIdentifier);

        Task SetSpecialRole(Wallet wallet, string tokenIdentifier, params string[] roles);

        Task<EsdtToken> CreateNftToken(
            Wallet wallet,
            string tokenIdentifier,
            string tokenName,
            ushort royalties,
            Dictionary<string, string> attributes,
            Uri[] uris,
            byte[] hash = null);


        Task<EsdtToken> GetNftToken(Address address, string tokenIdentifier, ulong tokenId);

        Task TransferEsdtToken(Wallet wallet, EsdtToken token, Address receiver, BigInteger quantity);

        Task TransferEsdtTokenToSmartContract(Wallet wallet, EsdtToken token, Address smartContract, string functionName, BigInteger quantity, params IBinaryType[] args);
    }
}