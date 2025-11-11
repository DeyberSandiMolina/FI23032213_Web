using System.ComponentModel.DataAnnotations;

namespace MyConsole.Entities
{
    public class TitleTag
    {
        [Key]
        public int TitleTagId { get; set; }

        [Required]
        public int TitleId { get; set; }

        [Required]
        public int TagId { get; set; }

        public Title Title { get; set; } = null!;
        public Tag Tag { get; set; } = null!;
    }
}
