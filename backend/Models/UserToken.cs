using System.ComponentModel.DataAnnotations;

public class UserToken
{
    [Key]
    public int token_id {get; set;}
    public int user_id {get; set;}
    public string? token_hash {get; set;}
    public DateTime refresh {get; set;}
    public DateTime expires {get; set;}
    public bool revoked {get; set;}
}