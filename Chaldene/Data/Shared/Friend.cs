﻿using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Chaldene.Data.Shared;

/// <summary>
/// 好友
/// </summary>
public record Friend
{
    // /// <summary>
    // /// 好友的资料
    // /// </summary>
    // [JsonIgnore] public Task<Profile> FriendProfile => this.GetFriendProfileAsync();

    /// <summary>
    ///     好友的QQ号
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    ///     好友的昵称
    /// </summary>
    [JsonProperty("nickname")]
    public string NickName { get; set; }

    /// <summary>
    ///     你给好友的备注
    /// </summary>
    [JsonProperty("remark")]
    public string Remark { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="friend"></param>
    /// <returns></returns>
    public static implicit operator UserId(Friend friend)
    {
        return new UserId(friend.Id);
    }
}

/// <summary>
/// 个人QQ号
/// </summary>
public struct UserId
{
    /// <summary>
    /// QQ号
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    public UserId(string id)
    {
        Id = id;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="qq"></param>
    /// <returns></returns>
    public static implicit operator UserId(string qq)
    {
        return new UserId(qq);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public static implicit operator string(UserId userId)
    {
        return userId.Id;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="qq"></param>
    /// <returns></returns>
    public static implicit operator UserId(long qq)
    {
        return new UserId(qq.ToString());
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(UserId other)
    {
        return Id == other.Id;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
        return obj is UserId other && Equals(other);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        return (Id != null ? Id.GetHashCode() : 0);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(UserId left, UserId right)
    {
        return left.Equals(right);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(UserId left, UserId right)
    {
        return !left.Equals(right);
    }
}