using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Manganese.Text;
using Chaldene.Data.Sessions;
using Chaldene.Data.Shared;
using Newtonsoft.Json;

namespace Chaldene.Sessions;

/// <summary>
///     账号管理器
/// </summary>
public partial class MiraiBot
{
    #region Private helpers

    private async Task<IEnumerable<T>> GetCollectionAsync<T>(HttpEndpoints endpoints, object extra = null)
    {
        var raw = await GetAsync(endpoints, extra).ConfigureAwait(false);
        raw = raw.Fetch("data");

        return raw.ToJArray().Select(x => x.ToObject<T>());
    }

    private async Task<Profile> GetProfileAsync(HttpEndpoints endpoints, object extra = null)
    {
        var raw = await GetAsync(endpoints, extra).ConfigureAwait(false);

        return JsonConvert.DeserializeObject<Profile>(raw);
    }

    #endregion

    #region Exposed

    /// <summary>
    ///     获取bot账号的好友列表
    /// </summary>
    public async Task<IEnumerable<Friend>> GetFriendsAsync()
    {
        return await GetCollectionAsync<Friend>(HttpEndpoints.FriendList).ConfigureAwait(false);
    }

    /// <summary>
    ///     获取bot账号的QQ群列表
    /// </summary>
    public async Task<IEnumerable<Group>> GetGroupsAsync()
    {
        return await GetCollectionAsync<Group>(HttpEndpoints.GroupList).ConfigureAwait(false);
    }

    /// <summary>
    ///     获取某群的全部群成员
    /// </summary>
    public async Task<IEnumerable<Member>> GetGroupMembersAsync(GroupId target)
    {
        return await GetCollectionAsync<Member>(HttpEndpoints.MemberList, new
        {
            target
        }).ConfigureAwait(false);
    }

    /// <summary>
    ///     删除好友
    /// </summary>
    /// <param name="target"></param>
    public async Task DeleteFriendAsync(UserId target)
    {
        _ = await PostJsonAsync(HttpEndpoints.DeleteFriend, new
        {
            target
        }).ConfigureAwait(false);
    }

    /// <summary>
    ///     获取bot资料
    /// </summary>
    public async Task<Profile> GetBotProfileAsync()
    {
        return await GetProfileAsync(HttpEndpoints.BotProfile).ConfigureAwait(false);
    }

    /// <summary>
    ///     获取好友资料
    /// </summary>
    public async Task<Profile> GetFriendProfileAsync(UserId target)
    {
        return await GetProfileAsync(HttpEndpoints.FriendProfile, new
        {
            target
        }).ConfigureAwait(false);
    }

    /// <summary>
    ///     获取群员资料
    /// </summary>
    /// <param name="id"></param>
    /// <param name="target">群号</param>
    public async Task<Profile> GetMemberProfileAsync(GroupId id, UserId target)
    {
        return await GetProfileAsync(HttpEndpoints.MemberProfile, new
        {
            target,
            memberId = id
        }).ConfigureAwait(false);
    }

    /// <summary>
    ///     获取群员资料
    /// </summary>
    public async Task<Profile> GetMemberProfileAsync(Member member)
    {
        return await GetMemberProfileAsync(member.Id, member.Group.Id).ConfigureAwait(false);
    }

    /// <summary>
    ///     获取任意QQ的资料（需要mirai-api-http 2.4.0及以上）
    /// </summary>
    public async Task<Profile> GetProfileAsync(UserId target)
    {
        return await GetProfileAsync(HttpEndpoints.UserProfile, new
        {
            target
        }).ConfigureAwait(false);
    }

    #endregion
}