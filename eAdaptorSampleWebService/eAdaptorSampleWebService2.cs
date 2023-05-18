using CargoWise.eHub.Common;
using CargoWise.eHub.Common.Extensions;
using System.IO;
using System.ServiceModel;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using System.Xml;
using System.Xml.Xsl;
using System.Text;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CargoWise.eAdaptorSampleWebService
{
	public class eAdaptorSampleWebService : IeAdapterStreamedService
	{
		public bool Ping()
		{
			return true;
		}

		public void SendStream(SendStreamRequest request)
		{
			string senderId = OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name;

			string destinationFolder = @"C:\eAdaptorSampleService";
#warning Please specify a destination folder for received messages. Make sure the folder exists on the machine that will run your eAdaptor Sample Web Service. You can then remove this warning.

			Validate(destinationFolder);

            foreach (var message in request.Messages)
            {
                string fileName = string.Format("Message_{1}_{0}.xml", message.MessageTrackingID, senderId);
                string filePath = Path.Combine(destinationFolder, fileName);

                try
                {
                    // Load the message stream into an XML document
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(message.MessageStream.DecodeAndDecompress());

                    // Remove namespaces
                    XmlNamespaceManager namespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
                    namespaceManager.AddNamespace(string.Empty, string.Empty);
                    XmlNodeList nodesWithNamespace = xmlDocument.SelectNodes("//*[namespace-uri()!='']", namespaceManager);
                    foreach (XmlNode node in nodesWithNamespace)
                    {
                        node.Attributes.RemoveAll();
                        XmlNode newNode = xmlDocument.CreateNode(XmlNodeType.Element, node.LocalName, null);
                        newNode.InnerXml = node.InnerXml;
                        node.ParentNode.ReplaceChild(newNode, node);
                    }

                    // Remove the XML declaration
                    XmlProcessingInstruction xmlDeclaration = (XmlProcessingInstruction)xmlDocument.SelectSingleNode("processing-instruction('xml')");
                    if (xmlDeclaration != null)
                    {
                        xmlDocument.RemoveChild(xmlDeclaration);
                    }

                    // Retrieve the DataProvider value
                    string dataProvider = xmlDocument.SelectSingleNode("//DataProvider")?.InnerText;
                    if (!string.IsNullOrEmpty(dataProvider))
                    {
                        Console.WriteLine($"DataProvider value: {dataProvider}");
                    }

                    // Save the modified XML to a new file
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        xmlDocument.Save(fileStream);
                    }

                    Console.WriteLine($"Successfully modified and saved {filePath}");



                    // Create an XmlResolver with default credentials
                    XmlUrlResolver resolver = new XmlUrlResolver();
                    resolver.Credentials = System.Net.CredentialCache.DefaultCredentials;

                    // Create an XmlReaderSettings object with the resolver
                    XmlReaderSettings readerSettings = new XmlReaderSettings();
                    readerSettings.XmlResolver = resolver;
                    readerSettings.ValidationType = ValidationType.None;
                    readerSettings.DtdProcessing = DtdProcessing.Parse;

                    // Load the XSLT file
                    XslCompiledTransform xslt = new XslCompiledTransform();
                    XsltSettings sets = new XsltSettings(true, true);
                    using (XmlReader reader = XmlReader.Create(@"CWConsolShipmentToCreateLSShipment.xslt", readerSettings))
                    {
                        xslt.Load(reader, sets, resolver);
                    }

                    // Create a StringWriter to store the transformed XML
                    StringWriter sw = new StringWriter();

                    // Transform the XML using the XSLT stylesheet and write it to the StringWriter
                    using (XmlReader reader = new XmlNodeReader(xmlDocument))
                    {
                        xslt.Transform(reader, null, sw);
                    }

                    // Create a new XDocument from the transformed XML
                    XDocument transformedXml = XDocument.Parse(sw.ToString());

                    // Save the transformed XML to a file
                    transformedXml.Save(@"C:\eAdaptorSampleService\output\CargoWiseXMLtoLogisysXMLoutput.xml");
                    Console.WriteLine($"CargoWise XML file has been converted to a Logisys XML and output file is in CargoWiseXMLtoLogisysXMLoutput.xml");

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to modify and save {filePath}: {ex.Message}");
                    Console.WriteLine($"Error: {ex.Message}");
                }

            }
        }

		public void Validate(string destinationFolder)
		{
			if ((destinationFolder == "")) throw new FaultException("No destination folder for received messages has been specified. Please modify the eAdaptorSampleWebService.cs file located within the eAdaptor solution and specify a destination folder for received messages.");
		}
	}
}
