using System;

namespace Lyrical.Controls.Base
{
    /// <summary>
    /// 가사 시간을 제공할 수 있는 제공자입니다.
    /// </summary>
    public interface ILyricsProvider
    {
        TimeSpan GetPosition();
    }
}
