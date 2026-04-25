public class AuthService
{
    IAuth _authRepository;
    UtilityService _utility;

    TimeSpan refresh_time = new TimeSpan(0, 15, 0);

    public AuthService(IAuth authRepository, UtilityService utility)
    {
        _authRepository = authRepository;
        _utility = utility;
    }

    public async Task<UserToken?> AuthenticateUser(Guid? uuid, string? token)
    {
        try {
            if(!_utility.ValidateString(token))
                return null;
            if(uuid is null)
                return null;
        
            User? user = await _authRepository.FindUser(uuid);
            if(user is null) return null;

            string? token_hash = _utility.Sha256Encrypt(token);
            UserToken? userToken = await _authRepository.FindToken(user.user_id, token_hash);

            if(userToken is null) return null;

            DateTime refresh = DateTime.Now.Add(refresh_time);
            await _authRepository.RefreshToken(userToken.token_id, refresh);
            return userToken;
        }
        catch
        {
            throw new Exception();
        }
    }

    public async Task<bool> AuthorizeUser(UserToken token, string? role)
    {
        try
        {
            if(!_utility.ValidateString(role))
                return false;
            
            if(await _authRepository.FindUserRole(token.user_id, role) is false)
                return false;

            return true;
        }
        catch
        {
            throw new Exception();
        }
    }
}