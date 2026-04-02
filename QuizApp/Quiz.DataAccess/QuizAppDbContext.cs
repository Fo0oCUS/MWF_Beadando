using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Quiz.DataAccess.Models;

namespace Quiz.DataAccess.Services;

public class QuizAppDbContext : IdentityDbContext<AppUser, UserRole, string>
{
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Models.Quiz> Quizzes { get; set; }
    public DbSet<AnswerOption> AnswerOptions { get; set; }
    public DbSet<SessionParticipant> SessionParticipants { get; set; }
    public DbSet<ParticipantAnswer> ParticipantAnswers { get; set; }
    public DbSet<QuizSession> QuizSessions { get; set; }
    
    public QuizAppDbContext(DbContextOptions<QuizAppDbContext> options) : base(options){}
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<Question>().HasIndex(x => new { x.QuizId, x.OrderIndex }).IsUnique();

        builder.Entity<Models.Quiz>().HasOne(x => x.CreatedByUser)
            .WithMany(x=>x.CreatedQuizzes)
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Models.Quiz>().HasMany(x => x.Questions)
            .WithOne(x => x.Quiz)
            .HasForeignKey(x => x.QuizId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Question>().HasMany<AnswerOption>(x => x.AnswerOptions)
            .WithOne(x => x.Question)
            .HasForeignKey(x => x.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<AnswerOption>().HasIndex(x => new { x.QuestionId, x.OrderIndex }).IsUnique();

        builder.Entity<QuizSession>().HasIndex(x => x.JoinCode).IsUnique();
        
        builder.Entity<QuizSession>()
            .HasOne(x => x.Quiz)
            .WithMany()
            .HasForeignKey(x => x.QuizId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.Entity<SessionParticipant>()
            .HasOne(x => x.QuizSession)
            .WithMany(x => x.Participants)
            .HasForeignKey(x => x.QuizSessionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Entity<SessionParticipant>()
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.Entity<ParticipantAnswer>()
            .HasOne(x => x.SessionParticipant)
            .WithMany(x => x.ParticipantAnswers)
            .HasForeignKey(x => x.SessionParticipantId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Entity<ParticipantAnswer>()
            .HasOne(x => x.Question)
            .WithMany()
            .HasForeignKey(x => x.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<ParticipantAnswer>()
            .HasOne(x => x.AnswerOption)
            .WithMany()
            .HasForeignKey(x => x.AnswerOptionId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<ParticipantAnswer>()
            .HasIndex(x => new { x.SessionParticipantId, x.QuestionId })
            .IsUnique();
        
        builder.Entity<SessionParticipant>()
            .HasIndex(x => new { x.QuizSessionId, x.Nickname })
            .IsUnique();
    }
}