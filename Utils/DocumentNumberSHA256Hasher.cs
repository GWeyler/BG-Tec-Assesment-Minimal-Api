using System.Security.Cryptography;
using System.Text;

namespace BG_Tec_Assesment_Minimal_Api.Utils
{
    public class DocumentNumberSHA256Hasher
    {
        public static string ComputeSHA256Hash(string input)
        {
            StringBuilder documentNumberSHA = new StringBuilder();
            using (SHA256 sHA256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] res = sHA256.ComputeHash(inputBytes);

                foreach (byte b in res)
                {
                    documentNumberSHA.Append(b.ToString("x2"));
                }
            }
            return documentNumberSHA.ToString();
        }
    }
}
