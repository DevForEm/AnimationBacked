using AnimationBaked.Properties;
using System.Data.SQLite;
using Mono.Unix;

namespace AnimationBaked.Utils.DataBaseManage;

public class Dbms
{
    private static SQLiteConnection? _connection = null;

    public Dbms()
    {
    }

    public static void Connect()
    {
        var path = Program.FilePath;
        var dbPath = Path.Combine(path, "data.db");
        var connectionString = $"Data Source={dbPath};Version=3;";

        Logger.Logger.LogWarning(connectionString);
        _connection = new SQLiteConnection(connectionString);
        _connection.Open();
        Logger.Logger.LogInfo("DataBase connected.");
        //return Task.Run(() => Thread.CurrentThread);
    }

    private static bool PermissionDirCheck(string path)
    {
        var platform = Environment.OSVersion.Platform;
        switch (platform)
        {
            case PlatformID.Win32NT:
                try
                {
                    var dir = new DirectoryInfo(path);
#pragma warning disable CA1416
                    var permission = dir.GetAccessControl();
#pragma warning restore CA1416
                    return true;
                }
                catch (UnauthorizedAccessException)
                {
                    return false;
                }

            //break;
            case PlatformID.Unix:
                try
                {
                    var unixFileInfo = new UnixFileInfo(path);
                    var access = unixFileInfo.FileAccessPermissions;
                    Logger.Logger.LogInfo(access.ToString());
                    return (access & FileAccessPermissions.UserWrite) != 0 ||
                           (access & FileAccessPermissions.GroupWrite) != 0 ||
                           (access & FileAccessPermissions.OtherWrite) != 0;
                }
                catch (UnauthorizedAccessException)
                {
                    return false;
                }

            //break;
            case PlatformID.Win32S:
            case PlatformID.Win32Windows:
            case PlatformID.WinCE:
            case PlatformID.Xbox:
            case PlatformID.MacOSX:
            case PlatformID.Other:
            default:
                return false;
        }
    }

    public void CreateTable()
    {
        string sql = @"
            CREATE TABLE IF NOT EXISTS VideoInfo (
                Id INTEGER PRIMARY KEY ,
                FileName TEXT NOT NULL,
                FileSize TEXT NOT NULL,
                FileHash TEXT NOT NULL,
                VideoDuration TEXT NOT NULL,
                MatchMode TEXT NOT NULL,
                EpisodeId INTEGER,
                animeId INTEGER,
                AnimeTitle TEXT,
                EpisodeTitle TEXT,
                AnimationType TEXT,
                TypeDescription TEXT,
                Shift integer
                                                 
            )";
        using var command = new SQLiteCommand(sql, _connection);
        command.ExecuteNonQuery();
    }

    public void InsertData(LocalAnimateInfo videoInfo)
    {
        var sql = @"
            INSERT INTO VideoInfo (FileName, FileSize, FileHash, VideoDuration, MatchMode)
            VALUES (@FileName, @FileSize, @FileHash, @VideoDuration, @MatchMode)";
        using var command = new SQLiteCommand(sql, _connection);
        command.Parameters.AddWithValue("@FileName", videoInfo.fileName);
        command.Parameters.AddWithValue("@FileSize", videoInfo.fileSize);
        command.Parameters.AddWithValue("@FileHash", videoInfo.fileHash);
        command.Parameters.AddWithValue("@VideoDuration", videoInfo.videoDuration);
        command.Parameters.AddWithValue("@MatchMode", videoInfo.matchMode);
        command.ExecuteNonQuery();
    }

    public List<LocalAnimateInfo> QueryData()
    {
        string sql = "SELECT * FROM VideoInfo";
        using var command = new SQLiteCommand(sql, _connection);
        using SQLiteDataReader reader = command.ExecuteReader();
        var videoInfos = new List<LocalAnimateInfo>();
        while (reader.Read())
        {
            var videoInfo = new LocalAnimateInfo
            {
                fileName = reader["FileName"].ToString(),
                fileSize = reader["FileSize"].ToString(),
                fileHash = reader["FileHash"].ToString(),
                videoDuration = reader["VideoDuration"].ToString(),
                matchMode = reader["MatchMode"].ToString()
            };
            videoInfos.Add(videoInfo);
        }

        return videoInfos;
    }
}