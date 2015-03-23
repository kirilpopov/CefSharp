﻿using System.Collections.Generic;
using System.Linq;

namespace CefSharp.BrowserSubprocess
{
    using System;
    using System.ServiceModel;
    using Internals;

    public class CefSubProcess : CefAppWrapper
    {
        private readonly IEnumerable<string> args;
        private int? parentBrowserId;
        protected List<CefBrowserWrapper> browsers = new List<CefBrowserWrapper>();

        public int? ParentProcessId { get; private set; }

        internal CefSubProcess(IEnumerable<string> args)
        {
            this.args = args;
            LocateParentProcessId(args);
        }

        private void LocateParentProcessId(IEnumerable<string> args)
        {
            // Format being parsed:
            // --channel=3828.2.1260352072\1102986608
            // We only really care about the PID (3828) part.
            const string channelPrefix = "--channel=";
            var channelArgument = args.SingleOrDefault(arg => arg.StartsWith(channelPrefix));
            if (channelArgument == null)
            {
                return;
            }

            var parentProcessId = channelArgument
                .Substring(channelPrefix.Length)
                .Split('.')
                .First();
            ParentProcessId = int.Parse(parentProcessId);
        }

        public override void OnBrowserCreated(CefBrowserWrapper browser)
        {
            browsers.Add(browser);

            if (parentBrowserId == null)
            {
                parentBrowserId = browser.BrowserId;
            }

            if (ParentProcessId == null || parentBrowserId == null)
            {
                return;
            }

            var browserId = browser.IsPopup ? parentBrowserId.Value : browser.BrowserId;

            SetBrowserProcess(browser, browserId);
            SetLoggingProcess(browser, browserId);
            
            browser.LogProcess.Log(new StartupArgumentsLogMessage(args));
        }

        private void SetBrowserProcess(CefBrowserWrapper browser, int browserId)
        {
            var serviceName = RenderprocessClientFactory.GetServiceName(ParentProcessId.Value, browserId);

            var binding = BrowserProcessServiceHost.CreateBinding();

            var channelFactory = new DuplexChannelFactory<IBrowserProcess>(
                this,
                binding,
                new EndpointAddress(serviceName)
                );

            channelFactory.Open();

            var browserProcess = channelFactory.CreateChannel();
            var clientChannel = ((IClientChannel)browserProcess);

            try
            {
                clientChannel.Open();
                if (!browser.IsPopup)
                {
                    browserProcess.Connect();
                }

                var javascriptObject = browserProcess.GetRegisteredJavascriptObjects();

                if (javascriptObject.MemberObjects.Count > 0)
                {
                    browser.JavascriptRootObject = javascriptObject;
                }

                browser.ChannelFactory = channelFactory;
                browser.BrowserProcess = browserProcess;
            }
            catch (Exception)
            {
            }
        }

        private void SetLoggingProcess(CefBrowserWrapper browser, int browserId)
        {
            var serviceName = RenderprocessClientFactory.GetLogServiceName(ParentProcessId.Value, browserId);

            var binding = BrowserProcessServiceHost.CreateBinding();

            var channelFactory = new ChannelFactory<ILogService>(                
                binding,
                new EndpointAddress(serviceName));

            channelFactory.Open();

            var logProcess = channelFactory.CreateChannel();
            var clientChannel = ((IClientChannel)logProcess);

            try
            {
                clientChannel.Open();

                browser.LogProcess = logProcess;
            }
            catch (Exception)
            {
            }
        }

        protected override void DoDispose(bool isDisposing)
        {
            foreach (var browser in browsers)
            {
                browser.Dispose();
            }

            browsers = null;

            base.DoDispose(isDisposing);
        }

        public override void OnBrowserDestroyed(CefBrowserWrapper browser)
        {
            browsers.Remove(browser);

            var channelFactory = browser.ChannelFactory;

            if (channelFactory.State == CommunicationState.Opened)
            {
                channelFactory.Close();
            }

            var clientChannel = ((IClientChannel)browser.BrowserProcess);

            if (clientChannel.State == CommunicationState.Opened)
            {
                clientChannel.Close();
            }

            browser.ChannelFactory = null;
            browser.BrowserProcess = null;
            browser.JavascriptRootObject = null;
        }

        public void Log(string message)
        { 
            foreach(var browser in browsers)
            {
                browser.LogProcess.Log(new LogMessage(message));
            }
        }

        public void Log(Exception exception)
        {
            foreach (var browser in browsers)
            {
                browser.LogProcess.Log(new LogMessage(exception));
            }
        }

        public void Log(string message, Exception exception)
        {
            foreach (var browser in browsers)
            {
                browser.LogProcess.Log(new LogMessage(message, exception));
            }
        }
    }
}