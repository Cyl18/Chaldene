using System;
using System.Linq;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Flurl;
using Manganese.Text;
using Chaldene.Data.Events;
using Chaldene.Data.Events.Concretes;
using Chaldene.Data.Events.Concretes.Bot;
using Chaldene.Data.Events.Concretes.Friend;
using Chaldene.Data.Events.Concretes.Group;
using Chaldene.Data.Events.Concretes.Message;
using Chaldene.Data.Events.Concretes.OtherClient;
using Chaldene.Data.Events.Concretes.Request;
using Chaldene.Data.Exceptions;
using Chaldene.Data.Messages;
using Chaldene.Data.Messages.Concretes;
using Chaldene.Data.Messages.Receivers;
using Chaldene.Data.Sessions;
using Chaldene.Data.Shared;
using Chaldene.Utils.Internal;
using Chaldene.Utils.Scaffolds;
using Flurl.Http;
using Newtonsoft.Json;
using Websocket.Client;
using Websocket.Client.Exceptions;

namespace Chaldene.Sessions;

/// <summary>
///     mirai-api-http机器人描述
/// </summary>
public partial class MiraiBot : IDisposable
{
    #region MessageReceivers

    /// <summary>
    /// 接收到的消息
    /// </summary>
    public event CommonEventHandler<FriendMessageReceiver> FriendMessageReceived
    {
        add => MessageReceived.OfType<FriendMessageReceiver>().Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// 接收到群消息
    /// </summary>
    public event CommonEventHandler<GroupMessageReceiver> GroupMessageReceived
    {
        add => MessageReceived.OfType<GroupMessageReceiver>().Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// 接收到临时消息
    /// </summary>
    public event CommonEventHandler<TempMessageReceiver> TempMessageReceived
    {
        add => MessageReceived.OfType<TempMessageReceiver>().Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// 接收到陌生人的消息
    /// </summary>
    public event CommonEventHandler<StrangerMessageReceiver> StrangerMessageReceived
    {
        add => MessageReceived.OfType<StrangerMessageReceiver>().Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// 接收到其它客户端的消息
    /// </summary>
    public event CommonEventHandler<OtherClientMessageReceiver> OtherClientMessageReceived
    {
        add => MessageReceived.OfType<OtherClientMessageReceiver>().Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// 未知类型的消息接收器
    /// </summary>
    public event CommonEventHandler<UnknownReceiver> UnknownChatMessageReceived
    {
        add => MessageReceived.OfType<UnknownReceiver>().Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    #endregion

    #region Events

    /// <summary>
    /// 未知的事件
    /// </summary>
    public event CommonEventHandler<UnknownEvent> UnknownEventReceived
    {
        add => EventReceived.OfType<UnknownEvent>().Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// 新的好友请求
    /// </summary>
    public event CommonEventHandler<NewFriendRequestedEvent> NewFriendRequestedEvent
    {
        add => EventReceived.OfType<NewFriendRequestedEvent>().Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// 新的邀请（邀请bot加入某群）
    /// </summary>
    public event CommonEventHandler<NewInvitationRequestedEvent> NewInvitationRequestedEvent
    {
        add => EventReceived.OfType<NewInvitationRequestedEvent>().Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// 新成员申请
    /// </summary>
    public event CommonEventHandler<NewMemberRequestedEvent> NewMemberRequestedEvent
    {
        add => EventReceived.OfType<NewMemberRequestedEvent>().Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// 其它客户端离线
    /// </summary>
    public event CommonEventHandler<OtherClientOfflineEvent> OtherClientOfflineEvent
    {
        add => EventReceived.OfType<OtherClientOfflineEvent>().Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// 其它客户端上线
    /// </summary>
    public event CommonEventHandler<OtherClientOnlineEvent> OtherClientOnlineEvent
    {
        add => EventReceived.OfType<OtherClientOnlineEvent>().Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// Bot被人at
    /// </summary>
    public event CommonEventHandler<AtEvent> AtEvent
    {
        add => EventReceived.OfType<AtEvent>().Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// 某人被戳了一戳
    /// </summary>
    public event CommonEventHandler<NudgeEvent> NudgeEvent
    {
        add => EventReceived.OfType<NudgeEvent>().Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// 群内是否允许匿名聊天的状态改变
    /// </summary>
    public event CommonEventHandler<GroupAllowedAnonymousChatEvent> GroupAllowedAnonymousChatEvent
    {
        add => EventReceived.OfType<GroupAllowedAnonymousChatEvent>()
            .Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// 群内是否允许坦白说的状态发生改变
    /// </summary>
    public event CommonEventHandler<GroupAllowedConfessTalkChanged> GroupAllowedConfessTalkChanged
    {
        add => EventReceived.OfType<GroupAllowedConfessTalkChanged>()
            .Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// 群内是否允许群员邀请新成员的状态发生改变
    /// </summary>
    public event CommonEventHandler<GroupAllowedMemberInviteEvent> GroupAllowedMemberInviteEvent
    {
        add => EventReceived.OfType<GroupAllowedMemberInviteEvent>()
            .Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// 入群公告发生改变
    /// </summary>
    public event CommonEventHandler<GroupEntranceAnnouncementChangedEvent> GroupEntranceAnnouncementChangedEvent
    {
        add => EventReceived.OfType<GroupEntranceAnnouncementChangedEvent>()
            .Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// 某条群消息被撤回
    /// </summary>
    public event CommonEventHandler<GroupMessageRecalledEvent> GroupMessageRecalledEvent
    {
        add => EventReceived.OfType<GroupMessageRecalledEvent>().Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// 某群开启/关闭了全员禁言
    /// </summary>
    public event CommonEventHandler<GroupMutedAllEvent> GroupMutedAllEvent
    {
        add => EventReceived.OfType<GroupMutedAllEvent>().Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// 某群的改了群名
    /// </summary>
    public event CommonEventHandler<GroupNameChangedEvent> GroupNameChangedEvent
    {
        add => EventReceived.OfType<GroupNameChangedEvent>().Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// Bot加入了一个新群
    /// </summary>
    public event CommonEventHandler<JoinedEvent> JoinedEvent
    {
        add => EventReceived.OfType<JoinedEvent>().Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// Bot被某群踢了
    /// </summary>
    public event CommonEventHandler<KickedEvent> KickedEvent
    {
        add => EventReceived.OfType<KickedEvent>().Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// Bot主动离开了某群
    /// </summary>
    public event CommonEventHandler<LeftEvent> LeftEvent
    {
        add => EventReceived.OfType<LeftEvent>().Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// 某人的群名片改变
    /// </summary>
    public event CommonEventHandler<MemberCardChangedEvent> MemberCardChangedEvent
    {
        add => EventReceived.OfType<MemberCardChangedEvent>().Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// 群员称号改变
    /// </summary>
    public event CommonEventHandler<MemberHonorChangedEvent> MemberHonorChangedEvent
    {
        add => EventReceived.OfType<MemberHonorChangedEvent>().Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// 新成员入群
    /// </summary>
    public event CommonEventHandler<MemberJoinedEvent> MemberJoinedEvent
    {
        add => EventReceived.OfType<MemberJoinedEvent>().Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// 某群员被踢出群
    /// </summary>
    public event CommonEventHandler<MemberKickedEvent> MemberKickedEvent
    {
        add => EventReceived.OfType<MemberKickedEvent>().Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// 群成员离开群
    /// </summary>
    public event CommonEventHandler<MemberLeftEvent> MemberLeftEvent
    {
        add => EventReceived.OfType<MemberLeftEvent>().Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// 某群员被禁言
    /// </summary>
    public event CommonEventHandler<MemberMutedEvent> MemberMutedEvent
    {
        add => EventReceived.OfType<MemberMutedEvent>().Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// 某群员权限改变，操作者一定是群主
    /// </summary>
    public event CommonEventHandler<MemberPermissionChangedEvent> MemberPermissionChangedEvent
    {
        add => EventReceived.OfType<MemberPermissionChangedEvent>().Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// 群头衔改动（只有群主有操作限权）
    /// </summary>
    public event CommonEventHandler<MemberTitleChangedEvent> MemberTitleChangedEvent
    {
        add => EventReceived.OfType<MemberTitleChangedEvent>().Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// 群员被解除禁言
    /// </summary>
    public event CommonEventHandler<MemberUnmutedEvent> MemberUnmutedEvent
    {
        add => EventReceived.OfType<MemberUnmutedEvent>().Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// Bot被禁言
    /// </summary>
    public event CommonEventHandler<MutedEvent> MutedEvent
    {
        add => EventReceived.OfType<MutedEvent>().Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// Bot在群内的权限改变，操作者一定是群主
    /// </summary>
    public event CommonEventHandler<PermissionChangedEvent> PermissionChangedEvent
    {
        add => EventReceived.OfType<PermissionChangedEvent>().Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// Bot被解除禁言
    /// </summary>
    public event CommonEventHandler<UnmutedEvent> UnmutedEvent
    {
        add => EventReceived.OfType<UnmutedEvent>().Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// 好友输入状态改变
    /// </summary>
    public event CommonEventHandler<FriendInputStatusChangedEvent> FriendInputStatusChangedEvent
    {
        add => EventReceived.OfType<FriendInputStatusChangedEvent>()
            .Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// 好友昵称改变
    /// </summary>
    public event CommonEventHandler<FriendNickChangedEvent> FriendNickChangedEvent
    {
        add => EventReceived.OfType<FriendNickChangedEvent>().Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// 好友撤回了某条消息
    /// </summary>
    public event CommonEventHandler<FriendRecalledEvent> FriendRecalledEvent
    {
        add => EventReceived.OfType<FriendRecalledEvent>().Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// Bot被服务器断开或因网络问题而掉线
    /// </summary>
    public event CommonEventHandler<DroppedEvent> DroppedEvent
    {
        add => EventReceived.OfType<DroppedEvent>().Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// Bot主动离线
    /// </summary>
    public event CommonEventHandler<OfflineEvent> OfflineEvent
    {
        add => EventReceived.OfType<OfflineEvent>().Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// Bot被挤下线
    /// </summary>
    public event CommonEventHandler<OfflineForceEvent> OfflineForceEvent
    {
        add => EventReceived.OfType<OfflineForceEvent>().Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// Bot自身上线
    /// </summary>
    public event CommonEventHandler<OnlineEvent> OnlineEvent
    {
        add => EventReceived.OfType<OnlineEvent>().Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// Bot主动重新登录
    /// </summary>
    public event CommonEventHandler<ReconnectedEvent> ReconnectedEvent
    {
        add => EventReceived.OfType<ReconnectedEvent>().Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }

    #endregion

    /// <summary>
    /// 构造一个 <see cref="MiraiBot"/> 实例
    /// </summary>
    /// <param name="address">mirai-api-http本地服务器地址，比如：localhost:2333，或者构造一个ConnectionConfig对象</param>
    /// <param name="verifyKey">verifyKey</param>
    /// <param name="qq">建立连接的QQ账号</param>
    public MiraiBot(string address, string verifyKey, UserId qq)
    {
        Address = address;
        VerifyKey = verifyKey;
        QQ = qq;
    }

    /// <summary>
    /// 一定要记得初始化 Address, VerifyKey, QQ 哟
    /// </summary>
    public MiraiBot()
    {
        
    }

    /// <summary>
    /// 销毁当前对象
    /// </summary>
    public async void Dispose()
    {
        await ReleaseAsync().ConfigureAwait(false);
        _client.Dispose();
    }

    #region Exposed

    /// <summary>
    /// 启动bot
    /// </summary>
    /// <exception cref="FlurlHttpException"></exception>
    /// <exception cref="WebsocketException"></exception>
    public async Task LaunchAsync()
    {
        //Instance = this;


        await VerifyAsync().ConfigureAwait(false);
        await BindAsync().ConfigureAwait(false);
        await StartWebsocketListenerAsync().ConfigureAwait(false);
        Connected = true;
    }

    /// <summary>
    /// 标识 WebSocket 连接是否已经建立
    /// </summary>
    public bool Connected { get; set; }

    #endregion

    #region Properties

    // /// <summary>
    // ///     最后一个启动的MiraiBot实例
    // /// </summary>
    // [JsonIgnore]
    // public static MiraiBot Instance { get; set; }

    [JsonIgnore] internal string HttpSessionKey { get; set; }

    [JsonIgnore] private string _qq;
    [JsonIgnore] private WebsocketClient _client;

    /// <summary>
    ///     mirai-api-http本地服务器地址，比如：localhost:114514，或者构造一个ConnectConfig对象
    ///     <exception cref="InvalidAddressException">传入错误的地址将会抛出异常</exception>
    /// </summary>
    public ConnectionConfig Address { get; set; }

    /// <summary>
    /// 是否使用https/wss连接
    /// </summary>
    public bool UseHttps { get; set; } = false;

    string HttpScheme => UseHttps ? "https" : "http";
    string WebsocketScheme => UseHttps ? "wss" : "ws";
    /// <summary>
    /// 使用自动重连<br/>
    /// 原理：在websocket断开连接后无限重试，但是不会输出任何报错<br/>
    /// 如果需要输出报错请手动重连 具体可以参考文档
    /// </summary>
    public void UseAutoReconnect()
    {
        Disconnected += async (_, _) =>
        {
            try
            {
                while (true)
                {
                    await LaunchAsync().ConfigureAwait(false);
                    break;
                }
            }
            catch (Exception)
            {
                await Task.Delay(1000);
                //
            }
        };
    }
    /// <summary>
    ///     建立连接的QQ账号
    /// </summary>
    public string QQ
    {
        get => _qq;
        set => _qq = value.ThrowIfNotInt64("错误的QQ号").ToString();
    }

    /// <summary>
    ///     Chaldene总是需要一个VerifyKey
    /// </summary>
    public string VerifyKey { get; set; }

    #endregion

    #region Handlers

    /// <summary>
    /// 接收到事件
    /// </summary>
    [JsonIgnore]
    internal IObservable<EventBase> EventReceived => _eventReceivedSubject.AsObservable();

    private readonly Subject<EventBase> _eventReceivedSubject = new();

    /// <summary>
    /// 收到消息
    /// </summary>
    [JsonIgnore]
    internal IObservable<MessageReceiverBase> MessageReceived => _messageReceivedSubject.AsObservable();

    private readonly Subject<MessageReceiverBase> _messageReceivedSubject = new();
    
    /// <summary>
    /// 接收到未知类型的Websocket消息
    /// </summary>
    public event CommonEventHandler<string> UnknownWebsocketMessageReceived
    {
        add => _unknownMessageReceived.Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    } 

    

    private readonly Subject<string> _unknownMessageReceived = new();

    /// <summary>
    /// Websocket断开连接
    /// </summary>
    public event CommonEventHandler<WebSocketCloseStatus> Disconnected
    {
        add => _disconnected.Subscribe(receiver => value.Invoke(this, receiver));
        remove => throw new NotImplementedException();
    }
    

    private readonly Subject<WebSocketCloseStatus> _disconnected = new();
    

    #endregion

    #region Http adapter private helpers

    /// <summary>
    ///     发送验证请求，获得未激活的session key
    /// </summary>
    /// <returns></returns>
    private async Task VerifyAsync()
    {
        var result = await PostJsonAsync(HttpEndpoints.Verify, new
        {
            verifyKey = VerifyKey
        }, false).ConfigureAwait(false);

        HttpSessionKey = result.Fetch("session");
    }

    /// <summary>
    ///     激活session key
    /// </summary>
    private async Task BindAsync()
    {
        _ = await PostJsonAsync(HttpEndpoints.Bind, new
        {
            sessionKey = HttpSessionKey,
            qq = QQ
        }, false).ConfigureAwait(false);
    }

    /// <summary>
    ///     释放已激活的session
    /// </summary>
    private async Task ReleaseAsync()
    {
        _ = await PostJsonAsync(HttpEndpoints.Release, new
        {
            sessionKey = HttpSessionKey,
            qq = QQ
        }, false).ConfigureAwait(false);
    }

    #endregion

    #region Websocket adapter private helpers

    /// <summary>
    ///     启动websocket监听
    /// </summary>
    private async Task StartWebsocketListenerAsync()
    {
        var url = $"{WebsocketScheme}://{Address.WebsocketAddress}/all"
            .SetQueryParam("verifyKey", VerifyKey)
            .SetQueryParam("qq", QQ)
            .ToUri();

        _client = new WebsocketClient(url)
        {
            IsReconnectionEnabled = false
        };

        await _client.StartOrFail().ConfigureAwait(false);

        _client.DisconnectionHappened
            .Subscribe(x =>
            {
                Connected = false;
                _disconnected.OnNext(x.CloseStatus ?? WebSocketCloseStatus.Empty);
            });

        _client.MessageReceived
            .Where(message => message.MessageType == WebSocketMessageType.Text)
            .Subscribe(message =>
            {
                var data = message.Text.Fetch("data");
                if (data == null || data.IsNullOrEmpty())
                {
                    throw new InvalidWebsocketReponseException("Websocket传回错误响应");
                }

                ProcessWebSocketData(data);
            });
    }

    private void ProcessWebSocketData(string data)
    {
        var dataType = data.Fetch("type");
        if (dataType == null || dataType.IsNullOrEmpty())
        {
            throw new InvalidWebsocketReponseException("Websocket传回错误的响应");
        }

        if (dataType.Contains("Message"))
        {
            var receiver = ReflectionUtils.GetMessageReceiverBase(data, this);

            var rawChain = data.Fetch("messageChain");
            if (rawChain == null || rawChain.IsNullOrEmpty())
            {
                throw new InvalidResponseException("Websocket传回错误的响应");
            }

            receiver.MessageChain = rawChain
                .ToJArray()
                .Select(token => ReflectionUtils.GetMessageBase(token.ToString()))
                .ToMessageChain();

            if (receiver.MessageChain.OfType<AtMessage>().Any(x => x.Target == QQ))
            {
                _eventReceivedSubject.OnNext(new AtEvent
                {
                    Receiver = (receiver as GroupMessageReceiver)!
                });
            }

            _messageReceivedSubject.OnNext(receiver);
        }
        else if (dataType.Contains("Event"))
        {
            _eventReceivedSubject.OnNext(ReflectionUtils.GetEventBase(data, this));
        }
        else
        {
            _unknownMessageReceived.OnNext(data);
        }
    }

    #endregion
}
/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public delegate void CommonEventHandler<T>(MiraiBot sender, T args);