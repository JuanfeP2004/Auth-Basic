using Microsoft.AspNetCore.Http.HttpResults;

public class LoginService
{
    UtilityService _utility;
    AuthService _auth;
    ILogin _loginRepository;

    readonly TimeSpan code_time = new TimeSpan(0, 5, 0);
    readonly TimeSpan token_time = new TimeSpan(0, 60, 0);
    readonly int password_length = 8;
    readonly string subjectLiteral2fa = "Auth Basic Authenticator Code";
    readonly string subjectLiteralReset = "Auth Basic Reset Code";
    
    public LoginService(UtilityService utility, AuthService auth, ILogin loginRepository)
    {
        _loginRepository = loginRepository;
        _auth = auth;
        _utility = utility;
    }
    public async Task<(int, AuthResponse)> Login(Credentials credentials)
    {
        try 
        {
            if(!_utility.ValidateString(credentials.email))
                return (400, new StringResponse{ Text = "Ins't a Email"});
            if(!_utility.ValidateString(credentials.password))
                return (400, new StringResponse{ Text = "Ins't a Password"});

            User? user = await _loginRepository.FindUser(credentials.email);
            if(user is null)
                return (404, new StringResponse{ Text = "Incorrect credentials"});

            string token_hash = _utility.Sha256Encrypt(credentials.password, user.salt_char);
            if(!token_hash.Equals(user.password_hash))
                return (404, new StringResponse{ Text = "Incorrect credentials"});

            string code = _utility.GenerateSafeCode(6);
            DateTime expires = DateTime.Now.Add(code_time);
            
            await _loginRepository.Create2FACode(user.user_id, code, expires);

            string message = $"Your code is {code}, It expires in {code_time.Minutes} Minutes";
            await Send2FA(user, subjectLiteral2fa, message);
        
            return (200, new GuidResponse {Uuid = user.uuid});
        }
        catch
        {
            return (500, new StringResponse {Text = "A server error ocurred"});
        }
    }
    public async Task<(int, AuthResponse)> Send2FA(SendCodeBody body)
    {
        try {
            if(body.Uuid is null)
                return (400, new StringResponse{Text = "Isn't an Id"});
            if(!_utility.ValidateString(body.Code))
                return (400, new StringResponse{Text = "Isn't a code"});

            User? user = await _loginRepository.FindUser(body.Uuid);
            if(user is null)
                return (404, new StringResponse{Text= "Invalid Id"});

            if(await _loginRepository.Use2FACode(user.user_id, body.Code) is false)
                return (404, new StringResponse{Text = "Invalid code"});

            string token_code = _utility.GenerateSafeString(24);
            string token_hash = _utility.Sha256Encrypt(token_code);
            DateTime expires = DateTime.Now.Add(token_time);

            await _loginRepository.CreateToken(user.user_id, token_hash, expires);

            UserResponse response = new UserResponse
            {
                uuid = user.uuid,
                name_user = user.name_user,
                email = user.email,
                phone = user.phone,
                is_active = user.is_active,
                created_at = user.created_at,
                token = token_code
            };

            return (200, response);
        }
        catch
        {
            return (500, new StringResponse{Text = "A server error ocurred"});
        }
    }
    public async Task<(int, AuthResponse)> Logout(Guid? uuid, string? token)
    {
        try {
            if(uuid is null)
                return (400, new StringResponse{ Text = "Ins't an Id"});
            if(!_utility.ValidateString(token))
                return (401, new StringResponse{ Text = "You're not authenticated"});
        
            UserToken? userToken = await _auth.AuthenticateUser(uuid, token);
            if(userToken is null)
                return (401, new StringResponse{ Text = "Invalid authentication token"});
        
            await _loginRepository.RevokeToken(userToken.token_id);

            return (204, new AuthResponse{});
        }
        catch
        {
            return (500, new StringResponse {Text = "A server error ocurred"});
        }
    }
    public async Task<(int, AuthResponse)> CreateResetCode(StringBody body)
    {
        try {
            if(!_utility.ValidateString(body.Text))
                return (400, new StringResponse {Text="Isn't an email"});

            User? user = await _loginRepository.FindUser(body.Text);
            if(user is null)
                return (404, new StringResponse{ Text = "This email doesn't exist in the application"});

            string code = _utility.GenerateSafeCode(6);
            DateTime expires = DateTime.Now.Add(code_time);
            
            await _loginRepository.CreateResetCode(user.user_id, code, expires);

            string message = $"Your code is {code}, It expires in {code_time.Minutes} Minutes";
            await Send2FA(user, subjectLiteralReset, message);
        
            return (200, new GuidResponse {Uuid = user.uuid});
        }
        catch
        {
            return (500, new StringResponse {Text = "A server error ocurred"});
        }
    }
    public async Task<(int, AuthResponse)> SendResetCode(ResetBody body)
    {
        try
        {
            if(body.Uuid is null)
                return (400, new StringResponse{Text = "Isn't an Id"});
            if(!_utility.ValidateString(body.Code))
                return (400, new StringResponse{Text = "Isn't a code"});
            if(!_utility.ValidateString(body.NewPassword))
                return (400, new StringResponse{Text = "Isn't a password"});
            if(body.NewPassword.Length < password_length)
                return (400, new StringResponse{Text = $"Password is too short, it must be at least {password_length} characters long"});

            User? user = await _loginRepository.FindUser(body.Uuid);
            if(user is null)
                return (404, new StringResponse{Text= "Invalid Id"});

            if(await _loginRepository.UseResetCode(user.user_id, body.Code) is false)
                return (404, new StringResponse{Text = "Invalid code"});

            string password_hash = _utility.Sha256Encrypt(body.NewPassword, user.salt_char);

            await _loginRepository.ResetPassword(user.user_id, password_hash);

            return (200, new StringResponse{Text = "Your password has been successfully changed"});
        }
        catch
        {
            throw new Exception();
        }
    }
    
    
    public async Task Send2FA(User user, string subject, string message)
    {
        switch (user.second_auth)
        {
            case 1:
                return;
            case 2:
                await _loginRepository.SendEmail(user.email, subject, message);
                break;
            default:
                throw new Exception();
        }
    }
}