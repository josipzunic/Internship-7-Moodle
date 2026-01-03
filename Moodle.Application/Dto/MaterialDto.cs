namespace Moodle.Application.Dto;

public class MaterialDto
{
    public DateTime CreatedAt { get; set; }
    public string Name { get; set; } = null!;
    public string Url { get; set; } = null!;
}