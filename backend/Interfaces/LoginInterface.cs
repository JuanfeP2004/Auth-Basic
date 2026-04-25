public interface ILogin
{
    public Task<User?> FindUser(string email);
    public Task<User?> FindUser(Guid uuid);
    public Task Create2FACode(int id, string code, DateTime expires);
    public Task<bool> Use2FACode(int user_id, string code);
    public Task CreateToken(int user_id, string token_hash, DateTime expires);
    
    public Task RevokeToken(int token_id);

    /*
    string CreateResetCode(string email);
    string SendResetCode(string email);
    */
    public Task SendEmail(string email, string subject, string message);
    //string SendSMS(string phon, string subject, string message);*/
}