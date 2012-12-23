
using Bluedot.HabboServer.Database.Actions;

namespace Bluedot.HabboServer.Database
{

    public class PersistentStorage
    {
        private readonly IPersistable _persistable;

        public PersistentStorage(IPersistable persistable)
        {
            _persistable = persistable;
        }

        public byte[] this[string name]
        {
            get
            {
                string typeName = _persistable.GetType().FullName;
                long instanceId = _persistable.PersistableInstanceId;

                return PersistenceActions.GetPersistentValue(typeName, instanceId, name);
            }
            set
            {
                string typeName = _persistable.GetType().FullName;
                long instanceId = _persistable.PersistableInstanceId;

                PersistenceActions.SetPersistentValue(typeName, instanceId, name, value);
            }
        }
    }
}
