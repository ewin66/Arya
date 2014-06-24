using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Arya.Framework.Common.Extensions
{
    public static class XmlSerializationHelper
    {
        /// <summary>
        /// To convert a Byte Array of Unicode values (UTF-8 encoded) to a complete String.
        /// </summary>
        /// <param name="characters">Unicode Byte Array to be converted to String</param>
        /// <returns>String converted from Unicode Byte Array</returns>
        private static string UTF8ByteArrayToString(byte[] characters)
        {
            var encoding = new UTF8Encoding();
            var constructedString = encoding.GetString(characters);
            return (constructedString);
        }

        /// <summary>
        /// Converts the String to UTF8 Byte array and is used in De serialization
        /// </summary>
        /// <param name="pXmlString"></param>
        /// <returns></returns>
        private static Byte[] StringToUTF8ByteArray(string pXmlString)
        {
            var encoding = new UTF8Encoding();
            var byteArray = encoding.GetBytes(pXmlString);
            return byteArray;
        }

        /// <summary>
        /// Serialize an object into an XML string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeObject<T>(this T obj)
        {
            try
            {
                var memoryStream = new MemoryStream();
                var xs = new XmlSerializer(typeof(T));
                var xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                xs.Serialize(xmlTextWriter, obj);
                memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                var xmlString = UTF8ByteArrayToString(memoryStream.ToArray());
                return xmlString;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static void SerializeObject<T>(this T obj, TextWriter file)
        {
            var xs = new XmlSerializer(typeof(T));
            xs.Serialize(file, obj);
        }

        public static void SerializeObject<T>(this T obj, string filename)
        {
            using (TextWriter file = new StreamWriter(filename))
                obj.SerializeObject(file);
        }

        /// <summary>
        /// Reconstruct an object from an XML string
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T DeserializeObject<T>(this string xml)
        {
            var xs = new XmlSerializer(typeof(T));
            var memoryStream = new MemoryStream(StringToUTF8ByteArray(xml));
            return (T)xs.Deserialize(memoryStream);
        }

        public static XElement SerializeToXElement<T>(this T source)
        {
            var target = new XDocument();
            var s = new XmlSerializer(typeof(T));
            XmlWriter writer = target.CreateWriter();
            s.Serialize(writer, source);
            writer.Close();
            return target.Root;
        }

        public static T DeserializeFromXElement<T>(this XElement source)
        {
            var target = new XmlSerializer(typeof(T));
            return (T)target.Deserialize(source.CreateReader());
        }
    }
}
