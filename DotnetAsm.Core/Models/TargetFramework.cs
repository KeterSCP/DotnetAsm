using System.Text.Json.Serialization;

namespace DotnetAsm.Core.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TargetFramework
{
    Net70,
    Net80,
}