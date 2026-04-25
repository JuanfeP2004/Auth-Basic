using System.Text.RegularExpressions;

public class UserService
{
    IUser _userRepository;

    UtilityService _utility;

    readonly int password_length = 8;
    readonly int salt_length = 16;
    readonly string email_pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
    readonly string phone_pattern = @"^(\+\d{1,2}\s?)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$";

    public UserService(IUser userRepository, UtilityService utility)
    {
        _userRepository = userRepository;
        _utility = utility;
    }

    public async Task<(int, AuthResponse)> Create(UserBody user)
    {
        try
        {
            if(!_utility.ValidateString(user.Name))
                return (400, new StringResponse {Text = "Ins't a name"});
            if(!_utility.ValidateString(user.Email))
                return (400, new StringResponse {Text = "Ins't a email"});
            if(!_utility.ValidateString(user.Phone))
                return (400, new StringResponse {Text = "Ins't a phone"});
            if(!_utility.ValidateString(user.Password))
                return (400, new StringResponse {Text = "Ins't a password"});
            if(!_utility.ValidateString(user.Second_factor))
                return (400, new StringResponse {Text = "Ins't a authentication factor"});
            if(user.Password.Length < password_length)
                return (400, new StringResponse {Text = $"Password is too short, it must be at least {password_length} characters long"});

            if(!Regex.IsMatch(user.Email, email_pattern))
                return (400, new StringResponse {Text = "Bad formated email"});
            if(!Regex.IsMatch(user.Phone, phone_pattern))
                return (400, new StringResponse {Text = "Bad formated phone"});
            
            Factor? factor = await _userRepository.FindAuthFactor(user.Second_factor);

            User? duplicate_user = await _userRepository.FindUser(user.Name, user.Email);
            if(duplicate_user is not null)
                return (400, new StringResponse {Text = "Already exists an user with the same name or email in the application"});

            string salt = _utility.GenerateSafeString(salt_length);
            string password_hash = _utility.Sha256Encrypt(user.Password, salt);

            User new_user = new User
            {
                uuid = Guid.NewGuid(),
                name_user = user.Name,
                email = user.Email,
                phone = user.Phone,
                password_hash = password_hash,
                salt_char = salt,
                second_auth = factor.factor_id,
                created_at = DateTime.Now,
                is_active = true
            };
            await _userRepository.CreateUser(new_user);
            return (201, new StringResponse {Text = "User created"});
        }
        catch
        {
            return (500, new StringResponse {Text = "A server error ocurred"});
        }
    }
}