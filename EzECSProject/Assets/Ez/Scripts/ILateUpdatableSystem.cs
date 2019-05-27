namespace Ez.Scripts
{
    /// <summary>
    /// Implement this interface on an EzSystem to allow it
    /// to late-update in real time.
    /// </summary>
    public interface ILateUpdatableSystem
    {
        void OnLateUpdate(EzEntitySpace space);
    }
}