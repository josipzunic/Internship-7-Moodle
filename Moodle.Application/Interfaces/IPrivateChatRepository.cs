using Moodle.Application.Dto;
using Moodle.Domain.Entities;

namespace Moodle.Application.Interfaces;

public interface IPrivateChatRepository
{
    Task<List<UserDto>> GetUsersWithoutConversationAsync(int userId);
    Task OpenPrivateChatAsync(int senderId, int receiverId);
    Task<List<UserDto>> GetUsersWithConversationAsync(int userId);
}