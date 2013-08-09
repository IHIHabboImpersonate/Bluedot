namespace IHI.Server.Database
{
    public interface IPersistableStorage
    {
        long PersistableInstanceId
        {
            get;
        }

        PersistentStorage PersistentStorage
        {
            get;
        }
    }
}