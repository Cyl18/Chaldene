using System.Linq;
using Manganese.Text;
using Chaldene.Data.Exceptions;

namespace Chaldene.Data.Sessions
{
    
/// <summary>
/// 连接配置
/// </summary>
public record ConnectionConfig
{
    /// <summary>
    /// http连接配置
    /// </summary>
    public AdapterConfig HttpAddress { get; set; }

    /// <summary>
    /// websocket连接配置
    /// </summary>
    public AdapterConfig WebsocketAddress { get; set; }

    /// <summary>
    /// 从地址构造连接配置
    /// </summary>
    /// <param name="address"></param>
    /// <returns></returns>
    public static implicit operator ConnectionConfig(string address)
    {
        return new ConnectionConfig
        {
            HttpAddress = address,
            WebsocketAddress = address
        };
    }
    
    /// <summary>
    /// 实例化一个连接配置
    /// </summary>
    public ConnectionConfig()
    {
    }

    /// <summary>
    /// 从地址构造连接配置, HTTP 和 Websocket 地址相同
    /// </summary>
    /// <param name="address"></param>
    public ConnectionConfig(string address)
    {
        HttpAddress = address;
        WebsocketAddress = address;
    }

    /// <summary>
    /// 连接配置转换成地址
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    /// <exception cref="InvalidAddressException"></exception>
    public static implicit operator string(ConnectionConfig config)
    {
        if (config.HttpAddress != config.WebsocketAddress)
        {
            throw new InvalidAddressException("只有相同的地址才能转换为字符串");
        }

        return $"{config.HttpAddress.Host}:{config.HttpAddress.Port}";
    }
    
    /// <summary>
    /// 从地址构造连接配置
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public static implicit operator AdapterConfig(ConnectionConfig config)
    {
        return config.HttpAddress;
    }

    /// <summary>
    /// 适配器配置
    /// </summary>
    /// <param name="Host"></param>
    /// <param name="Port"></param>
    public record AdapterConfig(string Host, string Port)
    {
        /// <summary>
        /// 从string自动转换成ip:port对
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static implicit operator AdapterConfig(string address)
        {
            address = address.TrimEnd('/').Empty("http://").Empty("https://");
            if (!address.Contains(':')) throw new InvalidAddressException($"错误的地址: {address}");

            var split = address.Split(':');
            
            if (split.Length != 2) throw new InvalidAddressException($"错误的地址: {address}");
            if (!split.Last().IsInteger()) throw new InvalidAddressException($"错误的地址: {address}");

            return new AdapterConfig(split[0], split[1]);
        }

        /// <summary>
        /// 转换为string
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static implicit operator string(AdapterConfig config)
        {
            return $"{config.Host}:{config.Port}";
        }

        /// <summary>
        /// this
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this;
        }
    }
}
}


namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit {}
}