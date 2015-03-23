// Copyright © 2010-2014 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CefSharp.Internals;

namespace CefSharp.BrowserSubprocess
{
    public class Program
    {
        private static CefSubProcess subProcess;
        public static int Main(string[] args)
        {
            Kernel32.OutputDebugString("BrowserSubprocess starting up with command line: " + String.Join("\n", args));
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            int result;

            using (subProcess = Create(args))
            {
                //if (subprocess is CefRenderProcess)
                //{
                //    MessageBox.Show("Please attach debugger now", null, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //}

                result = subProcess.Run();
            }

            Kernel32.OutputDebugString("BrowserSubprocess shutting down.");
            return result;
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;

            try
            {
                if (subProcess != null)
                {
                    subProcess.Log("Unhandled exception in " + subProcess.GetType() + exception.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(exception.ToString());
                MessageBox.Show(ex.ToString());
            }
        }

        public static CefSubProcess Create(IEnumerable<string> args)
        {
            const string typePrefix = "--type=";
            var typeArgument = args.SingleOrDefault(arg => arg.StartsWith(typePrefix));

            var type = typeArgument.Substring(typePrefix.Length);

            switch (type)
            {
                case "renderer":
                    return new CefRenderProcess(args);
                case "gpu-process":
                    return new CefGpuProcess(args);
                default:
                    return new CefSubProcess(args);
            }
        }
    }
}
