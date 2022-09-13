using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Chaldene.Data.Shared;

/// <summary>
/// 群
/// </summary>
public record Group
{
    /// <summary>
    ///     群号
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    ///     群名称
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    ///     权限类型
    /// </summary>
    [JsonProperty("permission")]
    [JsonConverter(typeof(StringEnumConverter))]
    public Permissions Permission { get; set; }

    public static implicit operator GroupId(Group group)
    {
        return group.Id;
    }

}

/// <summary>
/// 群号
/// </summary>
public struct GroupId
{
    /// <summary>
    /// 群号
    /// </summary>
    public string Id { get; }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    public GroupId(string id)
    {
        Id = id;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="qq"></param>
    /// <returns></returns>
    public static implicit operator GroupId(string qq)
    {
        return new GroupId(qq);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="group"></param>
    /// <returns></returns>
    public static implicit operator string(GroupId group)
    {
        return group.Id;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="qq"></param>
    /// <returns></returns>
    public static implicit operator GroupId(long qq)
    {
        return new GroupId(qq.ToString());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(GroupId other)
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
        return obj is GroupId other && Equals(other);
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
    public static bool operator ==(GroupId left, GroupId right)
    {
        return left.Equals(right);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(GroupId left, GroupId right)
    {
        return !left.Equals(right);
    }
}