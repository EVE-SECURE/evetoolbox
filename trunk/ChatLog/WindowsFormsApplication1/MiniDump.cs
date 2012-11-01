using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace WindowsFormsApplication1
{

        public static class DebugHelper
        {
            static class MinidumpType
            {
                public const int MiniDumpNormal = 0x00000000;
                public const int MiniDumpWithDataSegs = 0x00000001;
                public const int MiniDumpWithFullMemory = 0x00000002;
                public const int MiniDumpWithHandleData = 0x00000004;
                public const int MiniDumpFilterMemory = 0x00000008;
                public const int MiniDumpScanMemory = 0x00000010;
                public const int MiniDumpWithUnloadedModules = 0x00000020;
                public const int MiniDumpWithIndirectlyReferencedMemory = 0x00000040;
                public const int MiniDumpFilterModulePaths = 0x00000080;
                public const int MiniDumpWithProcessThreadData = 0x00000100;
                public const int MiniDumpWithPrivateReadWriteMemory = 0x00000200;
                public const int MiniDumpWithoutOptionalData = 0x00000400;
                public const int MiniDumpWithFullMemoryInfo = 0x00000800;
                public const int MiniDumpWithThreadInfo = 0x00001000;
                public const int MiniDumpWithCodeSegs = 0x00002000;
            }

            [DllImport("dbghelp.dll")]
            static extern bool MiniDumpWriteDump(IntPtr hProcess, Int32 processId, IntPtr hFile, int dumpType, IntPtr exceptionParam, IntPtr userStreamParam, IntPtr callackParam);

            /// <summary>
            /// 为指定的进程使用指定的路径创建DUMP文件
            /// </summary>
            /// <param name="process">要为其创建DUMP文件的进程</param>
            /// <param name="path">DUMP文件路径（自动添加“.dmp”扩展名）</param>
            public static void CreateMiniDump(this System.Diagnostics.Process process, string path)
            {
                using (var fs = new FileStream(path + ".dmp", FileMode.Create))
                {
                    if (fs.SafeFileHandle == null)
                        throw new NullReferenceException("fs.SafeFileHandle");

                    MiniDumpWriteDump(process.Handle, process.Id,
                                      fs.SafeFileHandle.DangerousGetHandle(),
                                      MinidumpType.MiniDumpNormal,
                                      IntPtr.Zero,
                                      IntPtr.Zero,
                                      IntPtr.Zero);
                }
            }

            /// <summary>
            /// 为当前的进程使用指定的路径创建DUMP文件
            /// </summary>
            /// <param name="path">DUMP文件路径（自动添加“.dmp”扩展名）</param>
            public static void CreateMiniDump(string path)
            {
                using (System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess())
                {
                    process.CreateMiniDump(path);
                }
            }

            /// <summary>
            /// 为当前的进程创建DUMP文件
            /// </summary>
            public static void CreateMiniDump()
            {
                var currentFile = new FileInfo(Assembly.GetCallingAssembly().Location);
                var dir = currentFile.Directory.FullName;
                var shortName = DateTime.Now.ToString("yyyyMMddHHmmss");
                var fullPath = Path.Combine(dir, shortName);

                CreateMiniDump(fullPath);
            }
        }
    

}
