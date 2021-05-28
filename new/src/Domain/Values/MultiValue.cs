using System.Collections.Generic;
using System.Linq;
using System.Text;
using Erdcsharp.Domain.Helper;

namespace Erdcsharp.Domain.Values
{
    public class MultiValue : BaseBinaryValue
    {
        public Dictionary<TypeValue, IBinaryType> Values { get; }

        public MultiValue(TypeValue type, Dictionary<TypeValue, IBinaryType> values) : base(type)
        {
            Values = values;
        }

        public static MultiValue From(params IBinaryType[] values)
        {
            var t = values.Select(s => s.Type).ToArray();
            return new MultiValue(TypeValue.MultiValue(t), values.ToDictionary(s => s.Type, d => d));
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine(Type.Name);
            foreach (var value in Values)
            {
                builder.AppendLine($"{value.Key}:{value}");
            }

            return builder.ToString();
        }

        public new string ToJSON()
        {
            var dic = new Dictionary<string, object>();
            for (var i = 0; i < Values.Count; i++)
            {
                var value = Values.ToArray()[i];
                if (value.Value.Type.BinaryType == TypeValue.BinaryTypes.Struct)
                {
                    var json = value.Value.ToJSON();
                    var jsonObject = JsonSerializer.Deserialize<object>(json);
                    dic.Add($"Multi_{i}", jsonObject);
                }
                else
                {
                    dic.Add($"Multi_{i}", value.Value.ToString());
                }
            }

            return JsonSerializer.Serialize(dic);
        }
    }
}