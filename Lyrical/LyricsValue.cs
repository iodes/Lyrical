using System.Linq;
using System.Collections.Generic;
using Lyrical.Base;

namespace Lyrical
{
    /// <summary>
    /// 가사 문자를 제공하는 값 클래스를 나타냅니다.
    /// </summary>
    public sealed class LyricsValue : ILyricsChild<LyricsSentence>
    {
        #region 속성
        /// <summary>
        /// 가사 문자의 부모 객체를 가져옵니다.
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
        /// 가사 문장의 문자 목록을 가져옵니다.
        /// </summary>
        public LyricsCharacter[] Lyrics
        {
            get
            {
                if (_Lyrics == null)
                {
                    _Lyrics = GetLyricsValue(ValueType.Lyrics);
                }

                return _Lyrics;
            }
        }
        private LyricsCharacter[] _Lyrics;

        /// <summary>
        /// 발음 문장의 문자 목록을 가져옵니다.
        /// </summary>
        public LyricsCharacter[] Pronounce
        {
            get
            {
                if (_Pronounce == null)
                {
                    _Pronounce = GetLyricsValue(ValueType.Pronounce);
                }

                return _Pronounce;
            }
        }
        private LyricsCharacter[] _Pronounce;

        /// <summary>
        /// 해석 문장의 문자 목록을 가져옵니다.
        /// </summary>
        public LyricsCharacter[] Translation
        {
            get
            {
                if (_Translation == null)
                {
                    _Translation = GetLyricsValue(ValueType.Translation);
                }

                return _Translation;
            }
        }
        private LyricsCharacter[] _Translation;
        #endregion

        #region 열거형
        private enum ValueType
        {
            Lyrics,
            Pronounce,
            Translation
        }
        #endregion

        #region 생성자
        internal LyricsValue()
        {

        }
        #endregion

        #region 내부 함수
        private LyricsCharacter[] GetLyricsValue(ValueType type)
        {
            if (Parent.Mappings.Count > 0)
            {
                int idx = 0;
                int prevPosition = -1;
                var tempSortedList = new SortedList<int, LyricsCharacter>();

                foreach (LyricsMapping mapping in Parent.Mappings)
                {
                    // 종류에 따른 정보 읽기
                    int position = 0;
                    string sourceText = null;

                    switch (type)
                    {
                        case ValueType.Lyrics:
                            if (mapping.Lyrics == null)
                            {
                                return null;
                            }

                            position = mapping.Lyrics.Position;
                            sourceText = Parent.Lyrics;
                            break;

                        case ValueType.Pronounce:
                            if (mapping.Pronounce == null)
                            {
                                return null;
                            }

                            position = mapping.Pronounce.Position;
                            sourceText = Parent.Pronounce;
                            break;

                        case ValueType.Translation:
                            if (mapping.Translation == null)
                            {
                                return null;
                            }

                            position = mapping.Translation.Position;
                            sourceText = Parent.Translation;
                            break;
                    }

                    // 중복 매핑 데이터 결합
                    if (!tempSortedList.ContainsKey(position))
                    {
                        int startIndex = idx == 0 ? 0 : prevPosition + 1;
                        int charLength = position - prevPosition;

                        tempSortedList.Add(position, new LyricsCharacter
                        {
                            _Parent = this,
                            _Index = idx,
                            _BeginTime = mapping.BeginTime,
                            _EndTime = mapping.EndTime,
                            _Position = position,
                            _Value = sourceText.Substring(startIndex, charLength)
                        });

                        prevPosition = position;
                        idx++;
                    }
                    else
                    {
                        var orgCharacter = tempSortedList[position];
                        tempSortedList[position] = new LyricsCharacter
                        {
                            _Parent = orgCharacter.Parent,
                            _Index = orgCharacter.Index,
                            _BeginTime = orgCharacter.BeginTime,
                            _EndTime = mapping.EndTime,
                            _Position = orgCharacter.Position,
                            _Value = orgCharacter.Value
                        };
                    }
                }

                return tempSortedList.Values.ToArray();
            }

            return null;
        }
        #endregion
    }
}
