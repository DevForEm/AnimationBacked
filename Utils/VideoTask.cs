using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;
using AnimationBaked.Properties;
using AnimationBaked.Utils.DataBaseManage;

//using static AnimationBaked.Utils.Logger.Logger;

namespace AnimationBaked.Utils;

/// <summary>
/// 处理视频相关任务，扫描视频文件，生成视频文件信息（大小，长度，hash等），Todo 生成视频文件缩略图等
/// </summary>
public abstract partial class VideoTask
{
    public static void ScanVideoFiles(string videoPath)
    {
        Logger.Logger.LogInfo("Start scanning video files.");
        if (!Directory.Exists(videoPath))
        {
            Logger.Logger.LogError("The directory does not exist.");
            return;
        }

        Dbms dbms = new();
        Logger.Logger.LogInfo("DataBase connected.");
        var directories = Directory.GetDirectories(videoPath);
        var outputPath = $"{videoPath}/config.json";
        Logger.Logger.LogPath(outputPath);
        foreach (var directory in directories)
        {
            var files = Directory.GetFiles(directory);
            foreach (var file in files)
            {
                if (file.EndsWith(".mp4") || file.EndsWith(".mkv") || file.EndsWith(".avi"))
                {
                    var fileInfo = new FileInfo(file);
                    var videoInfo = new LocalAnimateInfo
                    {
                        fileName = fileInfo.Name,
                        fileSize = fileInfo.Length.ToString(),
                        fileHash = GetHash(file),
                        videoDuration = GetVideoDuration(file),
                        matchMode = "hashAndFileName"
                    };
                    var queryData = dbms.QueryData();
                    if (IsVideoFileInDatabase(videoInfo, queryData))
                    {
                        dbms.InsertData(videoInfo);
                    }

                    var json = JsonSerializer.Serialize(videoInfo);

                    //Console.WriteLine(json);

                    File.AppendAllText(outputPath, json);
                }
            }
        }
    }

    private static bool IsVideoFileInDatabase(LocalAnimateInfo origin, List<LocalAnimateInfo> database)
    {
        var result = database.Find(info =>
            info.fileHash == origin.fileHash && info.fileName == origin.fileName);
        return result == null;
    }

    /// <summary>
    /// 获取文件前16MD的hash值
    /// </summary>
    /// <param name="file">文件 </param>
    /// <returns>hash值</returns>
    private static string GetHash(string file)
    {
        const int bufferSize = 16 * 1024 * 1024; //16MB
        using var stream = File.OpenRead(file);
        var buffer = new byte[bufferSize];
        var bytesRead = stream.Read(buffer, 0, bufferSize);
        var hash = MD5.HashData(buffer.AsSpan(0, bytesRead));
        var hashString = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        return hashString;
    }

    private static string GetVideoDuration(string video)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $"-i \"{video}\"",
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        var process = Process.Start(startInfo);
        if (process == null)
        {
            return "文件不存在";
        }

        var reader = process.StandardError;
        var output = reader.ReadToEnd();
        process.WaitForExit();
        var durationMatch = DurationRegex().Match(output);
        if (!durationMatch.Success) return "0";
        var hours = int.Parse(durationMatch.Groups[1].Value);
        var minutes = int.Parse(durationMatch.Groups[2].Value);
        var seconds = int.Parse(durationMatch.Groups[3].Value);
        var totalSeconds = hours * 3600 + minutes * 60 + seconds;
        return totalSeconds.ToString();
    }

    [GeneratedRegex(@"Duration: (\d{2}):(\d{2}):(\d{2})")]
    private static partial Regex DurationRegex();


    /// <summary>
    /// todo 从弹幕库获取弹幕并转换为json
    /// </summary>
    /// <param name="videoInfo"></param>
    /// <param name="outputPath"></param>
    public static void JsonConverterFromDanmuku(string videoInfo, string outputPath)
    {
    }
}