using CargoWise.eHub.Common;
using CargoWise.eHub.Common.Extensions;
using System;
using System.IO;
using System.ServiceModel;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using Microsoft.Extensions.Options;
using Microsoft.CodeAnalysis;
using System.Reflection.Metadata;
using System.ServiceModel.Channels;
using System.Text;
using System.Data.SqlTypes;
using System.Xml.Xsl;
using System.Net.Mail;
using Renci.SshNet;

namespace CargoWise.eAdaptorSampleWebService
{
	public class eAdaptorSampleWebService : IeAdapterStreamedService
	{
		public bool Ping()
		{
			return true;
		}
        public static string transformedJsonGlobal = string.Empty;

        public async void SendStream(SendStreamRequest request)
		{
            string senderId = OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name;

            foreach (var message in request.Messages)
			{
                // Load the JSON file
                var jsonText = File.ReadAllText(@"C:\eAdaptorSampleService\CWPartnerIDLookup.json");
                var jsonRows = JsonConvert.DeserializeObject<JArray>(jsonText);

                // get the current local date and time
                DateTime localDateTime = DateTime.UtcNow;

                // convert the local date and time to UTC
                DateTime utcDateTime = localDateTime.ToUniversalTime();

                // format the UTC date and time as a string
                string utcDateTimeString = utcDateTime.ToString("yyyyMMdd_HHmmss");

                // construct the prefix for the output file name
                string prefix = utcDateTimeString + "_";

                // Decode and decompress the message stream
                var decodedStream = message.MessageStream.DecodeAndDecompress();

                // Load the decoded and decompressed data as an XML document
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(decodedStream);



                // Convert the XmlDocument to XDocument
                XDocument xDoc = XDocument.Parse(xmlDoc.OuterXml);

                // Define the XML namespace
                XNamespace ns = "http://www.cargowise.com/Schemas/Universal/2011/11";

                // Find the RecipientID element
                XElement recipientIdElement = xDoc.Root.Element(ns + "Header")
                                                      .Element(ns + "RecipientID");

                // Find the RecipientID element
                XElement senderIdElement = xDoc.Root.Element(ns + "Header")
                                                      .Element(ns + "SenderID");

                // Get the value of the RecipientID element
                string recipientId = recipientIdElement.Value;
                // Get the value of the RecipientID element
                string senderId2 = senderIdElement.Value;


                // Find all rows in the JSON file that match the specified LEID
                var matchingRows = jsonRows
                    .Where(row =>
                        row.Value<string>("RecipientID") == recipientId // Check the PartnerID field
                    );

                // If any matching rows were found, create XML elements for them and add them to the JSON XML
                foreach (var matchingRow in matchingRows)
                {
                    XElement jsonXml = new XElement("row",
                        new XElement("PartnerID", matchingRow.Value<string>("PartnerID")),
                        new XElement("EnterpriseID", matchingRow.Value<string>("EnterpriseID")),
                        new XElement("CompanyID", matchingRow.Value<string>("CompanyID")),
                        new XElement("ServerID", matchingRow.Value<string>("ServerID")),
                        new XElement("eAdaptorInboundURL", matchingRow.Value<string>("eAdaptorInboundURL")),
                        new XElement("User", matchingRow.Value<string>("User")),
                        new XElement("Password", matchingRow.Value<string>("Password")),
                        new XElement("RecipientID", matchingRow.Value<string>("RecipientID")),
                        new XElement("TargetSystem", matchingRow.Value<string>("TargetSystem")),
                        new XElement("FTPHost", matchingRow.Value<string>("FTPHost")),
                        new XElement("FTPUser", matchingRow.Value<string>("FTPUser")),
                        new XElement("FTPPass", matchingRow.Value<string>("FTPPass")),
                        new XElement("FTPInputFolder", matchingRow.Value<string>("FTPInputFolder")),
                        new XElement("FTPOutputFolder", matchingRow.Value<string>("FTPOutputFolder")),
                        new XElement("TradechainURL", matchingRow.Value<string>("TradechainURL")),
                        new XElement("InternalRecipientFolder", matchingRow.Value<string>("InternalRecipientFolder")),
                        new XElement("LEID", matchingRow.Value<string>("LEID"))
                    );
                    // get the PartnerID and EnterpriseID values from the matching row
                    string partnerID = matchingRow.Value<string>("PartnerID");
                    string enterpriseID = matchingRow.Value<string>("EnterpriseID");
                    string companyID = matchingRow.Value<string>("CompanyID");
                    string serverID = matchingRow.Value<string>("ServerID");
                    string eAdaptorInboundURL = matchingRow.Value<string>("eAdaptorInboundURL");
                    string user = matchingRow.Value<string>("User");
                    string password = matchingRow.Value<string>("Password");
                    string recipientID = matchingRow.Value<string>("RecipientID");
                    string targetSystem = matchingRow.Value<string>("TargetSystem");
                    string ftpHost = matchingRow.Value<string>("FTPHost");
                    string ftpUser = matchingRow.Value<string>("FTPUser");
                    string ftpPass = matchingRow.Value<string>("FTPPass");
                    string ftpInputFolder = matchingRow.Value<string>("FTPInputFolder");
                    string ftpOutputFolder = matchingRow.Value<string>("FTPOutputFolder");
                    string tradechainURL = matchingRow.Value<string>("TradechainURL");
                    string leid = matchingRow.Value<string>("LEID");



                    prefix += partnerID + "_" + enterpriseID + "_" + companyID + "_" + serverID + "_" + message.MessageTrackingID + "_";

                    string destinationFolder = matchingRow.Value<string>("InternalRecipientFolder");


                    Validate(destinationFolder);

                    string fileName = string.Format("Message_{1}_{0}.xml", message.MessageTrackingID, senderId2);

                    using (var fileStream = new FileStream(Path.Combine(destinationFolder, fileName), FileMode.Create))
                    {
                        message.MessageStream.DecodeAndDecompress().WriteTo(fileStream);
                    }

                    // load the XML string into an XElement object
                    XElement xml = XElement.Parse(xmlDoc.OuterXml);

                    // convert the xml XElement to a JSON string
                    string xmlJson = JsonConvert.SerializeXNode(xml, Newtonsoft.Json.Formatting.None, false);

                    // parse the JSON string into a JObject
                    JObject xmlObj = JObject.Parse(xmlJson);

                    // serialize the JObject to a JSON string
                    string jsonStr = xmlObj.ToString();

                    // get the ForwardingConsol DataSource
                    XElement forwardingConsol = xml.Descendants("{http://www.cargowise.com/Schemas/Universal/2011/11}DataSource")
                                                    .FirstOrDefault(x => x.Element("{http://www.cargowise.com/Schemas/Universal/2011/11}Type")?.Value == "ForwardingConsol");
                    
                    // Save the transformed XML to a file
                    string fileName3 = prefix + "CargoWiseXMLtoLogisysXMLoutput.xml";
                    string key = "";
                    if (forwardingConsol != null)
                    {
                        // get the Key value for the ForwardingConsol DataSource
                        key = forwardingConsol.Element("{http://www.cargowise.com/Schemas/Universal/2011/11}Key")?.Value;

                        /*// Create an XmlResolver with default credentials
                        XmlUrlResolver resolver = new XmlUrlResolver();
                        resolver.Credentials = System.Net.CredentialCache.DefaultCredentials;

                        // Create an XmlReaderSettings object with the resolver
                        XmlReaderSettings readerSettings = new XmlReaderSettings();
                        readerSettings.XmlResolver = resolver;
                        readerSettings.ValidationType = ValidationType.None;
                        readerSettings.DtdProcessing = DtdProcessing.Parse;

                        var xsltText = File.ReadAllText(@"C:\eAdaptorSampleService\CWConsolShipmentToLSShipment.xslt");
                       
                        // Create an XslCompiledTransform and load the XSLT from xsltText
                        XslCompiledTransform xslt = new XslCompiledTransform();
                        XsltSettings sets = new XsltSettings(true, true);
                        using (XmlReader reader = XmlReader.Create(new StringReader(xsltText), readerSettings))
                        {
                            xslt.Load(reader, sets, resolver);
                        }

                        // Create a StringWriter to store the transformed XML
                        StringWriter sw = new StringWriter();

                        // Transform the XML using the XSLT stylesheet and write it to the StringWriter
                        using (XmlReader reader = XmlReader.Create(Path.Combine(destinationFolder, fileName), readerSettings))
                        {
                            xslt.Transform(reader, null, sw);
                        }

                        // Create a new XDocument from the transformed XML
                        XDocument transformedXml = XDocument.Parse(sw.ToString());

                        // Convert XDocument to JObject
                        JObject jobject = JObject.FromObject(transformedXml);

                        // Convert JObject to formatted JSON string
                        string jsonString = jobject.ToString(Newtonsoft.Json.Formatting.Indented);

                        // Convert the transformed XML to JSON
                        string transformedJson = JsonConvert.SerializeXNode(transformedXml);

                        transformedJsonGlobal = transformedJson;

                        // Create a new JObject from the transformed JSON
                        JObject transformedObject = JObject.Parse(transformedJson);

                        
                        File.WriteAllText(Path.Combine(destinationFolder, fileName3), jsonString);
                        Console.WriteLine($"CargoWise XML file has been converted to a Logisys JSON and output file is in {destinationFolder}"); */
                    }

                    if (targetSystem == "FTP")
                    {
                        Console.WriteLine($"Sending to FTP...");

                        string FTPprefix = utcDateTimeString + "_" + recipientId + "_" + enterpriseID + companyID + serverID;

                        try
                        {
                            // Get the file name from the archive file path
                            string FTPfileName = FTPprefix + "_" + message.MessageTrackingID;

                            // Create a WebClient object
                            using (WebClient client = new WebClient())
                            {
                                // Set the FTP credentials
                                client.Credentials = new NetworkCredential(ftpUser, ftpPass);

                                // Set the FTP folder paths with default values if not specified
                                string inputFolder = string.IsNullOrEmpty(ftpInputFolder) ? "ToLogisys/FromCW" : ftpInputFolder;
                                string outputFolder = string.IsNullOrEmpty(ftpOutputFolder) ? "ToLogisys" : ftpOutputFolder;

                                // Split the FTP host string into host and port
                                string[] hostParts = ftpHost.Split(':');
                                string ftpAddress = hostParts[0];
                                int ftpPort = hostParts.Length > 1 ? int.Parse(hostParts[1]) : 21;

                                // Create an FTP request to login to the server
                                string ftpServerUrl = $"ftp://{ftpAddress}:{ftpPort}/";
                                FtpWebRequest request2 = (FtpWebRequest)WebRequest.Create(ftpServerUrl);
                                request2.Method = WebRequestMethods.Ftp.ListDirectory;
                                request2.Credentials = new NetworkCredential(ftpUser, ftpPass);

                                try
                                {
                                    // Send the request to the server and get the response
                                    FtpWebResponse response = (FtpWebResponse)request2.GetResponse();

                                    // Write a log message to indicate successful login
                                    Console.WriteLine("Logged in to FTP server {0}:{1}", ftpAddress, ftpPort);

                                    // Close the response stream
                                    response.Close();
                                }
                                catch (WebException ex)
                                {
                                    // Handle any errors that occur during the login process
                                    Console.WriteLine("Error logging in to FTP server {0}:{1}: {2}", ftpAddress, ftpPort, ex.Message);
                                }


                                // Build the full FTP paths for input and output files
                                string ftpInputPath = $"ftp://{ftpAddress}:{ftpPort}/{inputFolder}/{FTPfileName}_input.xml";
                                string fullPath = Path.Combine(destinationFolder, fileName);

                                // Build the full FTP paths for input and output files
                                //string ftpOutputPath = $"ftp://{ftpAddress}:{ftpPort}/{outputFolder}/{FTPfileName}_input.json";
                                //string fullPath3 = Path.Combine(destinationFolder, fileName3);

                                // Upload the files to the FTP server
                                client.UploadFile(ftpInputPath, "STOR", fullPath);
                                Console.WriteLine($"File {FTPfileName}_input.xml uploaded to FTP server.");

                                // Upload the files to the FTP server
                                //client.UploadFile(ftpInputPath, "STOR", fullPath3);
                                //Console.WriteLine($"File {FTPfileName}_output.json uploaded to FTP server.");

                            }


                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error uploading file to FTP server: {ex.Message}");
                        }


                    }
                    else if (targetSystem == "SFTP")
                    {
                        Console.WriteLine($"Sending to SFTP...");

                        string SFTPprefix = utcDateTimeString + "_" + recipientId + "_" + enterpriseID + companyID + serverID;

                        try
                        {
                            // Set the SFTP folder paths with default values if not specified
                            string inputFolder = string.IsNullOrEmpty(ftpInputFolder) ? "/ToLogisys/FromCW" : ftpInputFolder;
                            string outputFolder = string.IsNullOrEmpty(ftpOutputFolder) ? "/ToLogisys" : ftpOutputFolder;

                            // Split the SFTP host string into host and port
                            string[] hostParts = ftpHost.Split(':');
                            string ftpAddress = hostParts[0];
                            int ftpPort = hostParts.Length > 1 ? int.Parse(hostParts[1]) : 22;

                            // Create a new SSH client and connect to the server
                            using (var client = new SftpClient(ftpAddress, ftpPort, ftpUser, ftpPass))
                            {
                                client.Connect();

                                // Write a log message to indicate successful login
                                Console.WriteLine("Logged in to SFTP server {0}:{1}", ftpAddress, ftpPort);

                                // Build the full SFTP paths for input and output files
                                string ftpInputPath = inputFolder + "/" + SFTPprefix + "_" + message.MessageTrackingID + "_input.xml";
                                string fullPath = Path.Combine(destinationFolder, fileName);

                                // Upload the input file to the SFTP server
                                using (var stream = File.OpenRead(fullPath))
                                {
                                    client.UploadFile(stream, ftpInputPath);
                                    Console.WriteLine($"File {SFTPprefix}_{message.MessageTrackingID}_input.xml uploaded to SFTP server.");
                                }

                                /*// Build the full SFTP paths for output files
                                string ftpOutputPath = outputFolder + "/" + SFTPprefix + "_" + message.MessageTrackingID + "_output.json";
                                string fullPath3 = Path.Combine(destinationFolder, fileName3);

                                // Upload the output file to the SFTP server
                                using (var stream = File.OpenRead(fullPath3))
                                {
                                    client.UploadFile(stream, ftpOutputPath);
                                    Console.WriteLine($"File {SFTPprefix}_{message.MessageTrackingID}_output.json uploaded to SFTP server.");
                                }*/

                                client.Disconnect();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error uploading file to SFTP server: {ex.Message}");
                        }
                    }
                    else if (targetSystem == "TC")
                    {
                        Console.WriteLine($"Sending to Tradechain...");

                        string messageId = new string(key.Where(char.IsDigit).ToArray());

                        var message2 = new RecievedMessage()
                        {
                            Action = "",
                            /*Payload = new Payload()
                            {
                                ContentType = "application/json",
                                ContentData = jsonStr
                            },*/
                            Payload = jsonStr,
                            Sender = leid,
                            ServiceCode = "FFA2ANATIVE",
                            MessageId = messageId
                        };

                        string messageJson = JsonConvert.SerializeObject(message2);

                        ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;



                        using (HttpClient client = new HttpClient())
                        {
                            var content = new StringContent(messageJson, Encoding.UTF8, "application/json");
                            var response = await client.PostAsync(tradechainURL, content);

                            if (response.IsSuccessStatusCode)
                            {
                                Console.WriteLine("Sending to Tradechain was successful.");
                            }
                            else
                            {
                                Console.WriteLine($"Sending to Tradechain failed with status code {response.StatusCode}.");
                                // handle error
                            }
                        }
                        /*RecievedMessage message2 = new RecievedMessage()

                        {
                            Action = "",
                            Payload = new Payload()
                            {
                                ContentType = "application/json",
                                ContentData = decodedStream
                            },
                            Sender = leid,
                            ServiceCode = "FFA2ANATIVE",
                            MessageId = messageId
                        };

                        string messageJson = JsonConvert.SerializeObject(message2);

                        using (HttpClient client = new HttpClient())
                        {
                            var content = new StringContent(messageJson, Encoding.UTF8, "application/json");
                            var response = await client.PostAsync("TradechainURL", content);

                            if (response.IsSuccessStatusCode)
                            {
                                // do something if the call is successful
                            }
                            else
                            {
                                // handle error
                            }
                        }*/


                    }
                }




            }
		}

		public void Validate(string destinationFolder)
		{
			if ((destinationFolder == "")) throw new FaultException("No destination folder for received messages has been specified. Please modify the eAdaptorSampleWebService.cs file located within the eAdaptor solution and specify a destination folder for received messages.");
		}
	}
}
