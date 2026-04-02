using Quiz.DataAccess.Models.Enums;
using Quiz.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.DataAccess.Services
{
    public class DbInitializer
    {
        public static void Initialize(QuizAppDbContext context) {
            if (context.AppUsers.Any()) return;

            // USER
            var user = new AppUser {
                UserName = "admin",
                Email = "admin@test.com"
            };

            context.AppUsers.Add(user);
            context.SaveChanges();

            // QUIZ
            var quiz = new Models.Quiz {
                Title = "Teszt Quiz",
                CreatedByUserId = user.Id,
                IsPublished = true
            };

            context.Quizzes.Add(quiz);
            context.SaveChanges();

            // QUESTIONS
            var q1 = new Question {
                QuizId = quiz.Id,
                Text = "Mennyi 2 + 2?",
                OrderIndex = 1,
                TimeLimitSeconds = 10
            };

            var q2 = new Question {
                QuizId = quiz.Id,
                Text = "Magyarország fővárosa?",
                OrderIndex = 2,
                TimeLimitSeconds = 10
            };

            context.Questions.AddRange(q1, q2);
            context.SaveChanges();

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

            context.AnswerOptions.AddRange(answersQ1);
            context.AnswerOptions.AddRange(answersQ2);
            context.SaveChanges();

            // SESSION
            var session = new QuizSession {
                QuizId = quiz.Id,
                HostUserId = user.Id,
                JoinCode = "ABC123",
                CurrentQuestionIndex = 0,
                QuizSessionStatus = QuizSessionStatus.Waiting
            };

            context.QuizSessions.Add(session);
            context.SaveChanges();

            // PARTICIPANTS
            var p1 = new SessionParticipant {
                QuizSessionId = session.Id,
                Nickname = "Bela",
                IsHost = true
            };

            var p2 = new SessionParticipant {
                QuizSessionId = session.Id,
                Nickname = "Anna"
            };

            context.SessionParticipants.AddRange(p1, p2);
            context.SaveChanges();

            // ANSWERS (simulate gameplay)
            var correctAnswerQ1 = context.AnswerOptions
                .First(a => a.QuestionId == q1.Id && a.IsCorrect);

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

            context.SaveChanges();
        }
    }
}
