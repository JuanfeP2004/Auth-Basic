using Microsoft.EntityFrameworkCore;

public class User
{
    public Guid uuid {get; set;}
    public string? name_user {get; set;}
    public string? email {get; set;}
    public string? phone {get; set;}
    public string? password_hash {get; set;}
    public string? salt_char {get; set;}
    public int second_auth {get; set;} 
    public bool is_active {get; set;}
    public DateTime created_at {get; set;}
}