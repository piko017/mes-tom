using System.Security.Cryptography;
using System.Text;

namespace TMom.Infrastructure.Helper
{
    public class MD5Helper
    {
        /// <summary>
        /// 16位MD5加密
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string MD5Encrypt16(string password)
        {
            var md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(Encoding.Default.GetBytes(password)), 4, 8);
            t2 = t2.Replace("-", string.Empty);
            return t2;
        }

        /// <summary>
        /// 32位MD5加密
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string MD5Encrypt32(string password = "")
        {
            string pwd = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(password) && !string.IsNullOrWhiteSpace(password))
                {
                    MD5 md5 = MD5.Create(); //实例化一个md5对像
                    // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
                    byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
                    // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
                    foreach (var item in s)
                    {
                        // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符
                        pwd = string.Concat(pwd, item.ToString("X2"));
                    }
                }
            }
            catch
            {
                throw new Exception($"错误的 password 字符串:【{password}】");
            }
            return pwd;
        }

        /// <summary>
        /// 64位MD5加密
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string MD5Encrypt64(string password)
        {
            // 实例化一个md5对像
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            MD5 md5 = MD5.Create();
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(s);
        }

        /// <summary>
        /// Sha1加密
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        /// <returns>加密后的十六进制的哈希散列（字符串）</returns>
        public static string Sha1(string str, string format = "x2")
        {
            var buffer = Encoding.UTF8.GetBytes(str);
            var data = SHA1.Create().ComputeHash(buffer);
            var sb = new StringBuilder();
            foreach (var t in data)
            {
                sb.Append(t.ToString(format));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Sha256加密
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        /// <returns>加密后的十六进制的哈希散列（字符串）</returns>
        public static string Sha256(string str, string format = "x2")
        {
            var buffer = Encoding.UTF8.GetBytes(str);
            var data = SHA256.Create().ComputeHash(buffer);
            var sb = new StringBuilder();
            foreach (var t in data)
            {
                sb.Append(t.ToString(format));
            }
            return sb.ToString();
        }

        /// <summary>
        /// DES加密(FactoryCode使用)
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string DesEncrypt(string input, string key = "iwop-3hn8-syty19")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(input)) return input;
                byte[] inputArray = Encoding.UTF8.GetBytes(input);
                var tripleDES = TripleDES.Create();
                var byteKey = Encoding.UTF8.GetBytes(key);
                byte[] allKey = new byte[24];
                Buffer.BlockCopy(byteKey, 0, allKey, 0, 16);
                Buffer.BlockCopy(byteKey, 0, allKey, 16, 8);
                tripleDES.Key = allKey;
                tripleDES.Mode = CipherMode.ECB;
                tripleDES.Padding = PaddingMode.PKCS7;
                ICryptoTransform cTransform = tripleDES.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
                return Convert.ToBase64String(resultArray, 0, resultArray.Length).Replace("/", "%2F.");
            }
            catch
            {
                return input;
            }
        }

        /// <summary>
        /// DES解密(FactoryCode使用)
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string DesDecrypt(string input, string key = "iwop-3hn8-syty19")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(input)) return input;
                input = input.Replace("%2F.", "/");
                byte[] inputArray = Convert.FromBase64String(input);
                var tripleDES = TripleDES.Create();
                var byteKey = Encoding.UTF8.GetBytes(key);
                byte[] allKey = new byte[24];
                Buffer.BlockCopy(byteKey, 0, allKey, 0, 16);
                Buffer.BlockCopy(byteKey, 0, allKey, 16, 8);
                tripleDES.Key = allKey;
                tripleDES.Mode = CipherMode.ECB;
                tripleDES.Padding = PaddingMode.PKCS7;
                ICryptoTransform cTransform = tripleDES.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
                return Encoding.UTF8.GetString(resultArray);
            }
            catch
            {
                return input;
            }
        }
    }
}