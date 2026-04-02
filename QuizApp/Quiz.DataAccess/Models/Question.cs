using System.ComponentModel.DataAnnotations;

namespace Quiz.DataAccess.Models;

public class Question
{
    [Key] public int Id { get; set; }

    public int QuizId { get; set; } 
    public Quiz? Quiz { get; set; }

    [Required]
    [MinLength(3)]
    [MaxLength(255)]
    public string Text { get; set; } = "";

    [Range(1, int.MaxValue)]
    public int OrderIndex { get; set; }

    public int TimeLimitSeconds { get; set; } = 10;

    public ICollection<AnswerOption> AnswerOptions { get; set; } = new List<AnswerOption>();


}