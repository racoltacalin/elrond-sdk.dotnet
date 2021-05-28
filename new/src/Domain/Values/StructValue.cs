﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using Erdcsharp.Domain.Exceptions;
using Erdcsharp.Domain.Helper;

namespace Erdcsharp.Domain.Values
{
    public class StructValue : BaseBinaryValue
    {
        public StructField[] Fields { get; }

        public StructValue(TypeValue structType, StructField[] fields) : base(structType)
        {
            Fields = fields;
            CheckTyping();
        }

        public StructField GetStructField(string name)
        {
            var field = Fields.SingleOrDefault(f => f.Name == name);
            return field;
        }

        private void CheckTyping()
        {
            var definitions = Type.GetFieldDefinitions();
            if (Fields.Length != definitions.Length)
            {
                throw new BinaryCodecException("fields length vs. field definitions length");
            }

            for (var i = 0; i < Fields.Length; i++)
            {
                var field = Fields[i];
                var definition = definitions[i];
                var fieldType = field.Value.Type;

                if (fieldType.RustType != definition.Type.RustType)
                    throw new BinaryCodecException("field rustType vs. field definitions rustType");
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine(Type.Name);
            foreach (var structField in Fields)
            {
                builder.AppendLine($"{structField.Name}:{structField.Value}");
            }

            return builder.ToString();
        }

        public new string ToJSON()
        {
            var dic = new Dictionary<string, object>();
            foreach (var field in Fields)
            {
                if (field.Value.Type.BinaryType == TypeValue.BinaryTypes.Struct)
                {
                    var json = field.Value.ToJSON();
                    var jsonObject = JsonSerializer.Deserialize<object>(json);
                    dic.Add(field.Name, jsonObject);
                }
                else
                {
                    dic.Add(field.Name, field.ToString());
                }
            }

            return JsonSerializer.Serialize(dic);
        }
    }
}