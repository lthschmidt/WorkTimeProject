using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class Transaction
{
    [Key]
    public int Id { get; set; }

    [Required]
    [ForeignKey("User")]
    public int UserId { get; set; }
    [JsonIgnore]
    public User? User { get; set; }

    [Required]
    [ForeignKey("Task")]
    public int TaskId { get; set; }
    [JsonIgnore]
    public Task? Task { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [JsonConverter(typeof(DateOnlyConverter))]
    public DateTime Date { get; set; }

    public double Hours { get; set; }
    
    public string Description { get; set; } = string.Empty;
}
