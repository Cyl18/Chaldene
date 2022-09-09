using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Manganese.Text;
using Chaldene.Data.Exceptions;
using Chaldene.Data.Sessions;
using Chaldene.Utils.Internal;

namespace Chaldene.Sessions;

public partial class MiraiBot
{
    #region Guarantee

    /// <summary>
    ///     根据json判断这个json是否是正确的，否则抛出异常
    /// </summary>
    /// <param name="json"></param>
    /// <param name="appendix"></param>
    internal void EnsureSuccess(string json, string appendix = null)
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
                // else
                //    message += $"\r\n备注: {MiraiBot.Instance.ToJsonString()}";
                // ?????
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
    internal async Task<string> GetAsync(string url, bool withSessionKey = true)
    {
        var result = withSessionKey
            ? await url
                .WithHeader("Authorization", $"session {HttpSessionKey}")
                .GetAsync().ConfigureAwait(false)
            : await url.GetAsync().ConfigureAwait(false);

        var re = await result.GetStringAsync().ConfigureAwait(false);
        EnsureSuccess(re, $"url={url}");

        return re;
    }

    internal async Task<string> GetAsync(HttpEndpoints endpoints, object extra = null,
        bool withSessionKey = true)
    {
        var url = $"{HttpScheme}://{Address.HttpAddress}/{endpoints.GetDescription()}";

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
    internal async Task<string> PostJsonAsync(string url, object json, bool withSessionKey = true)
    {
        var result = withSessionKey
            ? await url
                .WithHeader("Authorization", $"session {HttpSessionKey}")
                .PostJsonAsync(json).ConfigureAwait(false)
            : await url.PostJsonAsync(json).ConfigureAwait(false);

        var re = await result.GetStringAsync().ConfigureAwait(false);
        EnsureSuccess(re, $"url={url}\r\npayload={json.ToJsonString()}");

        return re;
    }

    /// <summary>
    /// </summary>
    /// <param name="endpoint"></param>
    /// <param name="json"></param>
    /// <param name="withSessionKey">加入Authentication: session xxx 请求头</param>
    /// <exception cref="InvalidResponseException"></exception>
    /// <returns></returns>
    internal async Task<string> PostJsonAsync(HttpEndpoints endpoint, object json,
        bool withSessionKey = true)
    {
        var url = $"{HttpScheme}://{Address.HttpAddress}/{endpoint.GetDescription()}";
        var result = await PostJsonAsync(url, json, withSessionKey).ConfigureAwait(false);

        return result;
    }

    #endregion
}