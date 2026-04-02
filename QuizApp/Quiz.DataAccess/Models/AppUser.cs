using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Quiz.DataAccess.Models;

public class AppUser : IdentityUser
{
    [Key] public string Id { get; set; }
    
    [MaxLength(20)]
    [MinLength(3)]
    public string UserName { get; set; }
    
    [EmailAddress]
    public string Email { get; set; }
    
    public ICollection<Quiz> CreatedQuizzes { get; set; } = new List<Quiz>();
}