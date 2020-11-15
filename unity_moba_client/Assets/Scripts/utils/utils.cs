using System;
using System.Security.Cryptography;
using System.Text;
using Random = System.Random;

public class utils
{
    public static string rand_str(int len)
    {
        byte[] b = new byte[4];
        new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
        Random r = new Random(BitConverter.ToInt32(b, 0));

        string str = null;
        str += "0123456789";
        str += "abcdefghijklmnopqrstuvwxyz";
        str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        string s = null;

        for (int i = 0; i < len; i++)
        {
            s += str.Substring(r.Next(0, str.Length - 1), 1);
        }

        return s;
    }

    public static string md5(string str)
    {
        string cl = str;
        StringBuilder md5_builder = new StringBuilder();
        MD5 md5 = MD5.Create();//实例化一个md5对象
        //加密后是一个字节类型的数组，这里要注意编码utf8/unicode等选择
        byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
        //通过使用循环，将字节类型的数组转化为植复仇，此字符串是常规字符串格式化所得
        for(int i = 0; i < s.Length; i++)
        {
            //将得到的字符串使用16进制类型格式，格式后的字符是小写的字母，如果使用大写（）
            md5_builder.Append(s[i].ToString("X2"));
        }

        return md5_builder.ToString();
    }
}
