using System;
using System.Linq;

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
                long instanceId = _persistable.PersistInstanceProperty();

                try
                {
                    using (Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
                    {
                        return dbSession.
                            PersistentStorage.
                            Where(
                                variable => variable.TypeName == typeName &&
                                            variable.InstanceId == instanceId &&
                                            variable.VariableName == name).
                            Select(variable => new { variable.Value }).
                            Single().
                            Value;
                    }
                }
                catch (InvalidOperationException)
                {
                    return null;
                }
            }
            set
            {
                string typeName = _persistable.GetType().FullName;
                long instanceId = _persistable.PersistInstanceProperty();

                lock (this)
                {
                    using (Session dbSession = CoreManager.ServerCore.GetDatabaseSession())
                    {
                        try
                        {
                            DBPersistentStorage persistentVariable = dbSession.
                                PersistentStorage.Where(
                                    variable => variable.TypeName == typeName &&
                                                variable.InstanceId == instanceId &&
                                                variable.VariableName == name).
                                Single();

                            if (value != null)
                                persistentVariable.Value = value;
                            else
                                dbSession.PersistentStorage.DeleteObject(persistentVariable);
                        }
                        catch (InvalidOperationException) // Doesn't already exist.
                        {
                            dbSession.PersistentStorage.AddObject(new DBPersistentStorage
                                                                         {
                                                                             TypeName = typeName,
                                                                             InstanceId = instanceId,
                                                                             VariableName = name,
                                                                             Value = value
                                                                         });
                        }
                        dbSession.SaveChanges();
                    }
                }
            }
        }
    }
}
