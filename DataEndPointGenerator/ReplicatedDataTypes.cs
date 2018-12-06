using Olive.Entities.Replication;
using System;

namespace OliveGenerator
{
    class ReplicatedDataType
    {
        public Type Type;
        public ReplicatedData ReplicatedDataObject;

        public ReplicatedDataType(Type type, ReplicatedData replicatedDataObject)
        {
            Type = type;
            ReplicatedDataObject = replicatedDataObject;
        }
    }
}
