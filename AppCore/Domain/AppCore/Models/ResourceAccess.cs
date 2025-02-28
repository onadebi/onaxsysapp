using AppCore.Domain.AppCore.Config;
using AppCore.Domain.AppCore.Models.Extensions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppCore.Domain.AppCore.Models;

[Table(nameof(ResourceAccess), Schema =SchemaConstants.AuthSchema)]
public class ResourceAccess: CommonProperties
{
    [Key]
    [Required]
    public Guid ResourceId { get; set; }

    [Required]
    public required string ResourceControllerName { get; set; }

    [Required]
    public required string ResourceFunctionName{ get; set; }

    [Required]
    public required string ResourceFullName { get; set; }

    public string? DisplayName { get; set; }

    [Required]
    public required string AllowedRoles { get; set; }

    #region Navigational Properties

    #endregion

}
