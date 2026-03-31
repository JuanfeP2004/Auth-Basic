using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

public class Factor
{
    [Key]
    public int factor_id {get; set;}
    public string? factor_name {get; set;}
}