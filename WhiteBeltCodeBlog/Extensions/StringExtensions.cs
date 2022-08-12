using System.Globalization;
using System.Text.RegularExpressions;

namespace WhiteBeltCodeBlog.Extensions
{
    public static class StringExtensions
    {
        public static string? Sluggify(this string phrase) //The "This" keyword allows the 
        {
            // Remove all accents and make the string lower case
            string output = phrase.RemoveAccents().ToLower();


            // Remove all special characters from the string.
            output = Regex.Replace(output, @"[^a-zA-Z0-9\s-]", "");

            // Remove all additional spaces in favor of just one
            output = Regex.Replace(output, @"\s+", " ").Trim();

            // Replace all spaces with a hyphen
            output = Regex.Replace(output, @"\s", "-");

            // Return the slug
            return output;
        }

        private static string? RemoveAccents(this string phrase)
        {
            if (string.IsNullOrWhiteSpace(phrase))
            {
                return phrase;
            }

            phrase = phrase.Normalize(System.Text.NormalizationForm.FormD);



            char[] chars = phrase.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();

            return new string(chars).Normalize(System.Text.NormalizationForm.FormC);


        }
    }
}
