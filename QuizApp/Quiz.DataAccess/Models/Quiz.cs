using System.ComponentModel.DataAnnotations;

namespace Quiz.DataAccess.Models;

public class Quiz
{
    [Key] public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    [MinLength(3)]
    public string Title { get; set; }

    public int CreatedByUserId { get; set; }
    public AppUser? CreatedByUser { get; set; }

    public bool IsPublished { get;  set; }

    public ICollection<Question> Questions { get; set; } = new List<Question>();

}