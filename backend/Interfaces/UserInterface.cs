public interface IUser
{
    public Task<User?> FindUser(string? name, string? email);
    public Task CreateUser(User template);

    public Task<Factor?> FindAuthFactor(string text);
}