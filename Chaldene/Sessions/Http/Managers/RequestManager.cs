using System.Threading.Tasks;
using Chaldene.Data.Events.Concretes.Request;
using Chaldene.Data.Sessions;
using Chaldene.Data.Shared;


namespace Chaldene.Sessions;

/// <summary>
/// 请求管理器
/// </summary>
public partial class MiraiBot
{
    /// <summary>
    ///     处理好友申请
    /// </summary>
    /// <param name="requestedEvent"></param>
    /// <param name="handler"></param>
    /// <param name="message"></param>
    public async Task HandleNewFriendRequestedAsync(NewFriendRequestedEvent requestedEvent,
        NewFriendRequestHandlers handler, string message = "")
    {
        var payload = new
        {
            eventId = requestedEvent.EventId,
            fromId = requestedEvent.FromId,
            groupId = requestedEvent.GroupId,
            operate = handler,
            message
        };

        _ = await PostJsonAsync(HttpEndpoints.NewFriendRequested, payload).ConfigureAwait(false);
    }

    /// <summary>
    ///     处理新成员入群申请,需要管理员权限
    /// </summary>
    /// <param name="requestedEvent"></param>
    /// <param name="handler"></param>
    /// <param name="message"></param>
    public async Task HandleNewMemberRequestedAsync(NewMemberRequestedEvent requestedEvent,
        NewMemberRequestHandlers handler, string message = "")
    {
        var payload = new
        {
            eventId = requestedEvent.EventId,
            fromId = requestedEvent.FromId,
            groupId = requestedEvent.GroupId,
            operate = handler,
            message
        };

        _ = await PostJsonAsync(HttpEndpoints.MemberJoinRequested, payload).ConfigureAwait(false);
    }

    /// <summary>
    ///     处理bot被邀请进群申请
    /// </summary>
    /// <param name="requestedEvent"></param>
    /// <param name="handler"></param>
    /// <param name="message"></param>
    public async Task HandleNewInvitationRequestedAsync(NewInvitationRequestedEvent requestedEvent,
        NewInvitationRequestHandlers handler, string message = "")
    {
        var payload = new
        {
            eventId = requestedEvent.EventId,
            fromId = requestedEvent.FromId,
            groupId = requestedEvent.GroupId,
            operate = handler,
            message
        };

        _ = await PostJsonAsync(HttpEndpoints.BotInvited, payload).ConfigureAwait(false);
    }
}