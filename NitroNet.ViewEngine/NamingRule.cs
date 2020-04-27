using Newtonsoft.Json.Schema;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace NitroNet.ViewEngine
{
    [Obsolete("will be removed as soon final switch to handlebarsnet is completed")]
    public class NamingRule : INamingRule
	{
        private static readonly Regex PascalCaseRegex = new Regex(@"\p{Lu}\p{Ll}+|\p{Lu}+(?!\p{Ll})|\p{Ll}+|\d+", RegexOptions.Compiled);

        public string GetClassName(JSchema schema, string propertyName)
		{
			var className = GetPropertyName(schema.Title);

			string namespaceName;
			className = ExtractClassName(className, out namespaceName);

			if (string.IsNullOrEmpty(className))
			{
				className = propertyName;
			}

			return className;
		}

        public string GetClassNameFromArrayItem(JSchema schema, string propertyName)
		{
			var className = GetPropertyName(schema.Title);
			if (string.IsNullOrEmpty(className))
			{
				className = Singular(propertyName);
			}

			return className;
		}

		private static string Singular(string propertyName)
		{
			if (propertyName.EndsWith("s"))
				return propertyName.Substring(0, propertyName.Length - 1);

			return propertyName;
		}

		public string GetPropertyName(string input)
		{
			return NormalizeName(input);
		}

        public string GetNamespaceName(JSchema schema)
		{
			var input = NormalizeName(schema.Title);

			string namespaceName;
			ExtractClassName(input, out namespaceName);
			return namespaceName;
		}

		private static string NormalizeName(string input)
		{
			if (string.IsNullOrEmpty(input))
				return input;

			return ConvertToPascalCase(input).Replace(" ", "").Replace("_", "").Trim();
		}

		private static string ExtractClassName(string input, out string namespaceName)
		{
			if (string.IsNullOrEmpty(input))
			{
				namespaceName = null;
				return input;
			}

			var index = input.LastIndexOf('/');
			if (index != -1)
			{
				namespaceName = input.Substring(0, index);
				return input.Substring(index + 1);
			}
			namespaceName = input;
			return input;
		}

		private static string ConvertToPascalCase(string input)
		{
		    return PascalCaseRegex.Replace(input, EvaluatePascal);
		}

	    private static string EvaluatePascal(Match match)
		{
			var value = match.Value;
			var valueLength = value.Length;

			if (valueLength == 1)
				return value.ToUpper();

			if (valueLength <= 2 && IsWordUpper(value))
				return value;

			return value.Substring(0, 1).ToUpper() + value.Substring(1, valueLength - 1).ToLower();
		}

		private static bool IsWordUpper(string word)
		{
			return word.All(c => !char.IsLower(c));
		}
	}
}