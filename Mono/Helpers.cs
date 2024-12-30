
namespace Banking.Mono
{
    internal class Helpers
    {
        internal static bool ExtractNumberFromString(string input, out int result)
        {
            result = 0;

            // Check if string is null or empty
            if (string.IsNullOrEmpty(input))
                return false;

            // Find the opening and closing parentheses
            int openParenIndex = input.IndexOf('(');
            int closeParenIndex = input.IndexOf(')');

            // Verify both parentheses exist and are in correct order
            if (openParenIndex == -1 || closeParenIndex == -1 || openParenIndex >= closeParenIndex)
                return false;

            // Extract the string between parentheses
            string numberString = input.Substring(openParenIndex + 1, closeParenIndex - openParenIndex - 1);

            // Try to parse the extracted string to an integer
            return int.TryParse(numberString, out result);
        }
    }
}
