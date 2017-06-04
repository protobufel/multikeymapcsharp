/*
Copyright 2017 David Tesler

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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
