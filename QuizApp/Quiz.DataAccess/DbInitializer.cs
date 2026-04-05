using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quiz.DataAccess.Models;
using Quiz.DataAccess.Models.Enums;

namespace Quiz.DataAccess.Services
{
    public class DbInitializer
    {
        public static async Task Initialize(
            QuizAppDbContext context,
            RoleManager<UserRole>? roleManager = null,
            UserManager<AppUser>? userManager = null) {
            if (roleManager == null) throw new ArgumentNullException(nameof(roleManager));
            if (userManager == null) throw new ArgumentNullException(nameof(userManager));

            await SeedRolesAsync(roleManager);
            await SeedUsersAsync(userManager);

            var user1 = await userManager.FindByEmailAsync("user1@example.com");
            var user2 = await userManager.FindByEmailAsync("user2@example.com");

            if (user1 == null || user2 == null) {
                throw new Exception("Seed users were not created correctly.");
            }

            // Ha már van legalább egy quiz, nem seedelünk újra mindent
            if (await context.Quizzes.AnyAsync()) {
                return;
            }

            // QUIZZES
            var quiz1 = new Models.Quiz {
                Title = "Teszt Quiz",
                CreatedByUserId = user1.Id,
                IsPublished = true
            };

            var quiz2 = new Models.Quiz {
                Title = "Teszt Quiz2",
                CreatedByUserId = user2.Id,
                IsPublished = true
            };

            context.Quizzes.AddRange(quiz1, quiz2);
            await context.SaveChangesAsync();

            // QUESTIONS
            var q1 = new Question {
                QuizId = quiz1.Id,
                Text = "Mennyi 2 + 2?",
                OrderIndex = 0,
                TimeLimitSeconds = 10
            };

            var q2 = new Question {
                QuizId = quiz1.Id,
                Text = "Magyarország fővárosa?",
                OrderIndex = 1,
                TimeLimitSeconds = 10
            };

            var q3 = new Question {
                QuizId = quiz2.Id,
                Text = "Kék?",
                OrderIndex = 0,
                TimeLimitSeconds = 10
            };

            context.Questions.AddRange(q1, q2, q3);
            await context.SaveChangesAsync();

            // ANSWERS Q1
            var answersQ1 = new List<AnswerOption>
            {
                new() { QuestionId = q1.Id, Text = "3", OrderIndex = 1, IsCorrect = false },
                new() { QuestionId = q1.Id, Text = "4", OrderIndex = 2, IsCorrect = true },
                new() { QuestionId = q1.Id, Text = "5", OrderIndex = 3, IsCorrect = false },
                new() { QuestionId = q1.Id, Text = "6", OrderIndex = 4, IsCorrect = false }
            };

            // ANSWERS Q2
            var answersQ2 = new List<AnswerOption>
            {
                new() { QuestionId = q2.Id, Text = "Debrecen", OrderIndex = 1, IsCorrect = false },
                new() { QuestionId = q2.Id, Text = "Budapest", OrderIndex = 2, IsCorrect = true },
                new() { QuestionId = q2.Id, Text = "Szeged", OrderIndex = 3, IsCorrect = false },
                new() { QuestionId = q2.Id, Text = "Pécs", OrderIndex = 4, IsCorrect = false }
            };

            // ANSWERS Q3
            var answersQ3 = new List<AnswerOption>
            {
                new() { QuestionId = q3.Id, Text = "Nem", OrderIndex = 1, IsCorrect = false },
                new() { QuestionId = q3.Id, Text = "Nem", OrderIndex = 2, IsCorrect = false },
                new() { QuestionId = q3.Id, Text = "Igen", OrderIndex = 3, IsCorrect = true },
                new() { QuestionId = q3.Id, Text = "Nem", OrderIndex = 4, IsCorrect = false }
            };

            context.AnswerOptions.AddRange(answersQ1);
            context.AnswerOptions.AddRange(answersQ2);
            context.AnswerOptions.AddRange(answersQ3);
            await context.SaveChangesAsync();

            // SESSION
            var session = new QuizSession {
                QuizId = quiz1.Id,
                HostUserId = user1.Id,
                JoinCode = "ABC123",
                CurrentQuestionIndex = -1,
                QuizSessionStatus = QuizSessionStatus.Waiting
            };

            context.QuizSessions.Add(session);
            await context.SaveChangesAsync();

            // PARTICIPANTS
            var p1 = new SessionParticipant {
                QuizSessionId = session.Id,
                Nickname = "User1",
                UserId = user1.Id,
                IsHost = true
            };

            var p2 = new SessionParticipant {
                QuizSessionId = session.Id,
                UserId = user2.Id,
                Nickname = "User2",
                IsHost = false
            };

            context.SessionParticipants.AddRange(p1, p2);
            await context.SaveChangesAsync();

            // ANSWERS (simulate gameplay)
            var correctAnswerQ1 = await context.AnswerOptions
                .FirstAsync(a => a.QuestionId == q1.Id && a.IsCorrect);

            context.ParticipantAnswers.Add(new ParticipantAnswer {
                SessionParticipantId = p1.Id,
                QuestionId = q1.Id,
                AnswerOptionId = correctAnswerQ1.Id,
                IsCorrect = true,
                AwardedPoints = 100,
                ResponseTimeMs = 1500
            });

            context.ParticipantAnswers.Add(new ParticipantAnswer {
                SessionParticipantId = p2.Id,
                QuestionId = q1.Id,
                AnswerOptionId = answersQ1.First().Id,
                IsCorrect = false,
                AwardedPoints = 0,
                ResponseTimeMs = 3000
            });

            await context.SaveChangesAsync();
        }

        private static async Task SeedRolesAsync(RoleManager<UserRole> roleManager) {
            string[] roleNames = { "Admin", "User" };

            foreach (var roleName in roleNames) {
                var roleExists = await roleManager.RoleExistsAsync(roleName);

                if (!roleExists) {
                    await roleManager.CreateAsync(new UserRole(roleName));
                }
            }
        }

        private static async Task SeedUsersAsync(UserManager<AppUser> userManager) {
            const string adminEmail = "admin@example.com";
            const string adminPassword = "Admin123#";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null) {
                adminUser = new AppUser {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Name = "Test Admin",
                    RefreshToken = Guid.NewGuid()
                };

                await userManager.CreateAsync(adminUser, adminPassword);
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            var users = new List<AppUser>
            {
                new AppUser
                {
                    UserName = "user1@example.com",
                    Email = "user1@example.com",
                    Name = "Test User1",
                    RefreshToken = Guid.NewGuid()
                },
                new AppUser
                {
                    UserName = "user2@example.com",
                    Email = "user2@example.com",
                    Name = "Test User2",
                    RefreshToken = Guid.NewGuid()
                },
                new AppUser
                {
                    UserName = "user3@example.com",
                    Email = "user3@example.com",
                    Name = "Test User3",
                    RefreshToken = Guid.NewGuid()
                }
            };

            foreach (var user in users) {
                var existingUser = await userManager.FindByEmailAsync(user.Email!);
                if (existingUser == null) {
                    await userManager.CreateAsync(user, "User123#");
                    await userManager.AddToRoleAsync(user, "User");
                }
            }
        }
    }
}