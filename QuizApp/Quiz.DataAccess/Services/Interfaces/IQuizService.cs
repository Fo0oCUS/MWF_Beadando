namespace Quiz.DataAccess.Services.Interfaces;

public interface IQuizService
{
    public Task<Models.Quiz> GetByIdAsync(int id);

    public Task<IReadOnlyCollection<Models.Quiz>> GetByUserIdAsync(string id);

    public Task<Models.Quiz> AddAsync(Models.Quiz quiz);
}