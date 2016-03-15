using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace NetFrame.auto {
    /// <summary>
    /// 底层框架默认的长度消息解码器
    /// </summary>
    public class LengthEncoding {
        /// <summary>
        /// 消息长度编码
        /// </summary>
        /// <param name="buff"></param>
        /// <returns></returns>
        public static byte[] encode(byte[] buff) {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);//写入二进制对象流
            //写入消息长度
            bw.Write(buff.Length);
            //写入消息体
            bw.Write(buff);
            byte[] result = new byte[ms.Length];
            Buffer.BlockCopy(ms.GetBuffer(), 0, result, 0, (int)ms.Length);
            bw.Close();
            ms.Close();
            return result;
        }
        /// <summary>
        /// 粘包长度解码
        /// </summary>
        /// <param name="cache"></param>
        /// <returns></returns>
        public static byte[] decode(ref List<byte> cache) {
            if (cache.Count < 4)
                return null;
            MemoryStream ms = new MemoryStream(cache.ToArray());//创建内存流对象，并写入缓存数据
            BinaryReader br = new BinaryReader(ms);//二进制读取流
            int length = br.ReadInt32();
            if (length > ms.Length - ms.Position)
            {
                return null;
            }
            byte[] result = br.ReadBytes(length);
            cache.Clear();
            //将读取剩余后数据写入缓存
            cache.AddRange(br.ReadBytes((int)(ms.Length - ms.Position)));
            br.Close();
            ms.Close();
            return result;
        }
    }
}
