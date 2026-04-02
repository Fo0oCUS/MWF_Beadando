using System.ComponentModel.DataAnnotations;

namespace Quiz.DataAccess.Models;

public class AppUser
{
    [Key] public int Id { get; set; }
    
    [MaxLength(20)]
    [MinLength(3)]
    public string UserName { get; set; }
    
    [EmailAddress]
    public string Email { get; set; }
    
    public ICollection<Quiz> CreatedQuizzes { get; set; } = new List<Quiz>();
}