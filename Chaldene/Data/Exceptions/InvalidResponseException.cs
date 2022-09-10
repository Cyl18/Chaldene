using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Chaldene.Data.Exceptions;

/// <summary>
/// 错误的响应
/// </summary>
[Serializable]
public class InvalidResponseException : Exception
{
    /// <summary>
    /// 返回的错误状态码
    /// </summary>
    public MiraiApiHttpStatusCodes? StatusCode { get; set; }
    //
    // For guidelines regarding the creation of new exception types, see
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
    // and
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
    //
    

    internal InvalidResponseException(string message, MiraiApiHttpStatusCodes? code) : base(message)
    {
        StatusCode = code;
    }
    
}

enum MiraiApiHttpStatusCodes
{
    [Description("success")] Success = 0,
    [Description("Auth Key错误")] AuthKeyFail = 1,
    [Description("指定Bot不存在")] NoBot = 2,
    [Description("Session失效或不存在")] IllegalSession = 3,
    [Description("Session未认证")] NotVerifySession = 4,
    [Description("指定对象不存在")] NoElement = 5,
    [Description("指定操作不支持")] NoOperateSupport = 6,
    [Description("无操作权限")] PermissionDenied = 10,
    [Description("Bot被禁言")] BotMuted = 20,
    [Description("消息过长")] MessageTooLarge = 30,
    [Description("无效参数")] InvalidParameter = 400,
}