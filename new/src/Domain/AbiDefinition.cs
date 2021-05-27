﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Erdcsharp.Domain.Serializer;
using Erdcsharp.Domain.Values;

namespace Erdcsharp.Domain
{
    public class AbiDefinition
    {
        public string Name { get; set; }
        public Constructor Constructor { get; set; }
        public Endpoint[] Endpoints { get; set; }
        public Dictionary<string, Types> Types { get; set; }

        public EndpointDefinition GetEndpointDefinition(string endpoint)
        {
            var data = Endpoints.ToList().SingleOrDefault(s => s.Name == endpoint);
            if (data == null)
                throw new Exception("Endpoint is not define in ABI");

            var inputs = data.Inputs.Select(i => new FieldDefinition(i.Name, "", GetTypeValue(i.Type))).ToList();
            var outputs = data.Outputs.Select(i => new FieldDefinition("", "", GetTypeValue(i.Type))).ToList();
            return new EndpointDefinition(endpoint, inputs.ToArray(), outputs.ToArray());
        }

        private TypeValue GetTypeValue(string rustType)
        {
            var optional = new Regex("^optional<(.*)>$");
            var multi = new Regex("^multi<(.*)>$");

            if (optional.IsMatch(rustType))
            {
                var innerType = optional.Match(rustType).Groups[1].Value;
                var innerTypeValue = GetTypeValue(innerType);
                return TypeValue.OptionValue(innerTypeValue);
            }

            if (multi.IsMatch(rustType))
            {
                var innerTypes = multi.Match(rustType).Groups[1].Value.Split(',').Where(s => !string.IsNullOrEmpty(s));
                var innerTypeValues = innerTypes.Select(GetTypeValue).ToArray();
                return TypeValue.MultiValue(innerTypeValues);
            }

            var typeFromBaseRustType = TypeValue.FromRustType(rustType);
            if (typeFromBaseRustType != null)
                return typeFromBaseRustType;

            if (Types.Keys.Contains(rustType))
            {
                var typeFromStruct = Types[rustType];
                return TypeValue.StructValue(
                    typeFromStruct.Type,
                    typeFromStruct.Fields
                        .ToList()
                        .Select(c => new FieldDefinition(c.Name, "", GetTypeValue(c.Type)))
                        .ToArray()
                );
            }

            return null;
        }

        public static AbiDefinition FromJson(string json)
        {
            return JsonSerializer.Deserialize<AbiDefinition>(json);
        }

        public static AbiDefinition FromJsonFilePath(string jsonFilePath)
        {
            var fileBytes = File.ReadAllBytes(jsonFilePath);
            var json = Encoding.UTF8.GetString(fileBytes);
            return FromJson(json);
        }
    }

    public class Constructor
    {
        public Input[] Inputs { get; set; }
        public object[] Outputs { get; set; }
    }

    public class Input
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool multi_arg { get; set; }
    }

    public class Types
    {
        public string Type { get; set; }
        public Field[] Fields { get; set; }
    }

    public class Field
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class Endpoint
    {
        public string Name { get; set; }
        public Input[] Inputs { get; set; }
        public Output[] Outputs { get; set; }
        public string[] PayableInTokens { get; set; }
    }

    public class Output
    {
        public string Type { get; set; }
        public bool MultiResult { get; set; }
    }
}