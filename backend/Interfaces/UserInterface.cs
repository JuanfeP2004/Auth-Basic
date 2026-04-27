public interface IUser
{
    public Task<User?> FindUser(string? name, string? email);
    public Task<User?> FindUser(Guid? uuid);
    public Task CreateUser(User template);
    public Task<Factor?> FindAuthFactor(string? text);
    public Task ModifyPassword(int user_id, string? new_password);
}