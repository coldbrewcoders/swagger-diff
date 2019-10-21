using System.ComponentModel.DataAnnotations;

namespace SwaggerDiff.Models
{
    public class SwaggerItem
    {
        public SwaggerItem(string serviceName, string serviceJSON)
        {
            ServiceName = serviceName;
            ServiceJSON = serviceJSON;
        }

        [Key]
        [Required]
        public string ServiceName { get; set; }

        [Required]
        public string ServiceJSON { get; set; }
    }
}
