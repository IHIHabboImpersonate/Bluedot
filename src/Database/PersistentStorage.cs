
using IHI.Server.Database;
using IHI.Server.Database.Actions;

namespace IHI.Server
{
    public class PersistentStorage
    {
        private readonly IPersistableStorage _persistableStorage;

        public PersistentStorage(IPersistableStorage persistableStorage)
        {
            _persistableStorage = persistableStorage;
        }

        public byte[] this[string name]
        {
            get
            {
                string typeName = _persistableStorage.GetType().FullName;
                long instanceId = _persistableStorage.PersistableInstanceId;

                return PersistenceActions.GetPersistentValue(typeName, instanceId, name);
            }
            set
            {
                string typeName = _persistableStorage.GetType().FullName;
                long instanceId = _persistableStorage.PersistableInstanceId;

                PersistenceActions.SetPersistentValue(typeName, instanceId, name, value);
            }
        }
    }
}
