﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Manganese.Text;
using Chaldene.Data.Events.Concretes.Request;
using Chaldene.Data.Messages;
using Chaldene.Data.Messages.Concretes;
using Chaldene.Data.Messages.Receivers;
using Chaldene.Data.Sessions;
using Chaldene.Data.Shared;
using Chaldene.Sessions;

namespace Chaldene.Utils.Scaffolds;

/// <summary>
/// mirai相关拓展方法
/// </summary>
public static class MiraiScaffold
{
    #region MiraiBot extensions

    /// <summary>
    ///     拓展方法，获取mirai-api-http插件的版本，此方法不需要经过任何认证
    /// </summary>
    /// <returns></returns>
    public static async Task<string> GetPluginVersionAsync(this MiraiBot bot)
    {
        try
        {
            var response = await bot.GetAsync(HttpEndpoints.About).ConfigureAwait(false);

            bot.EnsureSuccess(response, response);

            return response.Fetch("data.version");
        }
        catch (Exception e)
        {
            throw new Exception($"获取失败: {e.Message}\n{bot}");
        }
    }

    // /// <summary>
    // /// 判断某QQ号是否为bot账号的好友
    // /// </summary>
    // /// <param name="bot"></param>
    // /// <param name="qq"></param>
    // /// <returns></returns>
    // public static bool IsFriend(this MiraiBot bot, string qq)
    // {
    //     return bot.Friends.Value.Any(x => x.Id == qq);
    // }
    //
    // /// <summary>
    // /// 判断某群成员是否是bot账号的好友
    // /// </summary>
    // /// <param name="bot"></param>
    // /// <param name="member"></param>
    // /// <returns></returns>
    // public static bool IsFriend(this MiraiBot bot, Member member)
    // {
    //     return bot.Friends.Value.Any(x => x.Id == member.Id);
    // }
    //
    // /// <summary>
    // /// 判断某人是否是bot账号的好友
    // /// </summary>
    // /// <param name="bot"></param>
    // /// <param name="friend"></param>
    // /// <returns></returns>
    // public static bool IsFriend(this MiraiBot bot, Friend friend)
    // {
    //     return bot.Friends.Value.Any(x => x.Id == friend.Id);
    // }

    #endregion

    #region Modularization extensions

    /// <summary>
    /// 把MessageReceiverBase转换为具体的MessageReceiver
    /// </summary>
    /// <param name="base"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Concretize<T>(this MessageReceiverBase @base) where T : MessageReceiverBase
    {
        return (T)@base;
    }

    #endregion

    #region Message extension

    /// <summary>
    ///     发送群消息
    /// </summary>
    /// <param name="receiver"></param>
    /// <param name="chain"></param>
    /// <returns></returns>
    public static async Task<string> SendMessageAsync(this GroupMessageReceiver receiver,
        MessageChain chain)
    {
        return await receiver.Bot
            .SendGroupMessageAsync(receiver.Sender.Group.Id, chain).ConfigureAwait(false);
    }

    /// <summary>
    ///     发送好友消息
    /// </summary>
    /// <param name="receiver"></param>
    /// <param name="chain"></param>
    /// <returns></returns>
    public static async Task<string> SendMessageAsync(this FriendMessageReceiver receiver,
        MessageChain chain)
    {
        return await receiver.Bot
            .SendFriendMessageAsync(receiver.Sender.Id, chain).ConfigureAwait(false);
    }

    /// <summary>
    ///     发送临时消息
    /// </summary>
    /// <param name="receiver"></param>
    /// <param name="chain"></param>
    /// <returns></returns>
    public static async Task<string> SendMessageAsync(this TempMessageReceiver receiver, MessageChain chain)
    {
        return await receiver.Bot
            .SendTempMessageAsync(receiver.Sender.Id, receiver.Sender.Group.Id, chain).ConfigureAwait(false);
    }

    /// <summary>
    ///     发送群消息
    /// </summary>
    /// <param name="receiver"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static async Task<string> SendMessageAsync(this GroupMessageReceiver receiver, params MessageBase[] message)
    {
        return await receiver.Bot
            .SendGroupMessageAsync(receiver.Sender.Group.Id, message).ConfigureAwait(false);
    }

    /// <summary>
    ///     发送好友消息
    /// </summary>
    /// <param name="receiver"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static async Task<string> SendMessageAsync(this FriendMessageReceiver receiver, params MessageBase[] message)
    {
        return await receiver.Bot
            .SendFriendMessageAsync(receiver.Sender.Id, message).ConfigureAwait(false);
    }

    /// <summary>
    ///     发送临时消息
    /// </summary>
    /// <param name="receiver"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static async Task<string> SendMessageAsync(this TempMessageReceiver receiver, params MessageBase[] message)
    {
        return await receiver.Bot
            .SendTempMessageAsync(receiver.Sender, message).ConfigureAwait(false);
    }

    /// <summary>
    ///     撤回收到的消息
    /// </summary>
    /// <param name="receiver"></param>
    [Obsolete("此方法在mirai-api-http 2.6.0及以上版本会导致异常")]
    public static async Task RecallAsync(this MessageReceiverBase receiver)
    {
        var id = receiver.MessageChain.OfType<SourceMessage>().First().MessageId;
        await receiver.Bot
            .RecallAsync(id).ConfigureAwait(false);
    }

    /// <summary>
    ///     撤回收到的消息
    /// </summary>
    /// <param name="receiver"></param>
    public static async Task RecallAsync(this GroupMessageReceiver receiver)
    {
        var id = receiver.MessageChain.OfType<SourceMessage>().First().MessageId;
        await receiver.Bot
            .RecallAsync(receiver.GroupId, id).ConfigureAwait(false);
    }

    /// <summary>
    ///     撤回收到的消息
    /// </summary>
    /// <param name="receiver"></param>
    public static async Task RecallAsync(this FriendMessageReceiver receiver)
    {
        var id = receiver.MessageChain.OfType<SourceMessage>().First().MessageId;
        await receiver.Bot
            .RecallAsync(receiver.FriendId, id).ConfigureAwait(false);
    }

    /// <summary>
    ///     撤回收到的消息
    /// </summary>
    /// <param name="receiver"></param>
    public static async Task RecallAsync(this TempMessageReceiver receiver)
    {
        var id = receiver.MessageChain.OfType<SourceMessage>().First().MessageId;
        await receiver.Bot
            .RecallAsync(receiver.Sender.Id, id).ConfigureAwait(false);
    }

    /// <summary>
    /// 回复消息
    /// </summary>
    /// <param name="receiver"></param>
    /// <param name="chain"></param>
    /// <returns></returns>
    public static async Task<string> QuoteMessageAsync(this FriendMessageReceiver receiver,
        MessageChain chain)
    {
        var id = receiver.MessageChain.OfType<SourceMessage>().First().MessageId;

        return await receiver.Bot
            .QuoteFriendMessageAsync(receiver.Sender.Id, id, chain).ConfigureAwait(false);
    }

    /// <summary>
    /// 回复消息
    /// </summary>
    /// <param name="receiver"></param>
    /// <param name="chain"></param>
    /// <returns></returns>
    public static async Task<string> QuoteMessageAsync(this GroupMessageReceiver receiver,
        MessageChain chain)
    {
        var id = receiver.MessageChain.OfType<SourceMessage>().First().MessageId;

        return await receiver.Bot
            .QuoteGroupMessageAsync(receiver.Sender.Group.Id, id, chain).ConfigureAwait(false);
    }

    /// <summary>
    /// 回复消息
    /// </summary>
    /// <param name="receiver"></param>
    /// <param name="chain"></param>
    /// <returns></returns>
    public static async Task<string> QuoteMessageAsync(this TempMessageReceiver receiver,
        MessageChain chain)
    {
        var id = receiver.MessageChain.OfType<SourceMessage>().First().MessageId;

        return await receiver.Bot
            .QuoteTempMessageAsync(receiver.Sender.Id, receiver.Sender.Group.Id, id, chain).ConfigureAwait(false);
    }

    /// <summary>
    /// 回复消息
    /// </summary>
    /// <param name="receiver"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static async Task<string> QuoteMessageAsync(this FriendMessageReceiver receiver, params MessageBase[] message)
    {
        var id = receiver.MessageChain.OfType<SourceMessage>().First().MessageId;

        return await receiver.Bot
            .QuoteFriendMessageAsync(receiver.Sender.Id, id, message).ConfigureAwait(false);
    }

    /// <summary>
    /// 回复消息
    /// </summary>
    /// <param name="receiver"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static async Task<string> QuoteMessageAsync(this GroupMessageReceiver receiver, params MessageBase[] message)
    {
        var id = receiver.MessageChain.ToList().OfType<SourceMessage>().First().MessageId;

        return await receiver.Bot
            .QuoteGroupMessageAsync(receiver.Sender.Group.Id, id, message).ConfigureAwait(false);
    }

    /// <summary>
    /// 回复消息
    /// </summary>
    /// <param name="receiver"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static async Task<string> QuoteMessageAsync(this TempMessageReceiver receiver, params MessageBase[] message)
    {
        var id = receiver.MessageChain.OfType<SourceMessage>().First().MessageId;

        return await receiver.Bot
            .QuoteTempMessageAsync(receiver.Sender.Id, receiver.Sender.Group.Id, id, message).ConfigureAwait(false);
    }

    #endregion

    #region Request extensions

    /// <summary>
    ///     处理好友请求
    /// </summary>
    /// <param name="event"></param>
    /// <param name="handler"></param>
    /// <param name="message"></param>
    [Obsolete("请使用直接Approve/RejectAndBlock/RejectAsync 等方法")]
    public static async Task Handle(this NewFriendRequestedEvent @event,
        NewFriendRequestHandlers handler, string message = "")
    {
        await @event.Bot.HandleNewFriendRequestedAsync(@event, handler, message).ConfigureAwait(false);
    }

    /// <summary>
    /// 同意好友请求
    /// </summary>
    /// <param name="event"></param>
    public static async Task ApproveAsync(this NewFriendRequestedEvent @event)
    {
        await @event.Bot.HandleNewFriendRequestedAsync(@event, NewFriendRequestHandlers.Approve).ConfigureAwait(false);
    }

    /// <summary>
    /// 拒绝好友请求
    /// </summary>
    /// <param name="event">事件</param>
    /// <param name="message">回复的消息</param>
    public static async Task RejectAsync(this NewFriendRequestedEvent @event, string message = "")
    {
        await @event.Bot.HandleNewFriendRequestedAsync(@event, NewFriendRequestHandlers.Reject, message).ConfigureAwait(false);
    }
    
    /// <summary>
    /// 拒绝好友请求且屏蔽对方
    /// </summary>
    /// <param name="event">事件</param>
    /// <param name="message">回复的消息</param>
    public static async Task RejectAndBlockAsync(this NewFriendRequestedEvent @event, string message = "")
    {
        await @event.Bot.HandleNewFriendRequestedAsync(@event, NewFriendRequestHandlers.RejectAndBlock, message).ConfigureAwait(false);
    }

    /// <summary>
    ///     处理新成员加群请求
    /// </summary>
    /// <param name="requestedEvent"></param>
    /// <param name="handler"></param>
    /// <param name="message"></param>
    [Obsolete("请使用直接Approve/RejectAndBlock/RejectAsync 等方法")]
    public static async Task Handle(this NewMemberRequestedEvent requestedEvent,
        NewMemberRequestHandlers handler, string message = "")
    {
        await requestedEvent.Bot
            .HandleNewMemberRequestedAsync(requestedEvent, handler, message).ConfigureAwait(false);
    }
    
    /// <summary>
    /// 同意加群请求
    /// </summary>
    /// <param name="requestedEvent"></param>
    public static async Task ApproveAsync(this NewMemberRequestedEvent requestedEvent)
    {
        await requestedEvent.Bot.HandleNewMemberRequestedAsync(requestedEvent, NewMemberRequestHandlers.Approve).ConfigureAwait(false);
    }

    /// <summary>
    /// 拒绝加群请求
    /// </summary>
    /// <param name="requestedEvent">事件源</param>
    /// <param name="message">回复消息</param>
    public static async Task RejectAsync(this NewMemberRequestedEvent requestedEvent, string message = "")
    {
        await requestedEvent.Bot.HandleNewMemberRequestedAsync(requestedEvent, NewMemberRequestHandlers.Reject, message).ConfigureAwait(false);
    }

    /// <summary>
    /// 忽略加群请求
    /// </summary>
    /// <param name="requestedEvent"></param>
    public static async Task DismissAsync(this NewMemberRequestedEvent requestedEvent)
    {
        await requestedEvent.Bot.HandleNewMemberRequestedAsync(requestedEvent, NewMemberRequestHandlers.Dismiss).ConfigureAwait(false);
    }
    
    /// <summary>
    /// 拒绝加群请求并屏蔽
    /// </summary>
    /// <param name="requestedEvent"></param>
    /// <param name="message">回复消息</param>
    public static async Task RejectAndBlockAsync(this NewMemberRequestedEvent requestedEvent, string message = "")
    {
        await requestedEvent.Bot.HandleNewMemberRequestedAsync(requestedEvent, NewMemberRequestHandlers.RejectAndBlock, message).ConfigureAwait(false);
    }
    
    /// <summary>
    /// 忽略加群请求并屏蔽
    /// </summary>
    /// <param name="requestedEvent"></param>
    public static async Task DismissAndBlockAsync(this NewMemberRequestedEvent requestedEvent)
    {
        await requestedEvent.Bot.HandleNewMemberRequestedAsync(requestedEvent, NewMemberRequestHandlers.DismissAndBlock).ConfigureAwait(false);
    }

    /// <summary>
    ///     处理bot被邀请进群请求
    /// </summary>
    /// <param name="requestedEvent"></param>
    /// <param name="handler"></param>
    /// <param name="message"></param>
    [Obsolete("请使用直接Approve/RejectAndBlock/RejectAsync 等方法")]
    public static async Task Handle(this NewInvitationRequestedEvent requestedEvent,
        NewInvitationRequestHandlers handler, string message)
    {
        await requestedEvent.Bot
            .HandleNewInvitationRequestedAsync(requestedEvent, handler, message).ConfigureAwait(false);
    }
    
    /// <summary>
    /// 同意邀请
    /// </summary>
    /// <param name="requestedEvent"></param>
    public static async Task ApproveAsync(this NewInvitationRequestedEvent requestedEvent)
    {
        await requestedEvent.Bot.HandleNewInvitationRequestedAsync(requestedEvent, NewInvitationRequestHandlers.Approve,
            "").ConfigureAwait(false);
    }

    /// <summary>
    /// 拒绝邀请
    /// </summary>
    /// <param name="requestedEvent"></param>
    public static async Task RejectAsync(this NewInvitationRequestedEvent requestedEvent)
    {
        await requestedEvent.Bot.HandleNewInvitationRequestedAsync(requestedEvent, NewInvitationRequestHandlers.Reject,
            "").ConfigureAwait(false);
    }

    #endregion
}