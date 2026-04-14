using System.ComponentModel.DataAnnotations;

public class ResetCode
{
    [Key]
    public int code_id {get; set;}
    public int user_id {get; set;}
    public string? code {get; set;}
    public DateTime expires {get; set;}
    public bool used {get; set;}
}