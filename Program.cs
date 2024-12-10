using System.Runtime.InteropServices;

namespace ARPSender
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string desctinationAdress = "192.168.0.1"; // IP Adress of device in your network. Example: 192.168.0.1
            string sourceAdress = "192.168.0.10"; // IP Adress of your device in your network. Example: 192.168.0.100

            unsafe
            {
                long macAdress = 0;
                ulong macAdressLength = 6;
                int result = SendARPRequest(desctinationAdress, sourceAdress, &macAdress, &macAdressLength);
                if (result == 0)
                {
                    if (macAdress != 0)
                    {
                        Console.WriteLine("ARP request was successfull!");
                        Console.WriteLine($"Raw MAC-Adress: {macAdress}");
                        Console.WriteLine($"MAC-Adress: {NumberToMACAdress(macAdress)}");
                        Console.WriteLine($"MAC-Adress Length: {macAdressLength}");
                    }
                    else
                        Console.WriteLine("ARP request was sended, but MAC-Adress equals to Zero");
                }
                else
                    Console.WriteLine($"ARP request failed! Error Code: {result}");

                [DllImport("iphlpapi.dll", ExactSpelling = true)]
                static extern int SendARP(long destinationIpAdress, long sourceIpAdress, long* buffer, ulong* length);

                static int SendARPRequest(string destination, string source, long* buffer, ulong* length)
                    => SendARP(IPAdressToNumber(destination), IPAdressToNumber(source), buffer, length);
            }
        }

        public static long IPAdressToNumber(string ip)
        {
            var splittedAdress = ip.Split('.');
            var binaryAdress = splittedAdress.Select(b => NormalizeTo8Chars(Convert.ToString(int.Parse(b), 2)));
            var binaryIPAdress = string.Join("", binaryAdress.Reverse());
            return Convert.ToInt64(binaryIPAdress, 2);
        }

        public static string NormalizeTo8Chars(string s)
        {
            if (s.Length >= 8)
                return s;
            int count = 8 - s.Length;
            for (int i = 0; i < count; i++)
                s = "0" + s;
            return s;
        }

        public static string NumberToMACAdress(long number)
        {
            string result = "";
            string hexString = Convert.ToString(number, 16);
            for (int i = hexString.Length - 1; i > 0; i -= 2)
                result += "" + hexString[i - 1] + hexString[i] + ":";
            return result[..^1];
        }
    }
}