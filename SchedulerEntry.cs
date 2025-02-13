using System;

namespace ZeroPass
{
    public struct SchedulerEntry
    {
        public class Details
        {
            public Action<object> callback;

            public object callbackData;

            public float timeInterval;

            public Details(string name, Action<object> callback, object callback_data, float time_interval)
            {
                timeInterval = time_interval;
                this.callback = callback;
                callbackData = callback_data;
            }
        }

        public float time;

        public Details details
        {
            get;
            private set;
        }

        public Action<object> callback => details.callback;

        public object callbackData => details.callbackData;

        public float timeInterval => details.timeInterval;

        public SchedulerEntry(string name, float time, float time_interval, Action<object> callback, object callback_data)
        {
            this.time = time;
            details = new Details(name, callback, callback_data, time_interval);
        }

        public void FreeResources()
        {
            details = null;
        }

        public override string ToString()
        {
            return time.ToString();
        }

        public void Clear()
        {
            details.callback = null;
        }
    }
}
