using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CoD_BSP_Editor.Libs
{
    public static class BinLib
    {
        public static T ReadFromStream<T>(BinaryReader stream) where T : struct
        {
            unsafe
            {
                int size = Marshal.SizeOf<T>();
                byte[] data = stream.ReadBytes(size);

                fixed (byte* ptr = &data[0])
                {
                    return (T)Marshal.PtrToStructure(new IntPtr(ptr), typeof(T));
                }
            }
        }

        public static T ReadFromByteArray<T>(byte[] array) where T : struct
        {
            using (MemoryStream memoryStream = new MemoryStream(array))
            {
                using (BinaryReader stream = new BinaryReader(memoryStream))
                {
                    T obj = ReadFromStream<T>(stream);

                    return obj;
                }
            }
        }

        public static List<T> ReadListFromByteArray<T>(byte[] array) where T : struct
        {
            using (MemoryStream memoryStream = new MemoryStream(array))
            {
                using (BinaryReader stream = new BinaryReader(memoryStream))
                {
                    List<T> objects = new List<T>();
                    int count = array.Length / SizeOf<T>();

                    for (int i = 0; i < count; i++)
                    {
                        T obj = ReadFromStream<T>(stream);
                        objects.Add(obj);
                    }

                    return objects;
                }
            }
        }

        public static T[] ReadMultipleFromStream<T>(BinaryReader stream, int count) where T : struct
        {
            T[] objects = new T[count];

            for (int i = 0; i < count; i++)
            {
                T obj = ReadFromStream<T>(stream);
                objects[i] = obj;
            }

            return objects;
        }

        public static T OpenReadCloseBinary<T>(string filePath, long offset = 0) where T : struct
        {
            using (BinaryReader stream = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                stream.BaseStream.Position = offset;
                return ReadFromStream<T>(stream);
            }
        }

        public static byte[] ToByteArray<T>(T obj) where T : struct
        {
            var size = Marshal.SizeOf<T>();
            // Both managed and unmanaged buffers required.
            var bytes = new byte[size];
            var ptr = Marshal.AllocHGlobal(size);
            // Copy object byte-to-byte to unmanaged memory.
            Marshal.StructureToPtr(obj, ptr, false);
            // Copy data from unmanaged memory to managed buffer.
            Marshal.Copy(ptr, bytes, 0, size);
            // Release unmanaged memory.
            Marshal.FreeHGlobal(ptr);

            return bytes;
        }

        public static byte[] ListToByteArray<T>(List<T> list) where T : struct
        {
            List<byte> data = new List<byte>();
            foreach (T obj in list)
            {
                byte[] bytesFromObj = ToByteArray<T>(obj);
                data.AddRange(bytesFromObj);
            }

            return data.ToArray();
        }

        public static string BytesToString(byte[] str)
        {
            return Encoding.ASCII.GetString(str);
        }
        
        public static byte[] StringToBytes(string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }

        public static int SizeOf<T>()
        {
            return Marshal.SizeOf<T>();
        }
    }
}
