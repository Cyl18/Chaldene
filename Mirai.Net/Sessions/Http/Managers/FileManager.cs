﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using Manganese.Text;
using Mirai.Net.Data.Sessions;
using Mirai.Net.Data.Shared;
using Mirai.Net.Utils.Internal;
using File = Mirai.Net.Data.Shared.File;

namespace Mirai.Net.Sessions.Http.Managers;

/// <summary>
/// 文件管理器
/// </summary>
public static class FileManager
{
    /// <summary>
    ///     获取群文件列表
    /// </summary>
    /// <param name="target"></param>
    /// <param name="withDownloadInfo">附带下载信息，默认不附带</param>
    /// <param name="folderId">文件夹id，空字符串即为根目录</param>
    /// <returns></returns>
    public static async Task<IEnumerable<File>> GetFilesAsync(string target, bool? withDownloadInfo = null,
        string folderId = "")
    {
        var result = await HttpEndpoints.FileList.GetAsync(new
        {
            target,
            withDownloadInfo,
            id = folderId
        }).ConfigureAwait(false);

        var arr = result.Fetch("data").ToJArray();

        return arr.Select(x => x.ToObject<File>());
    }

    /// <summary>
    ///     获取群文件信息
    /// </summary>
    /// <param name="target">群号</param>
    /// <param name="fileId">文件id</param>
    /// <param name="withDownloadInfo"></param>
    /// <returns></returns>
    public static async Task<File> GetFileAsync(string target, string fileId, bool? withDownloadInfo = null)
    {
        var result = await HttpEndpoints.FileInfo.GetAsync(new
        {
            target,
            id = fileId,
            withDownloadInfo
        }).ConfigureAwait(false);

        return result.FetchJToken("data").ToObject<File>();
    }

    /// <summary>
    ///     创建群文件夹
    /// </summary>
    /// <param name="target"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static async Task<File> CreateFolderAsync(string target, string name)
    {
        var result = await HttpEndpoints.FileCreate.PostJsonAsync(new
        {
            id = "",
            target,
            directoryName = name
        }).ConfigureAwait(false);

        return result.FetchJToken("data").ToObject<File>();
    }

    /// <summary>
    ///     删除群文件
    /// </summary>
    /// <param name="target"></param>
    /// <param name="fileId"></param>
    public static async Task DeleteFileAsync(string target, string fileId)
    {
        _ = await HttpEndpoints.FileDelete.PostJsonAsync(new
        {
            target,
            id = fileId
        }).ConfigureAwait(false);
    }

    /// <summary>
    ///     移动群文件
    /// </summary>
    /// <param name="target"></param>
    /// <param name="fileId">移动文件id</param>
    /// <param name="destination">移动目标文件夹id</param>
    public static async Task MoveFileAsync(string target, string fileId, string destination)
    {
        _ = await HttpEndpoints.FileMove.PostJsonAsync(new
        {
            target,
            id = fileId,
            movoTo = destination
        }).ConfigureAwait(false);
    }

    /// <summary>
    ///     重命名群文件
    /// </summary>
    /// <param name="target"></param>
    /// <param name="fileId">重命名文件id</param>
    /// <param name="newName">新文件名</param>
    public static async Task RenameFileAsync(string target, string fileId, string newName)
    {
        _ = await HttpEndpoints.FileRename.PostJsonAsync(new
        {
            target,
            id = fileId,
            renameTo = newName
        }).ConfigureAwait(false);
    }

    /// <summary>
    ///     上传群文件，参考https://github.com/project-mirai/mirai-api-http/issues/456
    /// </summary>
    /// <param name="target">上传到哪个群</param>
    /// <param name="filePath">文件的路径</param>
    /// <param name="uploadPath">上传路径，例如/xx（不可以指定文件名，默认为上传到根目录）</param>
    /// <returns>有几率返回null，这是个mirai-api-http的玄学问题</returns>
    public static async Task<File> UploadFileAsync(string target, string filePath, string uploadPath = "/")
    {
        uploadPath ??= $"/{Path.GetFileName(filePath)}";

        var url = $"http://{MiraiBot.Instance.Address.HttpAddress}/{HttpEndpoints.FileUpload.GetDescription()}";

        var result = await url
            .WithHeader("Authorization", $"session {MiraiBot.Instance.HttpSessionKey}")
            .PostMultipartAsync(x => x
                .AddString("type", "group")
                .AddString("path", uploadPath)
                .AddString("target", target)
                .AddFile("file", filePath)).ConfigureAwait(false);

        var response = await result.GetStringAsync().ConfigureAwait(false);
        response.EnsureSuccess("这大抵是个玄学问题罢。");
        
        var re = response.ToJObject();

        return !re.ContainsKey("name") ? null : re.ToObject<File>();
    }

    /// <summary>
    ///     上传图片
    /// </summary>
    /// <param name="imagePath">图片路径</param>
    /// <param name="imageUploadTargets">上传类型</param>
    /// <returns>item1: 图片id，item2：图片url</returns>
    public static async Task<(string, string)> UploadImageAsync(string imagePath,
        ImageUploadTargets imageUploadTargets = ImageUploadTargets.Group)
    {
        var url = $"http://{MiraiBot.Instance.Address.HttpAddress}/{HttpEndpoints.UploadImage.GetDescription()}";

        var result = await url
            .WithHeader("Authorization", $"session {MiraiBot.Instance.HttpSessionKey}")
            .PostMultipartAsync(x => x
                .AddString("type", imageUploadTargets.GetDescription())
                .AddFile("img", imagePath)).ConfigureAwait(false);

        var re = await result.GetStringAsync().ConfigureAwait(false);
        re.EnsureSuccess("这大抵是个玄学问题罢。");

        return (re.Fetch("imageId"), re.Fetch("url"));
    }

    /// <summary>
    ///     上传语音
    /// </summary>
    /// <param name="voicePath">语言路径</param>
    /// <returns>item1: 语音id，item2：语音url</returns>
    public static async Task<(string, string)> UploadVoiceAsync(string voicePath)
    {
        var url = $"http://{MiraiBot.Instance.Address.HttpAddress}/{HttpEndpoints.UploadVoice.GetDescription()}";

        var result = await url
            .WithHeader("Authorization", $"session {MiraiBot.Instance.HttpSessionKey}")
            .PostMultipartAsync(x => x
                .AddString("type", "group")
                .AddFile("voice", voicePath)).ConfigureAwait(false);

        var re = await result.GetStringAsync().ConfigureAwait(false);
        re.EnsureSuccess("这大抵是个玄学问题罢。");

        return (re.Fetch("voiceId"), re.Fetch("url"));
    }
}