namespace PS
{
    public interface IFreezable
    {
        #region Properties

        bool IsFrozen { get; }

        #endregion

        #region Members

        void Freeze();

        #endregion
    }
}