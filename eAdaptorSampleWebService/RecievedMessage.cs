namespace CargoWise.eAdaptorSampleWebService
{
    public sealed class RecievedMessage
    {
        public string Action { get; set; } = string.Empty;
        public dynamic Payload { get; set; } = string.Empty;
        public string Sender { get; set; } = string.Empty;
        public string ServiceCode { get; set; } = string.Empty;
        public string MessageId { get; set; } = string.Empty;
    }

    public sealed class Payload
    {
        public string ContentType { get; set; } = string.Empty;

        public dynamic ContentData { get; set; } = string.Empty;
    }

    public sealed class LogiSysShipment
    {
        public dynamic DeliveryOptions { get; set; } = string.Empty;

        public dynamic Data { get; set; } = string.Empty;
    }
}