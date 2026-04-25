public class UserResponse : AuthResponse {
    public Guid uuid {get; set;}
    public string? name_user {get; set;}
    public string? email {get; set;}
    public string? phone {get; set;}
    public bool is_active {get; set;}
    public DateTime created_at {get; set;}

    public string? token {get; set;}
}