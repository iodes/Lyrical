namespace Lyrical.Base
{
    /// <summary>
    /// 객체가 특정 객체의 자식임을 명시합니다.
    /// </summary>
    /// <typeparam name="T">부모 객체의 형식입니다.</typeparam>
    public interface ILyricsChild<T>
    {
        T Parent { get; }
    }
}
