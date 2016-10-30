using System;
using System.Linq;
using Lyrical.Collections;

namespace Lyrical.Verifications
{
    /// <summary>
    /// 가사 문서의 검증을 제공하는 클래스를 나타냅니다.
    /// </summary>
    public class LyricsVerification
    {
        #region 속성
        /// <summary>
        /// 가사 문서를 가져옵니다.
        /// </summary>
        public LyricsDocument Document
        {
            get
            {
                return _Document;
            }
        }
        private LyricsDocument _Document;

        /// <summary>
        /// 가사 문서의 검증 결과 목록을 가져옵니다.
        /// </summary>
        public VerificationResultCollection Results
        {
            get
            {
                return _Results;
            }
        }
        private VerificationResultCollection _Results = new VerificationResultCollection();

        /// <summary>
        /// 가사 문서가 검증되었는지 여부를 가져옵니다.
        /// </summary>
        public bool Success
        {
            get
            {
                return Results.Count == 0;
            }
        }
        #endregion

        #region 생성자
        public LyricsVerification(LyricsDocument document)
        {
            _Document = document;
            Verification(Document);
        }
        #endregion

        #region 내부 함수
        private void Verification(LyricsDocument document)
        {
            // 제목 검사
            if (document.Title == null)
            {
                Write("가사의 제목 정보가 없습니다.");
            }

            // 앨범 검사
            if (document.Album == null)
            {
                Write("가사의 앨범 정보가 없습니다.");
            }

            // 연주자 검사
            if (document.Artist == null)
            {
                Write("가사의 연주자 정보가 없습니다.");
            }

            // 매핑 여부 검사
            bool existLR = false;
            bool existPR = false;
            bool existTR = false;
            foreach (LyricsSentence sentence in document.Sentences)
            {
                foreach (LyricsMapping mapping in sentence.Mappings)
                {
                    if (existLR == false && mapping.Lyrics != null)
                    {
                        existLR = true;
                    }
                    if (existPR == false && mapping.Pronounce != null)
                    {
                        existPR = true;
                    }
                    if (existTR == false && mapping.Translation != null)
                    {
                        existTR = true;
                    }
                }
            }

            // 매핑 데이터 검사
            for (int i = 0; i < document.Sentences.Count; i++)
            {
                var sentence = document.Sentences[i];

                int lastLPosition = -1;
                int lastPPosition = -1;
                int lastTPosition = -1;
                TimeSpan? lastEndTime = null;

                for (int j = 0; j < sentence.Mappings.Count; j++)
                {
                    var mapping = sentence.Mappings[j];

                    // 누락 검사
                    if (existLR && mapping.Lyrics == null)
                    {
                        Write($"{i}번 문장의 {j}번 가사 매핑이 누락되었습니다.", sentence, mapping);
                    }
                    if (existPR && mapping.Pronounce == null)
                    {
                        Write($"{i}번 문장의 {j}번 발음 매핑이 누락되었습니다.", sentence, mapping);
                    }
                    if (existTR && mapping.Translation == null)
                    {
                        Write($"{i}번 문장의 {j}번 해석 매핑이 누락되었습니다.", sentence, mapping);
                    }

                    // 시간 검사
                    if (mapping.BeginTime > mapping.EndTime)
                    {
                        Write($"{i}번 문장의 {j}번 매핑의 시작 시간이 종료 시간보다 느립니다.", sentence, mapping);
                    }
                    if (sentence.BeginTime > mapping.BeginTime)
                    {
                        Write($"{i}번 문장의 {j}번 매핑의 시작 시간이 문장의 시작 시간보다 빠릅니다.", sentence, mapping);
                    }
                    if (lastEndTime != null && mapping.BeginTime < lastEndTime)
                    {
                        Write($"{i}번 문장의 {j}번 매핑의 시작 시간이 이전 매핑의 종료 시간보다 빠릅니다.", sentence, mapping);
                    }

                    // 위치 검사
                    if (mapping.Lyrics?.Position > sentence.Lyrics?.Length - 1)
                    {
                        Write($"{i}번 문장의 {j}번 가사 매핑의 위치가 범위를 초과하였습니다.", sentence, mapping);
                    }
                    if (mapping.Pronounce?.Position > sentence.Pronounce?.Length - 1)
                    {
                        Write($"{i}번 문장의 {j}번 발음 매핑의 위치가 범위를 초과하였습니다.", sentence, mapping);
                    }
                    if (mapping.Translation?.Position > sentence.Translation?.Length - 1)
                    {
                        Write($"{i}번 문장의 {j}번 해석 매핑의 위치가 범위를 초과하였습니다.", sentence, mapping);
                    }

                    if (mapping.Lyrics?.Position < lastLPosition)
                    {
                        Write($"{i}번 문장의 {j}번 가사 매핑의 위치값이 이전 가사 매핑의 위치값보다 작습니다.", sentence, mapping);
                    }
                    if (mapping.Pronounce?.Position < lastPPosition)
                    {
                        Write($"{i}번 문장의 {j}번 발음 매핑의 위치값이 이전 발음 매핑의 위치값보다 작습니다.", sentence, mapping);
                    }
                    if (mapping.Translation?.Position < lastTPosition)
                    {
                        Write($"{i}번 문장의 {j}번 해석 매핑의 위치값이 이전 해석 매핑의 위치값보다 작습니다.", sentence, mapping);
                    }

                    lastEndTime = mapping.EndTime;
                    lastLPosition = Math.Max(lastLPosition, mapping.Lyrics?.Position ?? -1);
                    lastPPosition = Math.Max(lastPPosition, mapping.Pronounce?.Position ?? -1);
                    lastTPosition = Math.Max(lastTPosition, mapping.Translation?.Position ?? -1);
                }

                // 매핑 완성 검사
                var lastMapping = sentence.Mappings.Last();

                if (lastMapping.Lyrics?.Position < sentence.Lyrics?.Length - 1)
                {
                    Write($"{i}번 문장의 해석이 끝까지 매핑되지 않았습니다.", sentence);
                }
                if (lastMapping.Pronounce?.Position < sentence.Pronounce?.Length - 1)
                {
                    Write($"{i}번 문장의 발음이 끝까지 매핑되지 않았습니다.", sentence);
                }
                if (lastMapping.Translation?.Position < sentence.Translation?.Length - 1)
                {
                    Write($"{i}번 문장의 해석이 끝까지 매핑되지 않았습니다.", sentence);
                }
            }
        }

        private void Write(string value, LyricsSentence sentence = null, LyricsMapping mapping = null)
        {
            _Results.Add(new LyricsVerificationResult
            {
                _Message = value,
                _Mapping = mapping,
                _Sentence = sentence
            });
        }
        #endregion
    }
}
