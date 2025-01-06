/**
 * MIT License
 * 
 * Copyright (c) 2024 Andrew D. King
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.IO;
using System.Reflection;
using System.Text;

using UnityEngine;

namespace LabBenchStudios.Pdt.Unity.Common
{
    /// <summary>
    /// This is a simple console logging class intended to be used for
    /// redirecting library console output (e.g., Console.WriteLine()) messages
    /// to the Unity console. It supports both stdout and stderr logging.
    /// 
    /// NOTE: It is currently designed to support string-based messages only.
    /// </summary>
    public static class UnityConsoleOutputManager
    {
        // static

        /// <summary>
        /// Ensure one console info logging instance at runtime.
        /// </summary>
        private static UnityConsoleDebugWriter _UNITY_LOG_DEBUG_WRITER = new UnityConsoleDebugWriter();

        /// <summary>
        /// Ensure one console error logging instance at runtime.
        /// </summary>
        private static UnityConsoleErrorWriter _UNITY_LOG_ERROR_WRITER = new UnityConsoleErrorWriter();

        /// <summary>
        /// 
        /// </summary>
        public static void EnablePluginConsoleLogging()
        {
            EnablePluginConsoleLogging(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enableDiagFlag"></param>
        public static void EnablePluginConsoleLogging(bool enableDiagFlag)
        {
            _UNITY_LOG_DEBUG_WRITER.SetEnableDiagnosticsFlag(enableDiagFlag);
            Console.SetOut(_UNITY_LOG_DEBUG_WRITER);

            _UNITY_LOG_ERROR_WRITER.SetEnableDiagnosticsFlag(enableDiagFlag);
            Console.SetError(_UNITY_LOG_ERROR_WRITER);
        }


        // inner class declarations

        /// <summary>
        /// Redirector class for stderr to Unity error console.
        /// </summary>
        private class UnityConsoleErrorWriter : BaseUnityLogWriter
        {
            /// <summary>
            /// Overridden RedirectBuffer() method redirects stored buffer to
            /// Unity's error console output.
            /// </summary>
            protected override void RedirectBuffer()
            {
                Debug.LogError(base.GetOutputBuffer());
            }

        }

        /// <summary>
        /// Redirector class for stdout to Unity info console.
        /// </summary>
        private class UnityConsoleDebugWriter : BaseUnityLogWriter
        {
            /// <summary>
            /// Overridden RedirectBuffer() method redirects stored buffer to
            /// Unity's standard console output.
            /// </summary>
            protected override void RedirectBuffer()
            {
                Debug.Log(base.GetOutputBuffer());
            }

        }

        /// <summary>
        /// Base class handles intercepting Console output.
        /// </summary>
        protected abstract class BaseUnityLogWriter : TextWriter
        {
            /// <summary>
            /// 
            /// </summary>
            private bool enableTiming = true;

            /// <summary>
            /// 
            /// </summary>
            private bool enableDiag = false;

            /// <summary>
            /// 
            /// </summary>
            private bool prependNewline = false;

            //
            /// <summary>
            /// 
            /// </summary>
            private StringBuilder strBuilder = new StringBuilder();

            /// <summary>
            /// Use system default encoding.
            /// </summary>
            public override Encoding Encoding
            {
                get { return Encoding.Default; }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public string GetOutputBuffer()
            {
                string outStr = "";

                if (this.enableTiming) {
                    outStr = "[" + DateTime.Now.ToUniversalTime().ToString() + "]: ";
                }

                // NOTE: This is rather expensive - use sparingly
                if (this.enableDiag) {
                    outStr += this.GetCallStackInfo();
                }

                if (this.prependNewline) {
                    outStr = NewLine + outStr;
                }

                outStr += this.strBuilder.ToString() + NewLine;

                return outStr;
            }

            /// <summary>
            /// This is a very expensive operation, and may not add the value expected,
            /// depending on the actual call stack and whether or not it the calling
            /// stack is unwindable. Use sparingly!
            /// </summary>
            /// <returns></returns>
            public string GetCallStackInfo()
            {
                System.Diagnostics.StackTrace stack = new System.Diagnostics.StackTrace(true);
                System.Diagnostics.StackFrame[] stackFrames = stack.GetFrames();

                StringBuilder builder = new StringBuilder(NewLine);
                builder.Append("Call stack:").Append(NewLine);

                foreach (System.Diagnostics.StackFrame stackFrame in stackFrames) {
                    MethodBase method = stackFrame.GetMethod();
                    string sourceFile = stackFrame.GetFileName();
                    string sourceModule = Path.GetFileNameWithoutExtension(sourceFile);

                    if (string.IsNullOrEmpty(sourceModule)) { sourceModule = "    "; }

                    builder.Append($"    {sourceModule}.{method.Name}:{stackFrame.GetFileLineNumber()}").Append(NewLine);
                }

                builder.Append(NewLine);

                return builder.ToString();
            }

            /// <summary>
            /// Setting this flag to true will incur significant overhead for
            /// each redirected log message, as it requires creation of a
            /// full call stack and extraction of each call stack entry
            /// (source module, method, line number) into a line of text.
            /// 
            /// Set this flag only when granular debugging is required.
            /// 
            /// </summary>
            /// <param name="enable"></param>
            public void SetEnableDiagnosticsFlag(bool enable)
            {
                this.enableDiag = enable;
            }

            /// <summary>
            /// Setting this flag to true will incur some additional overhead for
            /// each redirected log message, as it requires creation of time
            /// and date stamp to be prepended to the message.
            /// 
            /// </summary>
            /// <param name="enable"></param>
            public void SetEnableTimingFlag(bool enable)
            {
                this.enableTiming = enable;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="msg"></param>
            public override void Write(string msg)
            {
                if (msg != null)
                {
                    this.strBuilder.Append(msg).Append(NewLine);
                    //this.Flush();
                    this.RedirectBuffer();
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="msg"></param>
            public override void WriteLine(string msg)
            {
                if (msg != null)
                {
                    this.strBuilder.Append(msg).Append(NewLine);
                    //this.Flush();
                    this.RedirectBuffer();
                }
            }

            // protected methods

            /// <summary>
            /// 
            /// </summary>
            protected abstract void RedirectBuffer();

        }

    }

}
