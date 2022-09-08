using System;
using System.Linq;
using System.Threading.Tasks;
using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Concretes;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Data.Sessions;
using Mirai.Net.Data.Shared;
using Newtonsoft.Json;

namespace Mirai.Net.Sessions;

/// <summary>
/// 群管理器
/// </summary>
public partial class MiraiBot
{
    #region Mute

    /// <summary>
    ///     禁言某群员
    /// </summary>
    /// <param name="target"></param>
    /// <param name="group"></param>
    /// <param name="time"></param>
    public async Task MuteAsync(string target, string group, int time)
    {
        var payload = new
        {
            target = group,
            memberId = target,
            time
        };

        await PostJsonAsync(HttpEndpoints.Mute, payload).ConfigureAwait(false);
    }

    /// <see cref="MuteAsync(string,string,int)" />
    public async Task MuteAsync(string target, string group, TimeSpan time)
    {
        await MuteAsync(target, group, Convert.ToInt32(time.TotalSeconds)).ConfigureAwait(false);
    }

    /// <summary>
    ///     禁言某群员
    /// </summary>
    /// <param name="member"></param>
    /// <param name="time"></param>
    public async Task MuteAsync(Member member, int time)
    {
        await MuteAsync(member.Id, member.Group.Id, time).ConfigureAwait(false);
    }

    /// <see cref="MuteAsync(Member,int)" />
    public async Task MuteAsync(Member member, TimeSpan time)
    {
        await MuteAsync(member.Id, member.Group.Id, time).ConfigureAwait(false);
    }

    #endregion

    #region UnMute

    /// <summary>
    ///     取消禁言
    /// </summary>
    /// <param name="target"></param>
    /// <param name="group"></param>
    public async Task UnMuteAsync(string target, string group)
    {
        var payload = new
        {
            target = group,
            memberId = target
        };

        await PostJsonAsync(HttpEndpoints.Unmute, payload).ConfigureAwait(false);
    }

    /// <summary>
    ///     取消禁言
    /// </summary>
    /// <param name="member"></param>
    public async Task UnMuteAsync(Member member)
    {
        await UnMuteAsync(member.Id, member.Group.Id).ConfigureAwait(false);
    }

    #endregion

    #region Kick

    /// <summary>
    ///     踢出某群员
    /// </summary>
    /// <param name="target"></param>
    /// <param name="group"></param>
    /// <param name="message"></param>
    public async Task KickAsync(string target, string group, string message = "")
    {
        var payload = new
        {
            target = group,
            memberId = target,
            msg = message
        };

        await PostJsonAsync(HttpEndpoints.Kick, payload).ConfigureAwait(false);
    }

    /// <summary>
    ///     踢出某群员
    /// </summary>
    /// <param name="member"></param>
    /// <param name="message"></param>
    public async Task KickAsync(Member member, string message = "")
    {
        await KickAsync(member.Id, member.Group.Id).ConfigureAwait(false);
    }

    #endregion

    #region Leave

    /// <summary>
    ///     bot退出某群
    /// </summary>
    /// <param name="target"></param>
    public  async Task LeaveAsync(string target)
    {
        var payload = new
        {
            target
        };

        await PostJsonAsync(HttpEndpoints.Leave, payload).ConfigureAwait(false);
    }

    /// <summary>
    ///     bot退出某群
    /// </summary>
    /// <param name="group"></param>
    public  async Task LeaveAsync(Group group)
    {
        await LeaveAsync(group.Id).ConfigureAwait(false);
    }

    #endregion

    #region MuteAll

    /// <summary>
    ///     全体禁言
    /// </summary>
    /// <param name="target"></param>
    /// <param name="mute">是否禁言</param>
    public  async Task MuteAllAsync(string target, bool mute = true)
    {
        var endpoint = mute ? HttpEndpoints.MuteAll : HttpEndpoints.UnmuteAll;
        var payload = new
        {
            target
        };

        await PostJsonAsync(endpoint, payload).ConfigureAwait(false);
    }

    /// <summary>
    ///     全体禁言
    /// </summary>
    /// <param name="group"></param>
    /// <param name="mute">是否禁言</param>
    public  async Task MuteAllAsync(Group group, bool mute = true)
    {
        await MuteAllAsync(group.Id, mute).ConfigureAwait(false);
    }

    #endregion

    #region Essence

    /// <summary>
    ///     设置精华消息
    /// </summary>
    /// <param name="messageId">消息id</param>
    [Obsolete("此方法在mirai-api-http 2.6.0及以上版本会导致异常")]
    public  async Task SetEssenceMessageAsync(string messageId)
    {
        var payload = new
        {
            target = messageId
        };

        await PostJsonAsync(HttpEndpoints.SetEssence, payload).ConfigureAwait(false);
    }

    /// <summary>
    ///     设置精华消息
    /// </summary>
    /// <param name="receiver"></param>
    [Obsolete("此方法在mirai-api-http 2.6.0及以上版本会导致异常")]
    public  async Task SetEssenceMessageAsync(MessageReceiverBase receiver)
    {// todo 重构这里 
        await PostJsonAsync(HttpEndpoints.SetEssence, receiver.MessageChain.OfType<SourceMessage>().First().MessageId).ConfigureAwait(false);
    }

    /// <summary>
    ///     设置精华消息
    /// </summary>
    /// <param name="messageId">消息id</param>
    /// <param name="target">群id</param>
    public  async Task SetEssenceMessageAsync(string messageId, string target)
    {
        var payload = new
        {
            messageId,
            target
        };

        await PostJsonAsync(HttpEndpoints.SetEssence, payload).ConfigureAwait(false);
    }

    /// <summary>
    ///     设置精华消息
    /// </summary>
    /// <param name="receiver"></param>
    public  async Task SetEssenceMessageAsync(GroupMessageReceiver receiver)
    {
        await SetEssenceMessageAsync(receiver.MessageChain.OfType<SourceMessage>().First().MessageId, receiver.GroupId).ConfigureAwait(false);
    }

    #endregion

    #region GroupSetting

    /// <summary>
    ///     获取群设置
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public  async Task<GroupSetting> GetGroupSettingAsync(string target)
    {
        var response = await GetAsync(HttpEndpoints.GroupConfig, new { target }).ConfigureAwait(false);

        return JsonConvert.DeserializeObject<GroupSetting>(response);
    }

    /// <summary>
    ///     获取群设置
    /// </summary>
    /// <param name="group"></param>
    /// <returns></returns>
    public  async Task<GroupSetting> GetGroupSettingAsync(Group group)
    {
        return await GetGroupSettingAsync(group.Id).ConfigureAwait(false);
    }


    /// <summary>
    ///     修改群设置
    /// </summary>
    /// <param name="target"></param>
    /// <param name="setting"></param>
    public  async Task SetGroupSettingAsync(string target, GroupSetting setting)
    {
        var payload = new
        {
            target,
            config = setting
        };

        await PostJsonAsync(HttpEndpoints.GroupConfig, payload).ConfigureAwait(false);
    }

    /// <summary>
    ///     修改群设置
    /// </summary>
    /// <param name="group"></param>
    /// <param name="setting"></param>
    public  async Task SetGroupSettingAsync(Group group, GroupSetting setting)
    {
        await SetGroupSettingAsync(group.Id, setting).ConfigureAwait(false);
    }

    #endregion

    #region MemberInfo

    /// <summary>
    ///     获取群员
    /// </summary>
    /// <param name="memberQQ"></param>
    /// <param name="group"></param>
    /// <returns></returns>
    public  async Task<Member> GetMemberAsync(string memberQQ, string group)
    {
        var response = await GetAsync(HttpEndpoints.MemberInfo, new
        {
            target = group,
            memberId = memberQQ
        }).ConfigureAwait(false);

        return JsonConvert.DeserializeObject<Member>(response);
    }

    /// <summary>
    ///     修改群员设置,需要相关的权限
    /// </summary>
    /// <param name="memberQQ"></param>
    /// <param name="group"></param>
    /// <param name="card">群名片, 需要管理员权限</param>
    /// <param name="title">群头衔, 需要群主权限</param>
    /// <returns></returns>
    public  async Task<Member> SetMemberInfoAsync(string memberQQ, string group, string card = null,
        string title = null)
    {
        var payload = new
        {
            target = group,
            memberId = memberQQ,
            info = new
            {
                name = card,
                specialTitle = title
            }
        };

        await PostJsonAsync(HttpEndpoints.MemberInfo, payload).ConfigureAwait(false);

        return await GetMemberAsync(memberQQ, group).ConfigureAwait(false);
    }

    #endregion
}