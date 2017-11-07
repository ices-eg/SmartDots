using System;
using System.Runtime.InteropServices;

namespace SmartDots.Helpers
{
    public static class FolderHelper
    {
        [DllImport("mpr.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.U4)]
        static extern int WNetGetUniversalName(
    string lpLocalPath,
    [MarshalAs(UnmanagedType.U4)] int dwInfoLevel,
    IntPtr lpBuffer,
    [MarshalAs(UnmanagedType.U4)] ref int lpBufferSize);
        /// <summary>
        ///   Gets the UNC path for the path passed in.
        /// </summary>
        /// <param name="path">The path for which we want the UNC path.</param>
        /// <returns>The UNC path.  Returns empty string if an error has occurred. </returns>
        public static string GetUniversalPath(string path)
        {
            const int UNIVERSAL_NAME_INFO_LEVEL = 0x00000001;
            const int ERROR_MORE_DATA = 234;
            const int NOERROR = 0;

            string retVal = null;

            // Pointer to the memory buffer to hold the result.
            IntPtr buffer = IntPtr.Zero;

            try
            {
                // First, call WNetGetUniversalName to get the size.
                // Passing (IntPtr)IntPtr.Size as the third parameter because WNetGetUniversalName doesn't
                // like NULL, and IntPtr.Size will always be a properly-aligned (if not actually valid)
                // IntPtr value.
                int size = 0;
                int apiRetVal = WNetGetUniversalName(path, UNIVERSAL_NAME_INFO_LEVEL, (IntPtr)IntPtr.Size, ref size);

                if (apiRetVal == ERROR_MORE_DATA)
                {
                    // Allocate the memory.
                    buffer = Marshal.AllocCoTaskMem(size);

                    // Now make the call.
                    apiRetVal = WNetGetUniversalName(path, UNIVERSAL_NAME_INFO_LEVEL, buffer, ref size);

                    if (apiRetVal == NOERROR)
                    {
                        // Now get the string.  It's all in the same buffer, but
                        // the pointer is first, so offset the pointer by IntPtr.Size
                        // and pass to PtrToStringAnsi.
                        retVal = Marshal.PtrToStringAuto(new IntPtr(buffer.ToInt64() + IntPtr.Size), size);
                        retVal = retVal.Substring(0, retVal.IndexOf('\0'));
                    }
                }
            }
            catch
            {
                // I know swallowing exceptions is nasty...
                retVal = "";
            }
            finally
            {
                Marshal.FreeCoTaskMem(buffer);
            }

            return retVal;
        }
    }
}
