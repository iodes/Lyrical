using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using Lyrical.Collections;

namespace Lyrical
{
    /// <summary>
    /// 가사 문서 클래스를 나타냅니다.
    /// </summary>
    public class LyricsDocument
    {
        #region 속성
        /// <summary>
        /// 가사의 제목을 설정하거나 가져옵니다.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 가사의 연주자를 설정하거나 가져옵니다.
        /// </summary>
        public string Artist { get; set; }

        /// <summary>
        /// 가사의 앨범명을 설정하거나 가져옵니다.
        /// </summary>
        public string Album { get; set; }

        /// <summary>
        /// 가사의 문장 목록을 설정하거나 가져옵니다.
        /// </summary>
        public SentenceCollection Sentences { get; set; } = new SentenceCollection();
        #endregion

        #region 열거형
        public enum ValueType
        {
            Lyrics,
            Pronounce,
            Translation
        }
        #endregion

        #region 생성자
        public LyricsDocument()
        {
            Sentences.CollectionChanged += Sentences_CollectionChanged;
        }

        public LyricsDocument(string path) : base()
        {
            Load(File.ReadAllText(path));
        }
        #endregion

        #region 이벤트
        private void Sentences_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (LyricsSentence sentence in e.NewItems)
            {
                sentence._Parent = this;
            }
        }
        #endregion

        #region 사용자 함수
        /// <summary>
        /// 문자 형식으로 포맷된 가사를 현재 객체의 내용으로 불러옵니다.
        /// </summary>
        /// <param name="value">문자 형식으로 포맷된 가사 내용입니다.</param>
        public void Load(string value)
        {
            foreach (string str in value.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                // 헤더 분석
                if (Title == null || Album == null || Artist == null)
                {
                    var regexHeader = Regex.Matches(str, @"\[(TI|AL|AR):(.+?)\]", RegexOptions.IgnoreCase);
                    foreach (Match matchHeader in regexHeader)
                    {
                        if (matchHeader.Success)
                        {
                            switch (matchHeader.Groups[1].Value.ToUpper())
                            {
                                case "TI":
                                    Title = matchHeader.Groups[2].Value;
                                    break;

                                case "AL":
                                    Album = matchHeader.Groups[2].Value;
                                    break;

                                case "AR":
                                    Artist = matchHeader.Groups[2].Value;
                                    break;
                            }
                        }
                    }
                }

                // 본문 분석
                var regexSen = Regex.Match(str, @"\[([\d]{2,}:[\d]{2}.[\d]{2})\]");
                var regexMap = Regex.Matches(str, @"\[([\d]{2,}:[\d]{2}.[\d]{2})~([0-9]{2,}:[\d]{2}.[\d]{2})((?:-[LPT]\d+){1,3})\]", RegexOptions.IgnoreCase);
                var regexLyrics = Regex.Matches(str, @"\[(LR|PR|TR):(.+?)\]", RegexOptions.IgnoreCase);

                // 문장 시작 시간
                if (regexSen.Success)
                {
                    Sentences.Add(new LyricsSentence
                    {
                        BeginTime = LyricsUtility.ToTime(regexSen.Groups[1].Value)
                    });
                }

                // 문장의 문자 매핑
                foreach (Match matchMap in regexMap)
                {
                    if (matchMap.Success)
                    {
                        var mapping = new LyricsMapping
                        {
                            BeginTime = LyricsUtility.ToTime(matchMap.Groups[1].Value),
                            EndTime = LyricsUtility.ToTime(matchMap.Groups[2].Value),
                        };

                        var regexMapCase = Regex.Matches(matchMap.Groups[3].Value, @"([LPT])(\d+)", RegexOptions.IgnoreCase);

                        foreach (Match matchMapCase in regexMapCase)
                        {
                            if (matchMapCase.Success)
                            {
                                var position = new LyricsPosition
                                {
                                    Position = int.Parse(matchMapCase.Groups[2].Value)
                                };

                                switch (matchMapCase.Groups[1].Value)
                                {
                                    case "L":
                                        mapping.Lyrics = position;
                                        break;

                                    case "P":
                                        mapping.Pronounce = position;
                                        break;

                                    case "T":
                                        mapping.Translation = position;
                                        break;
                                }
                            }
                        }

                        Sentences.Last().Mappings.Add(mapping);
                    }
                }

                // 문장의 내용 설정
                foreach (Match matchLyrics in regexLyrics)
                {
                    if (matchLyrics.Success)
                    {
                        switch (matchLyrics.Groups[1].Value.ToUpper())
                        {
                            case "LR":
                                Sentences.Last().Lyrics = matchLyrics.Groups[2].Value;
                                break;

                            case "PR":
                                Sentences.Last().Pronounce = matchLyrics.Groups[2].Value;
                                break;

                            case "TR":
                                Sentences.Last().Translation = matchLyrics.Groups[2].Value;
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 시간과 가장 가까운 문장 객체를 반환합니다.
        /// </summary>
        /// <param name="value">현재 재생 시간입니다.</param>
        /// <returns>시간과 가장 가까운 문장 객체입니다.</returns>
        public LyricsSentence GetSentence(TimeSpan value)
        {
            int index = Sentences.Select(x => x.BeginTime).ToList().BinarySearch(value);
            if (index < 0)
            {
                return Sentences[Math.Max(0, ~index - 1)];
            }
            else
            {
                return Sentences[index];
            }
        }

        /// <summary>
        /// 시간과 가장 가까운 문자 객체를 반환합니다.
        /// </summary>
        /// <param name="value">현재 재생 시간입니다.</param>
        /// <returns>시간과 가장 가까운 문자 객체입니다.</returns>
        public LyricsCharacter GetCharacter(TimeSpan value, ValueType type)
        {
            var currentSentence = GetSentence(value);

            LyricsCharacter[] target = null;
            switch (type)
            {
                case ValueType.Lyrics:
                    target = currentSentence.Characters.Lyrics;
                    break;

                case ValueType.Pronounce:
                    target = currentSentence.Characters.Pronounce;
                    break;

                case ValueType.Translation:
                    target = currentSentence.Characters.Translation;
                    break;
            }

            if (target != null)
            {
                int index = target.Select(x => x.BeginTime).ToList().BinarySearch(value);
                if (index < 0)
                {
                    return target[Math.Max(0, ~index - 1)];
                }
                else
                {
                    return target[index];
                }
            }

            return null;
        }

        /// <summary>
        /// 현재 가사의 전체 문자열을 반환합니다.
        /// </summary>
        /// <returns>현재 가사의 문자열 변환 결과입니다.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine($"[TI:{Title}]");
            builder.AppendLine($"[AL:{Album}]");
            builder.AppendLine($"[AR:{Artist}]\n");

            foreach (LyricsSentence sentence in Sentences)
            {
                builder.AppendLine(sentence.ToString());
            }

            return builder.ToString().Trim();
        }
        #endregion
    }
}
