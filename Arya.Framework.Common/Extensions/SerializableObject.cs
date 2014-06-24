using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;

namespace Arya.Framework.Extensions
{
    // This class acts as a serializable wrapper for any arbitrary object of type T.
    [Serializable]
    public class SerializableObject<T>
    {
        #region Properties
        private T _dataClass;
        public T DataClass
        {
            get { return _dataClass; }
            set { _dataClass = value; }
        }
        #endregion

        #region Constructors
        public SerializableObject()
        {
            // Instantiating this class without a parameter sets the DataClass 
            // to the default of the type parameter (usually null). T must be set
            // on instantiation.
            _dataClass = default(T);
        }

        public SerializableObject(T dataObject)
        {
            // Instantiating this class with a parameter and a matching data type (T)
            // sets the enclosed DataType to that object.
            _dataClass = dataObject;
        }
        #endregion

        #region Public Methods
        public T LoadFromXml(string fileName)
        {
            // This method reads an XML file into an object of type T,
            // optionally returning it directly. The type's default is returned if the
            // XML file does not match the declared class.
            if (File.Exists(fileName))
            {
                using (Stream stream = File.OpenRead(fileName))
                {
                    var deserializer = new SoapFormatter();
                    try
                    {
                        _dataClass = (T) deserializer.Deserialize(stream);
                    }
                    catch
                    {
                        _dataClass = default(T);
                    }
                }
            }
            return _dataClass;
        }

        public void SaveToXml(string fileName)
        {
            // this method write an XML file for the enclosing object.
            using (Stream stream = File.Create(fileName))
            {
                var serializer = new SoapFormatter();
                serializer.Serialize(stream, _dataClass);
            }
        }

        public T LoadFromBinary(string fileName)
        {
            // This method reads an binary file into an object of type T,
            // optionally returning it directly. The type's default is returned if the
            // binary file does not match the declared class.
            if (File.Exists(fileName))
            {
                using (Stream stream = File.OpenRead(fileName))
                {
                    var deserializer = new BinaryFormatter();
                    try
                    {
                        _dataClass = (T)deserializer.Deserialize(stream);
                    }
                    catch
                    {
                        _dataClass = default(T);
                    }
                }
            }
            return _dataClass;
        }

        public void SaveToBinary(string fileName)
        {
            // this method write a binary file for the enclosing object.
            using (Stream stream = File.Create(fileName))
            {
                var serializer = new BinaryFormatter();
                serializer.Serialize(stream, _dataClass);
            }
        }
        #endregion
    }
}
