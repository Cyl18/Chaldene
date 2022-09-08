﻿using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Manganese.Text;
using Mirai.Net.Data.Exceptions;
using Mirai.Net.Data.Sessions;
using Mirai.Net.Sessions;

namespace Mirai.Net.Utils.Internal;

internal static class MiraiHttpUtils
{
    #region Guarantee

    /// <summary>
    ///     根据json判断这个json是否是正确的，否则抛出异常
    /// </summary>
    /// <param name="json"></param>
    /// <param name="appendix"></param>
    internal static void EnsureSuccess(this string json, string appendix = null)
    {
        var obj = json.ToJObject();

        if (obj.ContainsKey("code"))
        {
            var code = obj.Fetch("code");
            if (code != "0")
            {
                var message = $"原因: {json.OfErrorMessage()}";

                if (!appendix.IsNullOrEmpty())
                    message += $"\r\n备注: {appendix}";
                else
                    message += $"\r\n备注: {MiraiBot.Instance.ToJsonString()}";

                throw new InvalidResponseException(message);
            }
        }
    }

    #endregion

    #region Http requests

    /// <summary>
    /// </summary>
    /// <param name="url"></param>
    /// <param name="withSessionKey">是否添加session头</param>
    /// <returns></returns>
    internal static async Task<string> GetAsync(string url, bool withSessionKey = true)
    {
        var result = withSessionKey
            ? await url
                .WithHeader("Authorization", $"session {MiraiBot.Instance.HttpSessionKey}")
                .GetAsync().ConfigureAwait(false)
            : await url.GetAsync().ConfigureAwait(false);

        var re = await result.GetStringAsync().ConfigureAwait(false);
        re.EnsureSuccess($"url={url}");

        return re;
    }

    internal static async Task<string> GetAsync(this HttpEndpoints endpoints, object extra = null,
        bool withSessionKey = true)
    {
        var url = $"http://{MiraiBot.Instance.Address.HttpAddress}/{endpoints.GetDescription()}";

        if (extra != null)
            url = url.SetQueryParams(extra);

        return await GetAsync(url, withSessionKey).ConfigureAwait(false);
    }

    /// <summary>
    /// </summary>
    /// <param name="url"></param>
    /// <param name="json"></param>
    /// <param name="withSessionKey">加入Authentication: session xxx 请求头</param>
    /// <exception cref="InvalidResponseException"></exception>
    /// <returns></returns>
    internal static async Task<string> PostJsonAsync(string url, object json, bool withSessionKey = true)
    {
        var result = withSessionKey
            ? await url
                .WithHeader("Authorization", $"session {MiraiBot.Instance.HttpSessionKey}")
                .PostJsonAsync(json).ConfigureAwait(false)
            : await url.PostJsonAsync(json).ConfigureAwait(false);

        var re = await result.GetStringAsync().ConfigureAwait(false);
        re.EnsureSuccess($"url={url}\r\npayload={json.ToJsonString()}");

        return re;
    }

    /// <summary>
    /// </summary>
    /// <param name="endpoint"></param>
    /// <param name="json"></param>
    /// <param name="withSessionKey">加入Authentication: session xxx 请求头</param>
    /// <exception cref="InvalidResponseException"></exception>
    /// <returns></returns>
    internal static async Task<string> PostJsonAsync(this HttpEndpoints endpoint, object json,
        bool withSessionKey = true)
    {
        var url = $"http://{MiraiBot.Instance.Address.HttpAddress}/{endpoint.GetDescription()}";
        var result = await PostJsonAsync(url, json, withSessionKey).ConfigureAwait(false);

        return result;
    }

    #endregion
}