using System;

namespace ZeroPass
{
    public class BucketUpdater<DataType> : UpdateBucketWithUpdater<DataType>.IUpdater
    {
        private Action<DataType, float> callback;

        public BucketUpdater(Action<DataType, float> callback)
        {
            this.callback = callback;
        }

        public void Update(DataType data, float dt)
        {
            callback(data, dt);
        }
    }
}