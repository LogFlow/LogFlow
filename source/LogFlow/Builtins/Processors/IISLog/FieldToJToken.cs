using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace LogFlow.Builtins.Processors.IISLog
{
    public static class FieldToJToken
    {
        private static readonly Dictionary<string, Func<string, JToken>> Mappings;

        static FieldToJToken()
        {
            Mappings = new Dictionary<string, Func<string, JToken>>
            {
                {"time-taken", value => int.Parse(value)},
                {"sc-status", value => int.Parse(value)},
                {"s-port", value => int.Parse(value)},
                {"sc-bytes", value => int.Parse(value)},
                {"cs-bytes", value => int.Parse(value)},
                {"sc-substatus", value => int.Parse(value)},
                {"sc-win32-status", value => int.Parse(value)}
            };
        }

        public static JToken Parse(KeyValuePair<string, string> field)
        {
            if (Mappings.ContainsKey(field.Key))
            {
                var convert = Mappings[field.Key];
                return convert(field.Value);
            }

            return field.Value;
        }
    }
}