using System.ComponentModel.DataAnnotations;

namespace PustokTask.Models
{
    public class Setting 
    {
        public int Id { get; set; } 
        [Key]
        public string Key { get; set; }
        public string Value { get; set; }

    }
}
