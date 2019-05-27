namespace Ez.Scripts
{
    /// <summary>
    /// Implement this interface on an EzSystem to allow it
    /// to update in real time.
    /// </summary>
    public interface IUpdatableSystem
    {
        void OnUpdate(EzEntitySpace space);
    }
}