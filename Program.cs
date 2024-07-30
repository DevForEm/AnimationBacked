// See https://aka.ms/new-console-template for more information

using AnimationBaked.Utils.Logger;
using AnimationBaked.Utils.DataBaseManage;

namespace AnimationBaked;

using Utils.DataBaseManage;
using Utils;

public abstract class Program
{
    private static readonly string Home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
#if DEBUG
    public static string FilePath = $"{Directory.GetCurrentDirectory()}/TestResources";
    public static readonly string ConfigPath = $"{Directory.GetCurrentDirectory()}/TestResources/config";
#endif
#if RELEASE
    public static string FilePath = $"{Directory.GetCurrentDirectory()}";
    public static readonly string ConfigPath = Path.Combine(Home, ".config/AnimationBaked");
#endif


    public static void Main(string[] args)
    {
        
        Logger.LogInfo("AnimationBaked,Start!");
        if (args.Length > 0)
        {
            foreach (var arg in args)
            {
                if (arg is "-d" or "--directory")
                {
                    FilePath = args[1];
                }
            }
        }

        var dbms = new Dbms();
        Dbms.Connect();
        dbms.CreateTable();
        VideoTask.ScanVideoFiles(FilePath);
    }
}