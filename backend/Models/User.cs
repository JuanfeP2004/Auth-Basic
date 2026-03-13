public class User
{
    public Guid guid {get; set;}
    public string? name {get; set;}
    public string? email {get; set;}
    public string? phone {get; set;}
    public string? password {get; set;}
    public string? salt {get; set;}
    public string? secondFactor {get; set;} 
    public bool isActive {get; set;}
    public DateTime createAt {get; set;}
}