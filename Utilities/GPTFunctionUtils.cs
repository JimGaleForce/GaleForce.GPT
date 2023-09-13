namespace GaleForce.GPT.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GaleForce.GPT.Attributes;
    using GaleForce.GPT.Services.OpenAI;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public static class GPTFunctionUtils
    {
        public static JArray AsJsonArray(string json)
        {
            return JsonConvert.DeserializeObject<JArray>(json);
        }

        public static string GPTFunctionJson<T>(T aiFunction)
            where T : class
        {
            var functions = new List<Dictionary<string, object>>();

            var types = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => p.GetCustomAttributes(typeof(AIFunctionAttribute), false).Any())
                .ToList();

            foreach (var type in types)
            {
                var functionAttr = type.GetCustomAttributes(typeof(AIFunctionAttribute), false).FirstOrDefault() as AIFunctionAttribute;
                var function = new Dictionary<string, object>();
                function.Add("name", functionAttr.Name);
                if (functionAttr.Description != null)
                {
                    function.Add("description", functionAttr.Description);
                }

                function.Add("parameters", GetParameters(type));
                functions.Add(function);
            }

            var jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };

            var json = JsonConvert.SerializeObject(functions, jsonSettings);
            return json;
        }

        private static Dictionary<string, object> GetParameters(Type type)
        {
            var properties = type.GetProperties();
            var parameters = new Dictionary<string, object>
            {
                { "type", "object" },
                { "properties", new Dictionary<string, Dictionary<string, object>>() },
                { "required", new List<string>() }
            };

            foreach (var property in properties)
            {
                var propertyAttr = property.GetCustomAttributes(typeof(AIPropertyAttribute), false).FirstOrDefault() as AIPropertyAttribute;

                if (propertyAttr == null)
                {
                    continue;
                }

                var typex = property.PropertyType.Name.ToLower();
                if (typex == "double" || typex == "float" || typex == "single" || typex == "decimal")
                {
                    typex = "number";
                }

                if (typex == "int32")
                {
                    typex = "integer";
                }

                var propertyDict = new Dictionary<string, object> { { "type", typex } };

                if (propertyAttr.Description != null)
                {
                    propertyDict.Add("description", propertyAttr.Description);
                }

                if (property.PropertyType.IsEnum)
                {
                    propertyDict["type"] = "string";
                    propertyDict["enum"] =
                        Enum.GetNames(property.PropertyType).Select(e => e.ToLower()).ToArray();
                }

                ((Dictionary<string, Dictionary<string, object>>)parameters["properties"]).Add(
                    property.Name.ToLower(),
                    propertyDict);

                if (propertyAttr.IsRequired)
                {
                    ((List<string>)parameters["required"]).Add(property.Name.ToLower());
                }
            }

            return parameters;
        }

        public static QOpenAIFunction PopulateByName(
            List<QOpenAIFunction> existingFunctions,
            string name,
            string arguments)
        {
            // Find the existing function by name
            var function = existingFunctions.FirstOrDefault(
                f => (f.GetType().GetCustomAttributes(typeof(AIFunctionAttribute), false).FirstOrDefault() as AIFunctionAttribute)?.Name ==
                    name);

            if (function == null)
            {
                // Handle the case where the function doesn't exist in the list
                // CHANGE TO RETURN NULL NEXT TIME - or solve ask
                throw new Exception($"Function with name {name} does not exist in the provided list.");
            }

            // arguments is a serialized json containing key value pairs
            // example: "{\n  \"location\": \"Boston\"\n}"

            // deserialize the arguments
            var deserializedArguments = JsonConvert.DeserializeObject<Dictionary<string, object>>(arguments);

            // go through the reflected properties of the function, and fill in those properties from the deserialized arguments
            foreach (var property in function.GetType().GetProperties())
            {
                var propName = property.Name;
                var aiProperty = property.GetCustomAttributes(typeof(AIPropertyAttribute), false).FirstOrDefault() as AIPropertyAttribute;
                if (aiProperty != null && !string.IsNullOrEmpty(aiProperty.Name))
                {
                    propName = aiProperty.Name;
                }

                var matchingKey = deserializedArguments.Keys
                    .FirstOrDefault(k => k.Equals(propName, StringComparison.OrdinalIgnoreCase));
                if (matchingKey != null && property.CanWrite)
                {
                    if (property.PropertyType.IsEnum)
                    {
                        property.SetValue(
                            function,
                            Enum.Parse(property.PropertyType, deserializedArguments[matchingKey].ToString(), true));
                    }
                    else
                    {
                        property.SetValue(
                            function,
                            Convert.ChangeType(deserializedArguments[matchingKey], property.PropertyType));
                    }
                }
            }

            return function;
        }
    }
}