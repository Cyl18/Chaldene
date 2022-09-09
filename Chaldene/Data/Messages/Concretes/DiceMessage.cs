using Newtonsoft.Json;

namespace Chaldene.Data.Messages.Concretes;

/// <summary>
/// 骰子消息
/// </summary>
public record DiceMessage : MessageBase
{
    /// <summary>
    /// 消息类型
    /// </summary>
    public override Messages Type { get; set; } = Messages.Dice;

    /// <summary>
    ///     点数
    /// </summary>
    [JsonProperty("value")]
    public string Value { get; set; }
}