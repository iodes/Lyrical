using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Lyrical.Base
{
    /// <summary>
    /// 속성 변경 이벤트를 발생시킬 수 있는 가사 객체입니다.
    /// </summary>
    public abstract class LyricsEvent : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
