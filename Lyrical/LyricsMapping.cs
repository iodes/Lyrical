using System;
using Lyrical.Base;

namespace Lyrical
{
    /// <summary>
    /// 가사 매핑 클래스를 나타냅니다.
    /// </summary>
    public class LyricsMapping : LyricsEvent, ILyricsChild<LyricsSentence>
    {
        #region 속성
        /// <summary>
        /// 문자 매핑의 부모 객체를 가져옵니다.
        /// </summary>
        public LyricsSentence Parent
        {
            get
            {
                return _Parent;
            }
        }
        internal LyricsSentence _Parent;

        /// <summary>
        /// 문자 매핑의 시작 시간을 설정하거나 가져옵니다.
        /// </summary>
        public TimeSpan BeginTime
        {
            get
            {
                return _BeginTime;
            }
            set
            {
                _BeginTime = value;
                OnPropertyChanged();
            }
        }
        private TimeSpan _BeginTime;

        /// <summary>
        /// 문자 매핑의 종료 시간을 설정하거나 가져옵니다.
        /// </summary>
        public TimeSpan EndTime
        {
            get
            {
                return _EndTime;
            }
            set
            {
                _EndTime = value;
                OnPropertyChanged();
            }
        }
        private TimeSpan _EndTime;

        /// <summary>
        /// 문자 매핑의 지속 시간을 가져옵니다.
        /// </summary>
        public TimeSpan Duration
        {
            get
            {
                return EndTime - BeginTime;
            }
        }

        /// <summary>
        /// 가사 문자 매핑의 위치를 설정하거나 가져옵니다.
        /// </summary>
        public LyricsPosition Lyrics
        {
            get
            {
                return _Lyrics;
            }
            set
            {
                _Lyrics = value;
                _Lyrics._Parent = this;
                _Lyrics.PropertyChanged += (s, e) =>
                {
                    OnPropertyChanged();
                };

                OnPropertyChanged();
            }
        }
        private LyricsPosition _Lyrics;

        /// <summary>
        /// 발음 문자 매핑의 위치를 설정하거나 가져옵니다.
        /// </summary>
        public LyricsPosition Pronounce
        {
            get
            {
                return _Pronounce;
            }
            set
            {
                _Pronounce = value;
                _Pronounce._Parent = this;
                _Pronounce.PropertyChanged += (s, e) =>
                {
                    OnPropertyChanged();
                };

                OnPropertyChanged();
            }
        }
        private LyricsPosition _Pronounce;

        /// <summary>
        /// 해석 문자 매핑의 위치를 설정하거나 가져옵니다.
        /// </summary>
        public LyricsPosition Translation
        {
            get
            {
                return _Translation;
            }
            set
            {
                _Translation = value;
                _Translation._Parent = this;
                _Translation.PropertyChanged += (s, e) =>
                {
                    OnPropertyChanged();
                };

                OnPropertyChanged();
            }
        }
        private LyricsPosition _Translation;
        #endregion

        #region 사용자 함수
        /// <summary>
        /// 현재 매핑의 전체 문자열을 반환합니다.
        /// </summary>
        /// <returns>현재 매핑의 문자열 변환 결과입니다.</returns>
        public override string ToString()
        {
            string lyrics = Lyrics != null ? $"-L{Lyrics.Position}" : null;
            string pronounce = Pronounce != null ? $"-P{Pronounce.Position}" : null;
            string translation = Translation != null ? $"-T{Translation.Position}" : null;

            return $"[{LyricsUtility.ToString(BeginTime)}~{LyricsUtility.ToString(EndTime)}{lyrics}{pronounce}{translation}]";
        }
        #endregion
    }
}
