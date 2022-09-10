# Chaldene

Chaldene 是 [Mirai.Net](https://github.com/SinoAHpx/Mirai.Net) 项目的简化重构版 fork ([已经取得作者授权](docs/images/授权.png)), 原作者 [SinoAHpx](https://github.com/SinoAHpx), 是基于 [mirai-api-http] 实现的 C# 版超轻量级 [mirai] 社区 SDK.

此项目遵循 [AGPL-3.0](https://github.com/Cyl18/Chaldene/blob/master/LICENSE) 协议开源.

## Simple as it is.

![1](docs/images/simple.gif)

有的时候, 你需要实现一些特别简单的功能, 想迅速写出一个机器人.

```shell
> Install-Package Chaldene
```

```csharp
var bot = new MiraiBot("localhost:5000", "*******", 780712);
await bot.LaunchAsync();

bot.GroupMessageReceived += async (sender, args) =>
{
    Console.WriteLine($"接收到消息: {args.MessageChain.GetPlainMessage()}");
};

await bot.SendFriendMessageAsync(233656, "橘子!");
```

好了, 你学会使用 Chaldene 了♥

## 对比

|                             |     Chaldene      |     Mirai.Net     | Cocoa  | Hyperai<sup>[1]</sup> |    Mirai-CSharp    |
| :-------------------------: | :---------------: | :---------------: | :----: | :-----: | :---------------: |
|          .NET 版本          | .NET Standard 2.0 | .NET Standard 2.0 | .NET 5 | .NET 5  | .NET Standard 2.0 |
|     单程序支持多个 Bot      |        ✅         |        ⛔         |   ⛔   |   ✅    |        ✅         |
|         支持 HTTPS          |        ✅         |        ⛔         |   ⛔   |   🟡<sup>[2]</sup>    |        ⛔         |
| 使用 ConfigureAwait(false)  |        ✅         |        ⛔         |   ⛔   |   🟡<sup>[2]</sup>    |     🟡(部分)      |
|       复杂的 MVC/DSL/事件订阅/DI        |        ⛔         |        🟡(部分)         |   ✅   |   ✅    |        ✅         |
|         Native 依赖         |        ⛔         |        ⛔         |   ⛔   |   ⛔    |        ✅         |
|          不会卖萌           |        ⛔         |        ✅         |   ✅   |   ✅    |        ✅         |
| 写出 HelloWorld 所需行数<sup>[3]</sup> |        6         |        ~6         | ⛔<sup>[4]</sup>  |   8<sup>[5]</sup>    |        24         |
|      文档完善度(主观)       |       你猜        |       中高        |   低   | 看不懂  |  很高但是看不懂   |
|          学习成本(主观)           |     **极低**      |        低         |  中高  |  极高   |        高         |

[1] Hyperai 更像是一个脚手架, 功能由插件提供, 下面的内容以[这个插件](https://github.com/d3ara1n/Ac682.Hyperai.Clients.Mirai)为准  
[2] 作者说需要插件支持, 但是我找不到这样的现成插件  
[3] 指发一条消息和自动回复消息的有效代码(不含大括号和空格)行数, 可能不客观, 详细参见牢骚  
[4] 我找不到主动发消息的方法  
[5] 作者原话 "远多于8行", 此处行数指代使用上方插件的行数

想看我发牢骚的话, [牢骚](docs/complicate.md)中有详细对比.

## 和 Mirai.Net 的区别在哪?

Chaldene 从 [Mirai.Net 2.4.4](https://github.com/SinoAHpx/Mirai.Net/tree/2.4) 分支 fork 而来, 原作者比较忙. Chaldene 定期会拉取上游更新, 相比 Mirai.Net 的主要区别和改动有:

- 为 SendMessage 加入了 `params MessageBase[]`, 简化消息发送
- 将 EventReceived 和 MessageReceived 整合到 MiraiBot 中显式声明
- 将 Account/File/Group/Message/Request Manager 整合到 MiraiBot 中显式声明
- 将群号和个人QQ号使用强类型声明
- 为发送图片加入了流支持
- 使用了 `.ConfigureAwait(false)`
- 支持多机器人实例
- 支持 HTTPS (`MiraiBot.UseHttps = true`)
- 加入 `MiraiBot.Connected`/`MiraiBot.UseAutoReconnect()`
- 部分 API 改动
- 补全一点异常文档

## 那为什么要写这个库? 

- [牢骚](docs/complicate.md)

## 但是我真的想看文档?

- [mirai-api-http 对接 Chaldene](docs/mirai-api-http.md)  
- [自动重连/异常处理/HTTPS](docs/additions.md)

其它内容通过 IDE 自动补全或者查阅原项目文档即可.

如果有什么可以做的更好的地方, 期待你的反馈！

---

## 闲聊群

加 [Mirai.Net 的闲聊群 752379554](https://jq.qq.com/?_wv=1027&k=gdWqppEO)~

## 感谢

感谢 [原作者 SinoAHpx](https://github.com/SinoAHpx) 创造了 Mirai.Net.

## 原项目文档

<details> <summary>原项目文档内容</summary>

## 速览

- 基于 [.NET Standard 2.0](https://docs.microsoft.com/en-us/dotnet/standard/net-standard) 开发，支持跨平台。
- 适配最新的 [mirai-api-http] 插件。
- 实现了 [mirai-api-http] 的 `Http Adapter` 和 `Websocket Adapter`
  - `Http Adapter` 用来进行发送操作。
  - `Websocket Adapter` 用来进行接收操作。
- 基于 [Rx.NET](https://github.com/dotnet/reactive) 的推送系统。
- 有一堆好用的脚手架和拓展方法。
- 提供了简单的模块化和命令系统实现。
- 源代码结构
  - Mirai.Net，主项目
  - Mirai.Net.Test，控制台测试项目
  - Mirai.Net.UnitTest，单元测试项目（现在没啥用了）

<details>
  <summary>实现的接口列表</summary>

_斜体的标注的接口是不稳定的_

~~删除线标注的接口是未实现的~~

- 账号信息
  - 获取好友列表
  - 获取群列表
  - 获取群成员列表
  - 获取 Bot 资料
  - 获取好友资料
  - 获取群成员资料
  - 获取陌生人资料
- 消息发送和撤回
  - 发送好友消息
  - 发送群消息
  - 发送临时会话消息
  - 发送头像戳一戳消息
  - 撤回消息
  - 根据消息 id 获取消息链
- 文件操作
  - 查看文件列表
  - 获取文件信息
  - 创建文件夹
  - 删除文件
  - 移动文件
  - 重命名文件
- 多媒体内容上传
  - 图片文件上传
  - 语音文件上传
  - 群文件上传
- 账号管理
  - 删除好友
- 群管理
  - 禁言群成员
  - 解除群成员禁言
  - 移除群成员
  - 退出群聊
  - 全体禁言
  - 解除全体禁言
  - 设置群精华消息
  - 获取群设置
  - 修改群设置
  - 获取群员设置
  - 修改群员设置
- 事件处理
  - 添加好友申请
  - 用户入群申请
  - Bot 被邀请入群申请

</details>

<details>
  <summary>支持的消息类型</summary>

- Quote - 回复消息
- At - @消息
- AtAll - @全体成员
- Face - QQ 表情
- Plain - 纯文本
- Image - 图片
- FlashImage - 闪照
- Voice - 语音
- Xml - XML 消息
- Json - JSON 消息
- App - App 消息
- Poke - 戳一戳
- Dice - 不知道是啥玩意
- MusicShare - 音乐分享
- ForwardMessage - 转发消息
- File - 文件
- MarketFace - 商城表情
- MiraiCode - Mirai 码

</details>

## 快速上手

**(以下仅为一些简单示例，如果需要更详细的说明，请移步[文档]。有时候文档跟不上版本请[进群提问](#mirainet-239)**

### 安装

- 使用 Nuget 安装(推荐)
  - Nuget 包管理器: `Install-Package Mirai.Net`
  - .NET CLI: `dotnet add package Mirai.Net`
  - **或者在 IDE 的可视化界面搜索`Mirai.Net`安装最新版。**
- 自己克隆这个仓库的默认分支，然后自己编译，然后自己添加 dll 引用。

### 创建和启动 Bot

<details>
  <summary>名称空间引用</summary>

```cs
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions;
```

</details>

```cs
using var bot = new MiraiBot
{
    Address = "localhost:8080",
    QQ = "xx",
    VerifyKey = "xx"
};
```

(因为`MiraiBot`类实现了`IDisposable`接口，所以可以使用`using`关键字)

`Address`和`VerifyKey`来自`mirai-api-http`的配置文件，`QQ`就是`Mirai Console`已登录的机器人的 QQ 号。

创建完`MiraiBot`实例之后，就可以启动了:

```cs
await bot.LaunchAsync();
```

### 监听事件和消息

`MiraiBot`类暴露两个属性: `EventReceived`和`MessageReceived`，订阅它们就可以监听事件和消息。

下面的例子就是过滤出接收到的`好友请求事件`事件，然后把它从`EventBase`转换成具体的`NewFriendRequestedEvent`，最后才是订阅器。

(消息的订阅器也是同样的)

```cs
bot.EventReceived
    .OfType<NewFriendRequestedEvent>()
    .Subscribe(x =>
    {
        //do things
    });
```

### Hello, World

`Mirai.Net`通过一系列的`xxManager`(**这些管理器都是静态类。**)来进行主动操作，其中，消息相关的管理器为`MessageManager`。

#### 发送消息

这里以发送群消息作为演示，实际上还可以发送好友消息，临时消息和戳一戳消息。

发送消息的方法有两个参数: 发送到哪里和发送什么。所以第一个参数就是发消息的群号，第二个参数就是要发送的消息链(或者字符串)。

```cs
await MessageManager.SendGroupMessageAsync("xx", "Hello, World");
```

或者:

```cs
await MessageManager.SendGroupMessageAsync("xx", new MessageChainBuilder().Plain("Hello, ").At("xx").Build());
```

## 贡献

此项目欢迎任何人的 [Pull Request](https://github.com/AHpxChina/Mirai.Net/pulls) 和 [Issue](https://github.com/AHpxChina/Mirai.Net/issues) 也欢迎 Star 和 Fork。

如果你认为文档不够好，也欢迎对 [文档仓库](https://github.com/SinoAHpx/Mirai.Net.Documents) 提交 [Pull Request](https://github.com/AHpxChina/Mirai.Net.Documents/pulls) 和 [Issue](https://github.com/AHpxChina/Mirai.Net.Documents/issues)。

## 致谢

- [mirai]
- [mirai-api-http]
- [Jetbrains](https://www.jetbrains.com/)
- [Flurl](https://flurl.dev/)
- [Json.NET](http://json.net/) ~~这甚至是这个项目名称的灵感来源~~
- [Websocket.Client](https://github.com/Marfusios/websocket-client)
- [Rx.NET](https://github.com/dotnet/reactive)
- [Manganese](https://github.com/SinoAHpx/Manganese)

</details>

[mirai-api-http]: https://github.com/project-mirai/mirai-api-http
[mirai]: https://github.com/mamoe/mirai
[文档]: https://sinoahpx.github.io/Mirai.Net.Documents/
