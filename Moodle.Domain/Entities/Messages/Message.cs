namespace Moodle.Domain.Entities.Messages;

public class Message
{
    public int MessageId {get; set;}
    public int SenderId {get; set;}
    public int ReceiverId {get; set;}
    public string Content { get; set; } = null!;
    public DateTime CreatedAt {get; set;}
    public DateTime UpdatedAt {get; set;}
}