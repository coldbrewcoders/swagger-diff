using System.ComponentModel.DataAnnotations;

namespace SwaggerDiff.Models
{
  public class SwaggerItem
  {
    [Required]
    public long Id { get; set; }

    [Required]
    public string ServiceName { get; set; }

    [Required]
    public string ServiceJSON { get; set; }
  }
}
