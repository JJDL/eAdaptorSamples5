using Newtonsoft.Json;

public class Payload
{
    [JsonProperty("FileName")]
    public string FileName { get; set; }

    [JsonProperty("ContentType")]
    public string ContentType { get; set; }

    [JsonProperty("ContentData")]
    public string ContentData { get; set; }
}

public class SendMessage
{
    [JsonProperty("Sender")]
    public string Sender { get; set; }

    [JsonProperty("Receiver")]
    public string Receiver { get; set; }

    [JsonProperty("DeliveryMethod")]
    public string DeliveryMethod { get; set; }

    [JsonProperty("ServiceCode")]
    public string ServiceCode { get; set; }

    [JsonProperty("ServiceAction")]
    public string ServiceAction { get; set; }

    [JsonProperty("Published")]
    public bool Published { get; set; }

    [JsonProperty("MaxRetryCount")]
    public int MaxRetryCount { get; set; }

    [JsonProperty("TimeToLive")]
    public string TimeToLive { get; set; }

    [JsonProperty("RefNo")]
    public string RefNo { get; set; }

    [JsonProperty("Remark")]
    public string Remark { get; set; }

    [JsonProperty("RefMessageId")]
    public string RefMessageId { get; set; }

    [JsonProperty("Payload")]
    public Payload Payload { get; set; }
}
