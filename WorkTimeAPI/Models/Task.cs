using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class Task
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Project")] // Внешний ключ
    public int ProjectId { get; set; }

    [JsonIgnore]
    public Project? Project { get; set; } // Навигационное свойство

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public bool IsActive { get; set; } = true;
}
