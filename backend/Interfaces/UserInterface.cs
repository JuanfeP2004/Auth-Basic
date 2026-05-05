public interface IUser
{
    public Task<User?> FindUser(string? name, string? email);
    public Task<User?> FindUser(int user_id);
    public Task<User?> FindUser(Guid? user_id);
    public Task CreateUser(User template);
    public Task<Factor?> FindAuthFactor(string? text);
    public Task ModifyPassword(int user_id, string? new_password);
    public Task ModifyIsActive(int user_id, bool is_active);

    public Task ModifySecondFactor(int user_id, int factor_id);

    public Task<Role?> GetRole(string? name_role);
    public Task AddRole(int user_id, int role_id);
    public Task RemoveRole(int user_id, int role_id);
}