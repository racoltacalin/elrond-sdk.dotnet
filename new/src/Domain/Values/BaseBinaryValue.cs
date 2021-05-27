using System.Collections.Generic;
using Erdcsharp.Domain.Serializer;

namespace Erdcsharp.Domain.Values
{
    public class BaseBinaryValue : IBinaryType
    {
        public TypeValue Type { get; }

        public BaseBinaryValue(TypeValue type)
        {
            Type = type;
        }

        public T ValueOf<T>() where T : IBinaryType
        {
            return (T) (IBinaryType)this;
        }

        public T ToObject<T>()
        {
            return JsonSerializer.Deserialize<T>(ToJSON());
        }

        public string ToJSON()
        {
            if (string.IsNullOrEmpty(Type.Name))
            {
                var kv = new KeyValuePair<string, string>(Type.Name ?? "", ToString());
                var json = JsonSerializer.Serialize(kv);
                return json;
            }
            else
            {
                var kv = new Dictionary<string, string>
                {
                    {Type.Name, ToString()}
                };
                var json = JsonSerializer.Serialize(kv);
                return json;
            }
        }
    }
}