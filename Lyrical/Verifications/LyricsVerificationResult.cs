namespace Lyrical.Verifications
{
    /// <summary>
    /// 가사 문서의 검증 결과를 제공하는 클래스를 나타냅니다.
    /// </summary>
    public class LyricsVerificationResult
    {
        /// <summary>
        /// 결과가 발생된 매핑을 가져옵니다.
        /// </summary>
        public LyricsMapping Mapping
        {
            get
            {
                return _Mapping;
            }
        }
        internal LyricsMapping _Mapping;

        /// <summary>
        /// 결과가 발생된 문장을 가져옵니다.
        /// </summary>
        public LyricsSentence Sentence
        {
            get
            {
                return _Sentence;
            }
        }
        internal LyricsSentence _Sentence;

        /// <summary>
        /// 검증 결과의 오류 메시지를 가져옵니다.
        /// </summary>
        public string Message
        {
            get
            {
                return _Message;
            }
        }
        internal string _Message;
    }
}
