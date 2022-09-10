# 对接 mirai-http-api v2

mirai-http-api 是一个 mcl 的插件, 用于与其它语言通过 http 方式对接. 如果你还没有配置好 mirai 并登录机器人, 请先配置好它.

[mirai-http-api v2](https://github.com/project-mirai/mirai-api-http) 插件默认生成的配置文件在 `./config/net.mamoe.mirai-api-http/setting.yml`, 长这样:

```yaml
# 默认生成的文件, 请不要使用
adapters: 
  - http
debug: false
enableVerify: true
verifyKey: INITKEYXeduDQuP
singleMode: false
cacheSize: 4096
persistenceFactory: 'built-in'
adapterSettings: {}
```

你需要为它加上 ws adapter, 把文件中的内容替换成下面的内容, 并修改 `verifyKey:` 后面的值为一串至少8位的密码, 用于机器人连接:

```yaml
adapters:
  - http
  - ws

enableVerify: true
# 请修改为你自己的 verifyKey 至少8位!
verifyKey: REPLACE_ME

debug: false
singleMode: false

cacheSize: 4096

adapterSettings:
  http:
    host: localhost
    port: 8080
    cors: ["*"]
    unreadQueueMaxSize: 100
  ws:
    host: localhost
    port: 8080
    reservedSyncId: -1
```

然后你就可以用 Chaldene 连接 mirai-api-http v2 了!

```csharp
var bot = new MiraiBot(address: "localhost:8080", verifyKey: "REPLACE_ME", qq: 123456);
await bot.LaunchAsync();
await bot.SendFriendMessageAsync(Constants.TestQQString, "Hi!");
```