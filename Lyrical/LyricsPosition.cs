using System;
using Lyrical.Base;

namespace Lyrical
{
    /// <summary>
    /// 가사 매핑의 위치 클래스를 나타냅니다.
    /// </summary>
    public class LyricsPosition : LyricsEvent, ILyricsChild<LyricsMapping>
    {
        /// <summary>
        /// 매핑 위치의 부모 객체를 가져옵니다.
        /// </summary>
        public LyricsMapping Parent
        {
            get
            {
                return _Parent;
            }
        }
        internal LyricsMapping _Parent;

        /// <summary>
        /// 매핑 위치를 설정하거나 가져옵니다.
        /// </summary>
        public int Position
        {
            get
            {
                return _Position;
            }
            set
            {
                _Position = value;
                OnPropertyChanged();
            }
        }
        private int _Position;

        /// <summary>
        /// 매핑 위치의 시작 시간을 가져옵니다.
        /// </summary>
        public TimeSpan BeginTime
        {
            get
            {
                return Parent.BeginTime;
            }
        }

        /// <summary>
        /// 매핑 위치의 종료 시간을 가져옵니다.
        /// </summary>
        public TimeSpan EndTime
        {
            get
            {
                return Parent.EndTime;
            }
        }

        /// <summary>
        /// 매핑 위치의 지속 시간을 가져옵니다.
        /// </summary>
        public TimeSpan Duration
        {
            get
            {
                return Parent.Duration;
            }
        }
    }
}
