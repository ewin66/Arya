using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Arya.HelperClasses
{
	public static class XmlOps
	{
		#region Methods (4) 

		// Public Methods (4) 

		public static CrossListCriteria DeSerialize(string InputFileName)
		{
			var ReturnTask = new CrossListCriteria();
			var oSerialiser = new XmlSerializer(typeof (CrossListCriteria));
			Stream oStream = new FileStream(InputFileName, FileMode.Open, FileAccess.Read);
			try
			{
				ReturnTask = (CrossListCriteria) oSerialiser.Deserialize(oStream);
			}
			catch
			{
				MessageBox.Show("Nothin' in file mate!");
			}
			oStream.Close();
			return ReturnTask;
		}

		public static CrossListCriteria DeSerializeXElement(this XElement el)
		{
			var serializer = new XmlSerializer(typeof (CrossListCriteria));
			return (CrossListCriteria) serializer.Deserialize(el.CreateReader());
		}

		public static void Serialize(CrossListCriteria cl, string outputFileName)
		{
			var oSerialiser = new XmlSerializer(typeof (CrossListCriteria));
			Stream oStream = new FileStream(outputFileName, FileMode.Truncate, FileAccess.ReadWrite);

			oSerialiser.Serialize(oStream, cl);
			oStream.Close();
		}


        public static WorkFlow DeSerializeWorkflow(this XElement el)
        {
            var serializer = new XmlSerializer(typeof(WorkFlow));
            return (WorkFlow)serializer.Deserialize(el.CreateReader());
        }

		public static XElement SerializeToXElement(this object cl)
		{
            if (cl == null) return null;
			var oSerialiser = new XmlSerializer(cl.GetType());

			var doc = new XDocument();

			using (XmlWriter xw = doc.CreateWriter())
			{
				oSerialiser.Serialize(xw, cl);
				xw.Close();
			}

			return doc.Root;
		}

		#endregion Methods 
	}
}