using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Chunker.Utils
{
    public class Util
    {
        public enum VALIDATE_CODE_TYPE
        {
            CHARACTER_NUMBER,
            NUMBER,
            CHARACTER
        }

        public static string GetMD5(string input)
        {
            byte[] result = Encoding.Default.GetBytes(input);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);

            return BitConverter.ToString(output).Replace("-", "").ToLower();
        }

        public static string GenerateCheckCode(VALIDATE_CODE_TYPE type, int len)
        {
            char[] VALIDATE_CODE_CHARACTERS = { '2', '3', '4', '5', '6', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'R', 'S', 'T', 'W', 'X', 'Y' };

            char code;
            string checkCode = String.Empty;
            System.Random random = new Random();

            for (int i = 0; i < len; i++)
            {
                code = VALIDATE_CODE_CHARACTERS[random.Next(VALIDATE_CODE_CHARACTERS.Length)];

                if (type == VALIDATE_CODE_TYPE.NUMBER)
                {
                    if ((int)code < 48 || (int)code > 57)
                    {
                        i--;
                        continue;
                    }
                }
                else if (type == VALIDATE_CODE_TYPE.CHARACTER)
                {
                    if ((int)code < 65 || (int)code > 90)
                    {
                        i--;
                        continue;
                    }
                }
                checkCode += code;
            }

            return checkCode.ToLower();
        }

        public static string GetQueryString(HttpRequestMessage request, string key)
        {
            var queryStrings = request.GetQueryNameValuePairs();
            if (queryStrings == null)
                return null;

            var match = queryStrings.FirstOrDefault(kv => string.Compare(kv.Key, key, true) == 0);
            if (string.IsNullOrEmpty(match.Value))
                return null;

            return match.Value;
        }
    }
}