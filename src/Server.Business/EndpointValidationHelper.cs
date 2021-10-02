﻿using System.Text.RegularExpressions;

namespace Server
{
    public static class EndpointValidationHelper
    {
        private static readonly Regex Regex = new("(http(s)?:\\/\\/.)?(www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{2,256}\\.[a-z]{2,6}\\b([-a-zA-Z0-9@:%_\\+.~#?&//=]*)",
            RegexOptions.Compiled);

        public static bool IsHttpEndpoint(string endpoint)
        {
            return Regex.IsMatch(endpoint);
        }
    }
}