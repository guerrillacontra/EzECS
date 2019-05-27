namespace Ez.Scripts
{
    /// <summary>
    /// Implement this interface on an EzSystem to allow it
    /// to fixed-update in real time.
    /// </summary>
    public interface IFixedUpdatableSystem
    {
        void OnFixedUpdate(EzEntitySpace space);
    }
}