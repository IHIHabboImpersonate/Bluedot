namespace Bluedot.HabboServer.Database
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