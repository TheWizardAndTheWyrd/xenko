// Copyright (c) Xenko contributors (https://xenko.com) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EnvDTE;
using Process = System.Diagnostics.Process;

namespace Xenko.Core.VisualStudio
{
    /// <summary>
    /// Helper class to locate Visual Studio instances.
    /// </summary>
    class VisualStudioDTE
    {
        public static IEnumerable<Process> GetActiveInstances()
        {
            return GetActiveDTEs().Select(x => x.ProcessId).Select(Process.GetProcessById);
        }

        public static DTE GetDTEByProcess(int processId)
        {
            return GetActiveDTEs().FirstOrDefault(x => x.ProcessId == processId).DTE;
        }

        /// <summary>
        /// Gets the instances of active <see cref="EnvDTE.DTE"/>.
        /// </summary>
        /// <returns></returns>
        internal static IEnumerable<Instance> GetActiveDTEs()
        {
            IRunningObjectTable rot;
            if (GetRunningObjectTable(0, out rot) == 0)
            {
                IEnumMoniker enumMoniker;
                rot.EnumRunning(out enumMoniker);

                var moniker = new IMoniker[1];
                while (enumMoniker.Next(1, moniker, IntPtr.Zero) == 0)
                {
                    IBindCtx bindCtx;
                    CreateBindCtx(0, out bindCtx);
                    string displayName;
                    moniker[0].GetDisplayName(bindCtx, null, out displayName);

                    // Check if it's Visual Studio
                    if (displayName.StartsWith("!VisualStudio"))
                    {
                        object obj;
                        rot.GetObject(moniker[0], out obj);

                        // Cast as DTE
                        var dte = obj as DTE;
                        if (dte != null)
                        {
                            yield return new Instance
                            {
                                DTE = dte,
                                ProcessId = int.Parse(displayName.Split(':')[1])
                            };
                        }
                    }
                }
            }
        }

        public struct Instance
        {
            public DTE DTE;

            public int ProcessId;
        }

        [DllImport("ole32.dll")]
        private static extern void CreateBindCtx(int reserved, out IBindCtx ppbc);

        [DllImport("ole32.dll")]
        private static extern int GetRunningObjectTable(int reserved, out IRunningObjectTable prot);
    }
}
