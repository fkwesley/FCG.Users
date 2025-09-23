using System.Text.Json.Serialization;

namespace FCG.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum LogLevel
    {
        Trace,
        Debug,
        Info,
        Warning,
        Error,
        Critical
    }
}
