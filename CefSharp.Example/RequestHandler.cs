using System;

namespace CefSharp.Example
{
    using System.Collections.Specialized;

    public class RequestHandler : IRequestHandler
    {
        public static readonly string VersionNumberString = String.Format("Chromium: {0}, CEF: {1}, CefSharp: {2}",
            Cef.ChromiumVersion, Cef.CefVersion, Cef.CefSharpVersion);

        bool IRequestHandler.OnBeforeBrowse(IWebBrowser browser, IRequest request, bool isRedirect, bool isMainFrame)
        {
            return false;
        }

        bool IRequestHandler.OnCertificateError(IWebBrowser browser, CefErrorCode errorCode, string requestUrl)
        {
            return false;
        }

        void IRequestHandler.OnPluginCrashed(IWebBrowser browser, string pluginPath)
        {
            // TODO: Add your own code here for handling scenarios where a plugin crashed, for one reason or another.
        }

        bool IRequestHandler.OnBeforeResourceLoad(IWebBrowser browser, IRequest request, IResponse response, bool isMainFrame)
        {
            return false;
        }

        bool IRequestHandler.GetAuthCredentials(IWebBrowser browser, bool isProxy, string host, int port, string realm, string scheme, ref string username, ref string password)
        {
            return false;
        }

        bool IRequestHandler.OnBeforePluginLoad(IWebBrowser browser, string url, string policyUrl, WebPluginInfo info)
        {
            bool blockPluginLoad = false;

            // Enable next line to demo: Block any plugin with "flash" in its name
            // try it out with e.g. http://www.youtube.com/watch?v=0uBOtQOO70Y
            //blockPluginLoad = info.Name.ToLower().Contains("flash");
            return blockPluginLoad;
        }

        void IRequestHandler.OnRenderProcessTerminated(IWebBrowser browser, CefTerminationStatus status)
        {
            // TODO: Add your own code here for handling scenarios where the Render Process terminated for one reason or another.
        }

        public void OnResponseStarted(IWebBrowser browser, IRequest request, bool isMainFrame)
        {
            throw new NotImplementedException();
        }

        public void OnBeforeSendHeaders(IWebBrowser browser, IRequest request, bool isMainFrame, NameValueCollection headers, bool viaProxy)
        {
            throw new NotImplementedException();
        }

        public void OnSendHeaders(IWebBrowser browser, IRequest request, bool isMainFrame, NameValueCollection headers)
        {
            throw new NotImplementedException();
        }

        public bool OnHeadersReceived(IWebBrowser browser, IRequest request, bool isMainFrame, string originalHeaders, out string modifiedHeaders, string allowedUnsafeRedirectUrl)
        {
            throw new NotImplementedException();
        }

        public void OnBeforeRedirect(IWebBrowser browser, IRequest request, bool isMainFrame, string newLocation)
        {
            throw new NotImplementedException();
        }

        public void OnRawBytesRead(IWebBrowser browser, IRequest request, bool isMainFrame, int bytesRead)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted(IWebBrowser browser, IRequest request, bool isMainFrame, bool started)
        {
            throw new NotImplementedException();
        }

        public void OnURLRequestDestroyed(IWebBrowser browser, IRequest request, bool isMainFrame)
        {
            throw new NotImplementedException();
        }
    }
}
