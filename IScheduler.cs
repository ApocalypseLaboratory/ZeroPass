using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroPass
{
    public interface IScheduler
    {
        SchedulerHandle Schedule(string name, float time, Action<object> callback, object callback_data = null, SchedulerGroup group = null);
    }
}
