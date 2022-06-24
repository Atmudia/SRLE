using SRML.Console;

namespace SRLE
{
    internal static class Extensions
    {
        public static void Log(this string str)
        {
            Console.Log(str);
        }
        public static void LogWarning(this string str)
        {
            Console.LogWarning(str);
        }
        public static void LogError(this string str)
        {
            Console.LogError(str);
        }
    }
}