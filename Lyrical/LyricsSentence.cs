using System;
using System.Text;
using System.Collections.Specialized;
using Lyrical.Base;
using Lyrical.Collections;

namespace Lyrical
{
    /// <summary>
    /// 가사 문장 클래스를 나타냅니다.
    /// </summary>
    public class LyricsSentence : ILyricsChild<LyricsDocument>
    {
        #region 객체
        LyricsCharacter[] lastLyrics;
        #endregion

        #region 속성
        /// <summary>
        /// 가사 문장의 부모 객체를 가져옵니다.
        /// </summary>
        public LyricsDocument Parent
        {
            get
            {
                return _Parent;
            }
        }
        internal LyricsDocument _Parent;

        /// <summary>
        /// 가사 문장의 시작 시간을 설정하거나 가져옵니다.
        /// </summary>
        public TimeSpan BeginTime { get; set; }

        /// <summary>
        /// 가사 문장의 내용을 설정하거나 가져옵니다.
        /// </summary>
        public string Lyrics { get; set; }

        /// <summary>
        /// 가사 문장의 발음을 설정하거나 가져옵니다.
        /// </summary>
        public string Pronounce { get; set; }

        /// <summary>
        /// 가사 문장의 해석을 설정하거나 가져옵니다.
        /// </summary>
        public string Translation { get; set; }

        /// <summary>
        /// 가사 문장의 매핑 목록을 설정하거나 가져옵니다.
        /// </summary>
        public MappingCollection Mappings { get; set; } = new MappingCollection();

        /// <summary>
        /// 가사 문장의 문자 값을 가져옵니다.
        /// </summary>
        public LyricsValue Characters
        {
            get
            {
                if (_Characters.Parent == null)
                {
                    _Characters._Parent = this;
                }

                return _Characters;
            }
        }
        private LyricsValue _Characters = new LyricsValue();
        #endregion

        #region 생성자
        public LyricsSentence()
        {
            Mappings.CollectionChanged += Mappings_CollectionChanged;
        }
        #endregion

        #region 이벤트
        private void Mappings_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (LyricsMapping mapping in e.NewItems)
                {
                    mapping._Parent = this;
                    mapping.PropertyChanged += Mapping_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (LyricsMapping mapping in e.OldItems)
                {
                    mapping._Parent = null;
                    mapping.PropertyChanged -= Mapping_PropertyChanged;
                }
            }

            ClearCache();
        }

        private void Mapping_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ClearCache();
        }
        #endregion

        #region 내부 함수
        private void ClearCache()
        {
            _Characters = new LyricsValue();
        }
        #endregion

        #region 사용자 함수
        /// <summary>
        /// 현재 가사의 전체 문자열을 반환합니다.
        /// </summary>
        /// <returns>현재 가사의 문자열 변환 결과입니다.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"[{LyricsUtility.ToString(BeginTime)}]");

            foreach (LyricsMapping mapping in Mappings)
            {
                builder.AppendLine(mapping.ToString());
            }

            builder.AppendLine($"[LR:{Lyrics}]");
            if (Pronounce != null)
            {
                builder.AppendLine($"[PR:{Pronounce}]");
            }
            if (Translation != null)
            {
                builder.AppendLine($"[TR:{Translation}]");
            }

            return builder.ToString();
        }
        #endregion
    }
}
