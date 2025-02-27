using System.Globalization;
using System.Text.RegularExpressions;

namespace WirelessSignals.Tools
{
    internal class BoltIdTool
    {
        public static Bolt.NetworkId StringToBoltNetworkId(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException("Input string cannot be null or empty");
            }

            var match = Regex.Match(input, @"\[(?i:NetworkID) ([0-9A-Fa-f]{1,2}(?:-[0-9A-Fa-f]{1,2}){7})\]", RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                throw new ArgumentException($"Input string '{input}' is not in the correct format. Please provide a valid input string.");
            }

            string[] parts = match.Groups[1].Value.Split('-');
            byte[] bytes = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                try
                {
                    bytes[i] = byte.Parse(parts[i], NumberStyles.HexNumber);
                }
                catch (FormatException)
                {
                    throw new ArgumentException($"Part '{parts[i]}' in the input string '{input}' is not in the correct format. Please provide a valid input string.");
                }
            }

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            ulong networkIdValue = BitConverter.ToUInt64(bytes, 0);
            return new Bolt.NetworkId(networkIdValue);
        }

        public static bool IsInputStringValid(string input)
        {
            // Check if it is default value
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            if (input == "0")
            {
                return false;
            }
            if (input == "[NetworkID 00000000-0000-0000-0000-000000000000]" || input == "[NetworkID 00-00-00-00-00]")
            {
                return false;
            }
            
            var match = Regex.Match(input, @"\[(?i:NetworkID) ([0-9A-Fa-f]{1,2}(?:-[0-9A-Fa-f]{1,2}){7})\]", RegexOptions.IgnoreCase);
            return match.Success;
        }
    }
}
