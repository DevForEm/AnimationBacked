namespace AnimationBaked.Utils;

public class OtherUtils
{/// <summary>
 /// 获取系统变量
 /// </summary>
 /// <param name="variableName">变量名称</param>
 /// <returns>变量完整路径</returns>
    public static string PrintEnvironmentVariable(string variableName)
    {
        var value = Environment.GetEnvironmentVariable(variableName);
        return value ?? "The environment variable does not exist.";
    }
}