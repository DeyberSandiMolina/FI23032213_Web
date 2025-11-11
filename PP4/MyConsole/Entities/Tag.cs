using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace MyConsole.Entities
{
    public class Tag
    {
        [Key]
        public int TagId { get; set; }

        [Required]
        public string TagName { get; set; } = null!;

        public ICollection<TitleTag> TitleTags { get; set; } = new List<TitleTag>();
    }
}
