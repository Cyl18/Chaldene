# 其它内容

## 异常处理

由于使用了 Flurl 库, 在遇到连接不上服务器等问题时的时候会抛出 `FlurlHttpException`;  
在遇到服务器返回的错误时, 如 VerifyKey 无效时会抛出 `InvalidResponseException`;  
遇到 WebSocket 相关问题时, 如没有启用 ws adapter 时会抛出 `WebSocketException`.

此外库中还有 `InvalidAddressException`/`InvalidQQException`/`InvalidWebsocketReponseException`

## 自动重连

`MiraiBot.UseAutoReconnect()` 即可自动重连, 原理是在失败后自动不停重试直到成功为止, 但是不会输出任何错误信息.

如果需要错误信息, 可以复制这段代码:

```csharp
bot.Disconnected += async (sender, args) =>
{
    while (true)
    {
        try
        {
            Console.WriteLine("mirai-api-http 连接断开, 正在尝试重连...");
            await sender.LaunchAsync();
            break;
        }
        catch (Exception)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }
};
```

## HTTPS

mirai-api-http 是不自带 HTTPS 功能的, 你需要使用类似 nginx 的反向代理来实现 HTTPS.

```csharp
var bot = new MiraiBot("xxx:443", VERIFY_KEY, BOT_QQ) { UseHttps = true };
```

## 线程安全

WebSocket 只接收消息, 而其它请求通过 http 发送, 所以理论上是线程安全的.
