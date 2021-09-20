using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ArkaApp.Helper
{
    public class Hash
    {
        public static string HMAC_SHA256(string value, string key)
        {
            // The first two lines take the input values and convert them from strings to Byte arrays
            byte[] HMACkey = (new ASCIIEncoding()).GetBytes(key);
            byte[] HMACdata = (new ASCIIEncoding()).GetBytes(value);

            // create a HMACSHA1 object with the key set
            HMACSHA256 myhmacSha256 = new HMACSHA256(HMACkey);

            // calculate the hash (returns a byte array)
            byte[] HMAChash = myhmacSha256.ComputeHash(HMACdata);

            // loop through the byte array and add append each piece to a string to obtain a hash string
            string strResult = "";
            for (int i = 0; i <= HMAChash.Length - 1; i++)
            {
                strResult += HMAChash[i].ToString("x").PadLeft(2, '0');
            }

            return strResult;

        }
    }
}