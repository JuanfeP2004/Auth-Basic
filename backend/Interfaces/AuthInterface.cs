public interface IAuth
{
    public Task<User?> FindUser(Guid? uuid);
    public Task<UserToken?> FindToken(int user_id, string? token_hash);

    public Task<bool> FindUserRole(int user_id, string? role_name);

    public Task RefreshToken(int token_id, DateTime refresh);
}