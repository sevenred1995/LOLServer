using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NetFrame {
    public class ByteArray {
        public MemoryStream ms;
        public BinaryReader br;
        public BinaryWriter bw;
        public void Close() {
            ms.Close();
            br.Close();
            bw.Close();
        }
        public ByteArray(byte[] buff) {
            ms = new MemoryStream(buff);
            br = new BinaryReader(ms);
            bw = new BinaryWriter(ms);
        }
        public int Postion {
            get { return (int)ms.Position; }
        }
        public int Length {
            get { return (int)ms.Length; }
        }
        public bool Readnable {
            get { return ms.Length > ms.Position; }
        }
        public ByteArray() {
            ms = new MemoryStream();
            br = new BinaryReader(ms);
            bw = new BinaryWriter(ms);
        }
        public void write(int value) {
            bw.Write(value);
        }
        public void write(byte value) {
            bw.Write(value);
        }
        public void write(byte[] value) {
            bw.Write(value);
        }
        public void write(bool value) {
            bw.Write(value);
        }
        public void write(string value) {
            bw.Write(value);
        }
        public void write(double value) {
            bw.Write(value);
        }
        public void write(float value) {
            bw.Write(value);
        }
        public void write(long value) {
            bw.Write(value);
        }
        public void read(out int value) {
            value = br.ReadInt32();
        }
        public void read(out byte value) {
            value = br.ReadByte();
        }
        public void read(out bool value) {
            value = br.ReadBoolean();
        }
        public void read(out string value) {
            value = br.ReadString();
        }
        public void read(out double value) {
            value = br.ReadDouble();
        }
        public void read(out float value) {
            value = br.ReadSingle();
        }
        public void read(out long value) {
            value = br.ReadInt64();
        }
        public void read(out byte[] value, int length) {
            value = br.ReadBytes(length);
        }
        public void repostion() {
            ms.Position = 0;
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public byte[] getBuff() {
            byte[] result = new byte[ms.Length];
            Buffer.BlockCopy(ms.GetBuffer(), 0, result, 0, (int)ms.Length);
            return result;
        }

    }
}
