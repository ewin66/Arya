
        using System.IO;
using System.Xml;
using System.Xml.Xsl;
public class XsltTransformer
        {
            private readonly XslCompiledTransform xslTransform;

            public XsltTransformer(string xsl)
            {
                xslTransform = new XslCompiledTransform();

                using (var stringReader = new StringReader(xsl))
                {
                    using (var xslt = XmlReader.Create(stringReader))
                    {
                        xslTransform.Load(xslt);
                    }
                }
            }

            public string Transform(object report)
            {
                var xml = new XmlSerializerHelper().Serialize(report);

                using (var writer = new StringWriter())
                {
                    using (var input = XmlReader.Create(new StringReader(xml)))
                    {
                        xslTransform.Transform(input, null, writer);
                        return writer.ToString();
                    }
                }
            }