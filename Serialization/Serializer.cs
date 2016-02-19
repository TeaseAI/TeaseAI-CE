using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.IO;

namespace TeaseAI_CE.Serialization
{
    public static class Serializer
    {
        public static void SerializeAsBinary<T>(T obj, FileInfo file)
        {
            if (file.Exists)
            {
                file.Delete();
            }

            FileStream fs = new FileStream(file.FullName, FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();

            bf.Serialize(fs, obj);

            fs.Close();
        }

        public static T DeserializeFromBinary<T>(FileInfo file)
        {
            T obj;

            FileStream fs = new FileStream(file.FullName, FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();

            obj = (T)bf.Deserialize(fs);

            fs.Close();

            return obj;
        }

        public static void SerializeAsXML<T>(T obj, FileInfo file)
        {
            if (file.Exists)
            {
                file.Delete();
            }

            FileStream fs = new FileStream(file.FullName, FileMode.Create);
            XmlSerializer xmlSer = new XmlSerializer(typeof(T));

            xmlSer.Serialize(fs, obj);

            fs.Close();
        }

        public static T DeserializeFromXML<T>(FileInfo file)
        {
            T obj;

            FileStream fs = new FileStream(file.FullName, FileMode.Open);
            XmlSerializer xmlSer = new XmlSerializer(typeof(T));

            obj = (T)xmlSer.Deserialize(fs);

            fs.Close();

            return obj;
        }

        public static void SerializeAsJson<T>(T obj, FileInfo file)
        {
            if (file.Exists)
            {
                file.Delete();
            }

            DataContractJsonSerializer jsonSer = new DataContractJsonSerializer(typeof(T));
            FileStream fs = new FileStream(file.FullName, FileMode.Create);

            jsonSer.WriteObject(fs, obj);

            fs.Close();
        }

        public static T DeserializeFromJson<T>(FileInfo file)
        {
            T obj;

            DataContractJsonSerializer jsonSer = new DataContractJsonSerializer(typeof(T));
            FileStream fs = new FileStream(file.FullName, FileMode.Open);

            obj = (T)jsonSer.ReadObject(fs);

            fs.Close();

            return obj;
        }

        public static T DeserializeFromJson<T>(string json)
        {
            T obj;

            MemoryStream ms = new MemoryStream(Encoding.Default.GetBytes(json));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));

            obj = (T)ser.ReadObject(ms);

            return obj;
        }

}
}
