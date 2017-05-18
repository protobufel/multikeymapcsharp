using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace MultiKeyMapTests
{
    class CommonHelpers
    {

        public static byte[] SerializeHelper<T>(T serialObj)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                formatter.Serialize(memoryStream, serialObj);
                //memoryStream.Position = 0;
                return memoryStream.ToArray();
            }

        }

        public static T DeserializeHelper<T>(byte[] serialized)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (MemoryStream memoryStream = new MemoryStream(serialized))
            {
                return (T)formatter.Deserialize(memoryStream);
            }
        }

        public static Class1<T> GetClass1<T>(T any)
        {
            return new Class1<T>(any);
        }

        public static ValueTuple<T> GetValueTuple<T>(T any)
        {
            return new ValueTuple<T>(any);
        }

        public class Class1<T> : Tuple<T>
        {

            public Class1(T name) : base(name)
            {
                Name = name;
            }

            public T Name { get; private set; }
        }
    }
}
