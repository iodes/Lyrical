using System.Collections.ObjectModel;

namespace Lyrical.Collections
{
    /// <summary>
    /// 가사의 매핑 정보를 제공하는 데이터 컬렉션을 나타냅니다.
    /// </summary>
    public sealed class MappingCollection : ObservableCollection<LyricsMapping>
    {
        internal MappingCollection()
        {

        }
    }
}
