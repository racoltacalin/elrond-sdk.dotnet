﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Erdcsharp.Domain.Codec;
using Erdcsharp.Domain.Helper;
using Erdcsharp.Domain.Values;
using Erdcsharp.Provider;
using Erdcsharp.Provider.Dtos;

namespace Erdcsharp.Domain
{
    public class TransactionRequest
    {
        private readonly Account _account;
        private readonly string _chainId;
        private const int TransactionVersion = 4;

        public Address Sender { get; }
        public long Nonce { get; }
        public long GasPrice { get; }
        public TokenAmount Value { get; private set; }
        public Address Receiver { get; private set; }
        public GasLimit GasLimit { get; private set; }
        public string Data { get; private set; }

        private TransactionRequest(Account account, Constants constants)
        {
            _account = account;
            _chainId = constants.ChainId;
            Sender = account.Address;
            Receiver = Address.Zero();
            Value = new TokenAmount(0);
            Nonce = account.Nonce;
            GasLimit = new GasLimit(constants.MinGasLimit);
            GasPrice = constants.MinGasPrice;
        }

        public static TransactionRequest CreateTransaction(Account account, Constants constants)
        {
            return new TransactionRequest(account, constants);
        }

        public static TransactionRequest CreateTransaction(Account account, Constants constants, Address receiver,
            TokenAmount value)
        {
            return new TransactionRequest(account, constants)
            {
                Receiver = receiver,
                Value = value
            };
        }

        public void SetGasLimit(GasLimit gasLimit)
        {
            GasLimit = gasLimit;
        }

        public void SetData(string data)
        {
            var dataBytes = Encoding.UTF8.GetBytes(data);
            Data = Convert.ToBase64String(dataBytes);
        }

        public string GetDecodedData()
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(Data));
        }

        public TransactionRequestDto GetTransactionRequest()
        {
            var transactionRequestDto = new TransactionRequestDto
            {
                ChainID = _chainId,
                Data = Data,
                GasLimit = GasLimit.Value,
                Receiver = Receiver.Bech32,
                Sender = Sender.Bech32,
                Value = Value.ToString(),
                Version = TransactionVersion,
                Signature = null,
                Nonce = Nonce,
                GasPrice = GasPrice
            };

            return transactionRequestDto;
        }

        public async Task<Transaction> Send(IElrondProvider provider, Wallet wallet)
        {
            var transactionRequestDto = GetTransactionRequest();
            var account = wallet.GetAccount();
            await account.Sync(provider);

            if (Value.Number > account.Balance.Number)
                throw new Exception($"Insufficient funds, required : {Value} and got {account.Balance}");

            if (Nonce != account.Nonce)
                throw new Exception($"Incorrect nonce, account nonce is {account.Nonce}, not {Nonce}");

       

            var json = JsonSerializer.Serialize(transactionRequestDto);
            var message = Encoding.UTF8.GetBytes(json);

            transactionRequestDto.Signature = wallet.Sign(message);

            var result = await provider.SendTransaction(transactionRequestDto);
            _account.IncrementNonce();
            return Transaction.From(result);
        }

        public void AddArgument(IBinaryType[] args)
        {
            if (!args.Any())
                return;

            var binaryCodec = new BinaryCodec();
            var decodedData = GetDecodedData();
            var data = args.Aggregate(decodedData, (c, arg) => c + $"@{Converter.ToHexString(binaryCodec.EncodeTopLevel(arg))}");
            SetData(data);
        }
    }
}