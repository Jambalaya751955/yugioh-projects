using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDBUpdater.Helpers
{
    static class Threading
    {
        public static List<Task> WaitAll(this List<Task> tasks)
        {
            //foreach (Task task in tasks)
            for (int i = 0; i < tasks.Count; ++i)
                if (tasks[i] != null)
                    tasks[i].Wait();
            return tasks;
        }

        public static Task[] WaitAll(this Task[] tasks)
        {
            //foreach (Task task in tasks)
            for (int i = 0; i < tasks.Length; ++i)
                if (tasks[i] != null)
                    tasks[i].Wait();
            return tasks;
        }
    }
}
