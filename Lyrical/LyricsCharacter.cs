using System;
using Lyrical.Base;

namespace Lyrical
{
    /// <summary>
    /// 가사 문자 클래스를 나타냅니다.
    /// </summary>
    public sealed class LyricsCharacter : ILyricsChild<LyricsValue>
    {
        #region 속성
        /// <summary>
        /// 문자 값의 부모 문자를 가져옵니다.
        /// </summary>
        public LyricsValue Parent
        {
            get
            {
                return _Parent;
            }
        }
        internal LyricsValue _Parent;

        /// <summary>
        /// 문자 값의 인덱스 번호를 가져옵니다.
        /// </summary>
        public int Index
        {
            get
            {
                return _Index;
            }
        }
        internal int _Index;

        /// <summary>
        /// 문자 값의 위치를 가져옵니다.
        /// </summary>
        public int Position
        {
            get
            {
                return _Position;
            }
        }
        internal int _Position;

        /// <summary>
        /// 문자 값의 위치에 해당하는 문자를 가져옵니다.
        /// </summary>
        public string Value
        {
            get
            {
                return _Value;
            }
        }
        internal string _Value;

        /// <summary>
        /// 문자 값의 시작 시간을 가져옵니다.
        /// </summary>
        public TimeSpan BeginTime
        {
            get
            {
                return _BeginTime;
            }
        }
        internal TimeSpan _BeginTime;

        /// <summary>
        /// 문자 값의 종료 시간을 가져옵니다.
        /// </summary>
        public TimeSpan EndTime
        {
            get
            {
                return _EndTime;
            }
        }
        internal TimeSpan _EndTime;

        /// <summary>
        /// 문자 값의 지속 시간을 가져옵니다.
        /// </summary>
        public TimeSpan Duration
        {
            get
            {
                return EndTime - BeginTime;
            }
        }
        #endregion

        #region 생성자
        internal LyricsCharacter()
        {

        }
        #endregion
    }
}
