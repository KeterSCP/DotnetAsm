using System.ComponentModel.DataAnnotations;

namespace DotnetAsm.Core.ConfigOptions;

public class CodeWriterSettings
{
    public const string SectionName = "CodeWriter";

    [Required]
    public required string WritePath { get; init; }
}
