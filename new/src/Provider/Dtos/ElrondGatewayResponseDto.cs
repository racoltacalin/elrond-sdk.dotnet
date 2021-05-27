using Erdcsharp.Domain.Exceptions;

namespace Erdcsharp.Provider.Dtos
{
    public class ElrondGatewayResponseDto<T>
    {
        public T Data { get; set; }
        public string Error { get; set; }
        public string Code { get; set; }

        public void EnsureSuccessStatusCode()
        {
            if (string.IsNullOrEmpty(Error))
                return;

            throw new GatewayException(Error, Code);
        }
    }
}