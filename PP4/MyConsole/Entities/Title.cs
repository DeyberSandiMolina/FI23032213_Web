using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace MyConsole.Entities
{
    public class Title
    {
        [Key]
        public int TitleId { get; set; }

        [Required]
        public string TitleName { get; set; } = null!;

        [Required]
        public int AuthorId { get; set; }

        public Author Author { get; set; } = null!;

        public ICollection<TitleTag> TitleTags { get; set; } = new List<TitleTag>();
    }
}
