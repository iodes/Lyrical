using System;
using System.Globalization;

namespace Lyrical
{
    /// <summary>
    /// 가사 도구 클래스를 나타냅니다.
    /// </summary>
    public static class LyricsUtility
    {
        public static TimeSpan ToTime(string value)
        {
            return TimeSpan.ParseExact(value, @"mm\:ss\.ff", CultureInfo.InvariantCulture);
        }

        public static string ToString(TimeSpan value)
        {
            return value.ToString(@"mm\:ss\.ff");
        }
    }
}
