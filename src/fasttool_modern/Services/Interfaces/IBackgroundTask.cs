using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace fasttool_modern.Services.Interfaces
{
    public interface IBackgroundTask
    {
        void Start();
        void Stop();
    }
}
