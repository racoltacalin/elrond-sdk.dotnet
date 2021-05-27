using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Erdcsharp.Domain.Serializer;
using Erdcsharp.Provider.Dtos;

namespace Erdcsharp.Provider
{
    public class ElrondProvider : IElrondProvider
    {
        private readonly HttpClient _httpClient;

        public ElrondProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ConfigDataDto> GetConstants()
        {
            var response = await _httpClient.GetAsync("network/config");
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ElrondGatewayResponseDto<ConfigDataDto>>(content);
            result.EnsureSuccessStatusCode();
            return result.Data;
        }

        public async Task<AccountDataDto> GetAccount(string address)
        {
            var response = await _httpClient.GetAsync($"address/{address}");

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ElrondGatewayResponseDto<AccountDataDto>>(content);
            result.EnsureSuccessStatusCode();
            return result.Data;
        }

        public async Task<ESDTTokenDataDto> GetEsdtTokens(string address)
        {
            var response = await _httpClient.GetAsync($"address/{address}/esdt");

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ElrondGatewayResponseDto<ESDTTokenDataDto>>(content);
            result.EnsureSuccessStatusCode();
            return result.Data;
        }

        public async Task<EsdtNftItemDto> GetEsdtNftToken(string address, string tokenIdentifier, ulong tokenId)
        {
            var response = await _httpClient.GetAsync($"address/{address}/nft/{tokenIdentifier}/nonce/{tokenId}");

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ElrondGatewayResponseDto<EsdtNftItemDto>>(content);
            result.EnsureSuccessStatusCode();
            return result.Data;
        }

        public async Task<EsdtDataDto> GetEsdtToken(string address, string tokenIdentifier)
        {
            var response = await _httpClient.GetAsync($"address/{address}/esdt/{tokenIdentifier}");

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ElrondGatewayResponseDto<EsdtDataDto>>(content);
            result.EnsureSuccessStatusCode();
            return result.Data;
        }

        public async Task<CreateTransactionResponseDataDto> SendTransaction(TransactionRequestDto transactionRequestDto)
        {
            var raw = JsonSerializer.Serialize(transactionRequestDto);
            var payload= new StringContent(raw, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("transaction/send", payload);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ElrondGatewayResponseDto<CreateTransactionResponseDataDto>>(content);
            result.EnsureSuccessStatusCode();
            return result.Data;
        }

        public async Task<TransactionCostDataDto> GetTransactionCost(TransactionRequestDto transactionRequestDto)
        {
            var raw = JsonSerializer.Serialize(transactionRequestDto);
            var payload = new StringContent(raw, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("transaction/cost", payload);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ElrondGatewayResponseDto<TransactionCostDataDto>>(content);
            result.EnsureSuccessStatusCode();
            return result.Data;
        }

        public async Task<QueryVmResultDataDto> QueryVm(QueryVmRequestDto queryVmRequestDto)
        {
            var raw = JsonSerializer.Serialize(queryVmRequestDto);
            var payload = new StringContent(raw, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("vm-values/query", payload);
            
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ElrondGatewayResponseDto<QueryVmResultDataDto>>(content);
            result.EnsureSuccessStatusCode();
            return result.Data;
        }

        public async Task<TransactionResponseData> GetTransactionDetail(string txHash)
        {
            var response = await _httpClient.GetAsync($"transaction/{txHash}?withResults=true");

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ElrondGatewayResponseDto<TransactionResponseData>>(content);
            result.EnsureSuccessStatusCode();
            return result.Data;
        }
    }
}