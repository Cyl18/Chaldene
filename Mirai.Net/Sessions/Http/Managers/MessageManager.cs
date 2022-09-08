using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Manganese.Text;
using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Sessions;
using Mirai.Net.Data.Shared;
using Mirai.Net.Utils.Scaffolds;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mirai.Net.Sessions;

/// <summary>
/// 消息管理器
/// </summary>
public partial class MiraiBot
{
    #region Private helpers

    private async Task<string> SendMessageAsync(HttpEndpoints endpoints, object payload)
    {
        var response = await PostJsonAsync(endpoints, payload).ConfigureAwait(false);

        return response.Fetch("messageId");
    }

    #endregion

    #region Exposed

    /// <summary>
    /// 由消息id获取一条消息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [Obsolete("此方法在mirai-api-http 2.6.0及以上版本会导致异常")]
    public async Task<T> GetMessageReceiverByIdAsync<T>(string messageId) where T : MessageReceiverBase
    {
        var response = await GetAsync(HttpEndpoints.MessageFromId, new
        {
            id = messageId
        }).ConfigureAwait(false);

        return JsonConvert.DeserializeObject<T>(response);
    }

    /// <summary>
    /// 获取一条消息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="messageId">消息id</param>
    /// <param name="target">好友id或群id</param>
    /// <returns></returns>
    public async Task<T> GetMessageReceiverAsync<T>(string messageId, string target) where T : MessageReceiverBase
    {
        var response = await GetAsync(HttpEndpoints.MessageFromId, new
        {
            id = messageId,
            target
        }).ConfigureAwait(false);

        return JsonConvert.DeserializeObject<T>(response);
    }

    /// <summary>
    /// 获取漫游消息（目前仅支持好友）
    /// </summary>
    /// <param name="target">好友id或群id</param>
    /// <param name="timeStart">起始时间, UTC+8 时间戳, 单位为秒. 可以为 0, 即表示从可以获取的最早的消息起. 负数将会被看是 0.</param>
    /// <param name="timeEnd">结束时间, UTC+8 时间戳, 单位为秒. 可以为 <c>long.MaxValue</c>, 即表示到可以获取的最晚的消息为止. 低于 timeStart 的值将会被看作是 timeStart 的值.</param>
    /// <returns></returns>
    public async Task<IEnumerable<MessageChain>> GetRoamingMessagesAsync(string target, string timeStart, string timeEnd)
    {
        var response = await GetAsync(HttpEndpoints.RoamingMessages, new
        {
            timeStart,
            timeEnd,
            target
        }).ConfigureAwait(false);

        return ((JArray)response.ToJObject()["data"]).Values<MessageChain>();
    }

    /// <summary>
    ///     发送好友消息
    /// </summary>
    /// <param name="target"></param>
    /// <param name="chain"></param>
    /// <returns></returns>
    public async Task<string> SendFriendMessageAsync(UserId target, MessageChain chain)
    {
        var payload = new
        {
            target = (string)target,
            messageChain = chain
        };

        return await SendMessageAsync(HttpEndpoints.SendFriendMessage, payload).ConfigureAwait(false);
    }
    /// <summary>
    /// 发送好友消息
    /// </summary>
    /// <param name="target"></param>
    /// <param name="messages"></param>
    /// <returns></returns>
    public async Task<string> SendFriendMessageAsync(UserId target, params MessageBase[] messages)
    {
        return await SendFriendMessageAsync(target, new MessageChain(messages)).ConfigureAwait(false);
    }

    /// <summary>
    ///     发送群消息
    /// </summary>
    /// <param name="target"></param>
    /// <param name="chain"></param>
    /// <returns></returns>
    public async Task<string> SendGroupMessageAsync(GroupId target, MessageChain chain)
    {
        var payload = new
        {
            target = (string)target,
            messageChain = chain
        };

        return await SendMessageAsync(HttpEndpoints.SendGroupMessage, payload).ConfigureAwait(false);
    }

    /// <summary>
    /// 发送群消息
    /// </summary>
    /// <param name="target"></param>
    /// <param name="messages"></param>
    /// <returns></returns>
    public Task<string> SendGroupMessageAsync(GroupId target, params MessageBase[] messages)
    {
        return SendGroupMessageAsync(target, new MessageChain(messages));
    }


    /// <summary>
    ///     发送群临时消息
    /// </summary>
    /// <param name="qq"></param>
    /// <param name="group"></param>
    /// <param name="chain"></param>
    /// <returns></returns>
    public async Task<string> SendTempMessageAsync(UserId qq, GroupId group, MessageChain chain)
    {
        var payload = new
        {
            qq,
            group,
            messageChain = chain
        };

        return await SendMessageAsync(HttpEndpoints.SendTempMessage, payload).ConfigureAwait(false);
    }
    
    /// <summary>
    /// 发送群临时消息
    /// </summary>
    /// <param name="qq"></param>
    /// <param name="group"></param>
    /// <param name="messages"></param>
    /// <returns></returns>
    public Task<string> SendTempMessageAsync(UserId qq, GroupId group, params MessageBase[] messages)
    {
        return SendTempMessageAsync(qq, group, new MessageChain(messages));
    }

    /// <summary>
    ///     发送群临时消息
    /// </summary>
    /// <param name="member"></param>
    /// <param name="chain"></param>
    /// <returns></returns>
    public async Task<string> SendTempMessageAsync(Member member, MessageChain chain)
    {
        return await SendTempMessageAsync(member.Id, member.Group.Id, chain).ConfigureAwait(false);
    }
    
    /// <summary>
    /// 发送群临时消息
    /// </summary>
    /// <param name="member"></param>
    /// <param name="messages"></param>
    /// <returns></returns>
    public Task<string> SendTempMessageAsync(Member member, params MessageBase[] messages)
    {
        return SendTempMessageAsync(member, new MessageChain(messages));
    }

    /// <summary>
    ///     发送头像戳一戳
    /// </summary>
    /// <param name="target">戳一戳的目标</param>
    /// <param name="subject">在什么地方戳</param>
    /// <param name="kind">只可以选Friend, Strange和Group</param>
    public async Task SendNudgeAsync(UserId target, string subject, MessageReceivers kind)
    {
        var payload = new
        {
            target,
            subject,
            kind = kind.ToString()
        };

        await PostJsonAsync(HttpEndpoints.SendNudge, payload).ConfigureAwait(false);
    }

    /// <summary>
    ///     撤回消息
    /// </summary>
    /// <param name="messageId">消息id</param>
    [Obsolete("此方法在mirai-api-http 2.6.0及以上版本会导致异常")]
    public async Task RecallAsync(string messageId)
    {
        var payload = new
        {
            target = messageId
        };

        await PostJsonAsync(HttpEndpoints.Recall, payload).ConfigureAwait(false);
    }

    /// <summary>
    ///     撤回消息
    /// </summary>
    /// <param name="messageId">消息id</param>
    /// <param name="target">好友id或群id</param>
    public async Task RecallAsync(string messageId, string target)
    {
        var payload = new
        {
            target,
            messageId
        };

        await PostJsonAsync(HttpEndpoints.Recall, payload).ConfigureAwait(false);
    }

    /// <summary>
    ///     回复好友消息
    /// </summary>
    /// <param name="target"></param>
    /// <param name="messageId"></param>
    /// <param name="chain"></param>
    /// <returns></returns>
    public async Task<string> QuoteFriendMessageAsync(UserId target, string messageId,
        MessageChain chain)
    {
        var payload = new
        {
            target,
            quote = messageId,
            messageChain = chain
        };

        return await SendMessageAsync(HttpEndpoints.SendFriendMessage, payload).ConfigureAwait(false);
    }
    
    /// <summary>
    /// 回复好友消息
    /// </summary>
    /// <param name="target"></param>
    /// <param name="messageId"></param>
    /// <param name="messages"></param>
    /// <returns></returns>
    public Task<string> QuoteFriendMessageAsync(UserId target, string messageId, params MessageBase[] messages)
    {
        return QuoteFriendMessageAsync(target, messageId, new MessageChain(messages));
    }

    /// <summary>
    ///     回复群消息
    /// </summary>
    /// <param name="target"></param>
    /// <param name="messageId"></param>
    /// <param name="chain"></param>
    /// <returns></returns>
    public async Task<string> QuoteGroupMessageAsync(GroupId target, string messageId, MessageChain chain)
    {
        var payload = new
        {
            target,
            quote = messageId,
            messageChain = chain
        };

        return await SendMessageAsync(HttpEndpoints.SendGroupMessage, payload).ConfigureAwait(false);
    }
    
    /// <summary>
    /// 回复群消息
    /// </summary>
    /// <param name="target"></param>
    /// <param name="messageId"></param>
    /// <param name="messages"></param>
    /// <returns></returns>
    public Task<string> QuoteGroupMessageAsync(GroupId target, string messageId, params MessageBase[] messages)
    {
        return QuoteGroupMessageAsync(target, messageId, new MessageChain(messages));
    }


    /// <summary>
    ///     回复临时消息
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="group"></param>
    /// <param name="messageId"></param>
    /// <param name="chain"></param>
    /// <returns></returns>
    public async Task<string> QuoteTempMessageAsync(UserId memberId, GroupId group, string messageId,
        MessageChain chain)
    {
        var payload = new
        {
            qq = memberId,
            group,
            quote = messageId,
            messageChain = chain
        };

        return await SendMessageAsync(HttpEndpoints.SendTempMessage, payload).ConfigureAwait(false);
    }
    /// <summary>
    /// 回复临时消息
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="group"></param>
    /// <param name="messageId"></param>
    /// <param name="messages"></param>
    /// <returns></returns>
    public Task<string> QuoteTempMessageAsync(UserId memberId, GroupId group, string messageId, params MessageBase[] messages)
    {
        return QuoteTempMessageAsync(memberId, group, messageId, new MessageChain(messages));
    }

    /// <summary>
    ///     回复临时消息
    /// </summary>
    /// <param name="member"></param>
    /// <param name="messageId"></param>
    /// <param name="chain"></param>
    /// <returns></returns>
    public async Task<string> QuoteTempMessageAsync(Member member, string messageId,
        MessageChain chain)
    {
        return await QuoteTempMessageAsync(member.Id, member.Group.Id, messageId, chain).ConfigureAwait(false);
    }
    
    /// <summary>
    /// 回复临时消息
    /// </summary>
    /// <param name="member"></param>
    /// <param name="messageId"></param>
    /// <param name="messages"></param>
    /// <returns></returns>
    public Task<string> QuoteTempMessageAsync(Member member, string messageId, params MessageBase[] messages)
    {
        return QuoteTempMessageAsync(member, messageId, new MessageChain(messages));
    }

    #endregion

}