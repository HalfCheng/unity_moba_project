using System.CodeDom;

public class tcp_packer
{
    private const int HEADER_SIZE = 2;
    
    /// <summary>
    /// 封包，处理粘包问题，在数据头添加包体大小信息
    /// </summary>
    /// <param name="cmd_data"></param>
    /// <returns></returns>
    public static byte[] pack(byte[] cmd_data)
    {
        int len = cmd_data.Length;
        if (len > 65535 - 2)
        {
            return null;
        }

        int cmd_len = len + HEADER_SIZE;
        byte[] cmd = new byte[cmd_len];
        data_viewer.write_ushort_le(cmd, 0, (ushort)cmd_len);
        data_viewer.write_bytes(cmd, HEADER_SIZE, cmd_data);

        return cmd;
    }
    
    /// <summary>
    /// 拆包
    /// </summary>
    /// <param name="data"></param>
    /// <param name="data_len"></param>
    /// <param name="pkg_size"></param>
    /// <param name="head_size"></param>
    /// <returns></returns>
    public static bool read_header(byte[] data, int data_len, out int pkg_size, out int head_size)
    {
        pkg_size = head_size = 0;
        if (data_len < 2)
            return false;

        head_size = 2;
        pkg_size = (data[0] | (data[1] << 8));
        
        return true;
    }
}