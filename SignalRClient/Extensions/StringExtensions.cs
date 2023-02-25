using System.Collections.Generic;
using System.Linq;

namespace SignalRClient.Extensions
{
    public static class StringExtensions
    {
        public static List<string> ToListFromCommaSeparateString(this string? sentence)
        {
            if (sentence == null) return new List<string>();
            var result = sentence.Split(',').ToList();
            result.ForEach(x => x = x.Trim());
            return result;

        }
    }
}