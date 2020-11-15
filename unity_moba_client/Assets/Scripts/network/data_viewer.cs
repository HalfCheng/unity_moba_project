using System;

public class data_viewer
{
    /// <summary>
    /// 小尾  低位存低内存地址，高位存高内存地址
    /// 大尾  低位存高内存地址，高位存低内存地址
    /// </summary>
    /// <param name="buf"></param>
    /// <param name="offset"></param>
    /// <param name="value"></param>
    public static void write_ushort_le(byte[] buf, int offset, ushort value)
    {
        byte[] byte_value = BitConverter.GetBytes(value);
        //小尾 还是 大尾  BitConvert 系统是小尾还是大尾
        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(byte_value);
        }

        Array.Copy(byte_value, 0, buf, offset, byte_value.Length);
    }

    public static void write_uint_le(byte[] buf, int offset, uint value)
    {
        byte[] byte_value = BitConverter.GetBytes(value);
        //小尾 还是 大尾  BitConvert 系统是小尾还是大尾
        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(byte_value);
        }

        Array.Copy(byte_value, 0, buf, offset, byte_value.Length);
    }

    public static void write_bytes(byte[] dst, int offset, byte[] value)
    {
        Array.Copy(value, 0, dst, offset, value.Length);
    }

    public static ushort read_ushort_le(byte[] data, int offset)
    {
        return (ushort)(data[offset] | (data[offset + 1] << 8));
    }
}