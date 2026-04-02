using System.ComponentModel.DataAnnotations;

namespace Quiz.DataAccess.Models;

public class AnswerOption
{
    [Key] public int Id { get; set; }
    
    public int QuestionId { get; set; }
    public Question? Question { get; set; }

    [Required]
    [MinLength(1)]
    [MaxLength(255)]
    public string Text { get; set; } = "";
    
    [Range(0, int.MaxValue)]
    public int OrderIndex { get; set; }
    
    public bool IsCorrect { get; set; }
}