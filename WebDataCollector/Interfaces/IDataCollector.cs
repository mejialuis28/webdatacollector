using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebDataCollector.Interfaces
{
    interface IDataCollector
    {
        Task<List<Dictionary<string, string>>> Collect();

        Task CollectAndPublish();
    }
}
