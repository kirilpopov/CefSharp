using System;
using System.Linq;
using System.Threading.Tasks;
using CefSharp.Internals;
using System.Collections.Generic;
using System.ServiceModel;
using TaskExtensions = CefSharp.Internals.TaskExtensions;

namespace CefSharp.BrowserSubprocess
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, IncludeExceptionDetailInFaults=true)]
    public class CefRenderProcess : CefSubProcess, IRenderProcess
    {      
        public CefRenderProcess(IEnumerable<string> args) 
            : base(args)
        {
        }
              
        public Task<JavascriptResponse> EvaluateScriptAsync(int browserId, long frameId, string script, TimeSpan? timeout)
        {            
            var factory = RenderThreadTaskFactory;
            var browser = browsers.FirstOrDefault(x => x.BrowserId == browserId);
            if (browser == null)
            {
                return TaskExtensions.FromResult(new JavascriptResponse
                {
                    Success = false,
                    Message = string.Format("Browser with Id {0} not found in Render Sub Process.", browserId)
                });
            }

            var task = factory.StartNew(() =>
            {
                try
                {
                    var response = browser.DoEvaluateScript(frameId, script);

                    return response;
                }
                catch (Exception ex)
                {
                    return new JavascriptResponse
                    {
                        Success = false,
                        Message = ex.ToString()
                    };
                }
            }, TaskCreationOptions.AttachedToParent);

            return timeout.HasValue ? task.WithTimeout(timeout.Value) : task;
        }

        public IAsyncResult BeginEvaluateScriptAsync(int browserId, long frameId, string script, TimeSpan? timeout, AsyncCallback callback, object state)
        {
            var tcs = new TaskCompletionSource<JavascriptResponse>(state);
            var task = EvaluateScriptAsync(browserId, frameId, script, timeout);
            task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    tcs.TrySetException(t.Exception.InnerExceptions);
                }
                else if (t.IsCanceled)
                {
                    tcs.TrySetCanceled();
                }
                else
                {
                    tcs.TrySetResult(t.Result);
                }

                if (callback != null)
                {
                    callback(tcs.Task);
                }
            });
            return tcs.Task;
        }

        public JavascriptResponse EndEvaluateScriptAsync(IAsyncResult result)
        {
            return ((Task<JavascriptResponse>)result).Result;
        }

        public IAsyncResult BeginJavascriptCallbackAsync(int browserId, long callbackId, object[] parameters, TimeSpan? timeout, AsyncCallback callback, object state)
        {
            var tcs = new TaskCompletionSource<JavascriptResponse>(state);
            var task = JavascriptCallbackAsync(browserId, callbackId, parameters, timeout);
            task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    tcs.TrySetException(t.Exception.InnerExceptions);
                }
                else if (t.IsCanceled)
                {
                    tcs.TrySetCanceled();
                }
                else
                {
                    tcs.TrySetResult(t.Result);
                }

                if (callback != null)
                {
                    callback(tcs.Task);
                }
            });
            return tcs.Task;
        }

        public JavascriptResponse EndJavascriptCallbackAsync(IAsyncResult result)
        {
            return ((Task<JavascriptResponse>)result).Result;
        }

        public void DestroyJavascriptCallback(int browserId, long id)
        {
            var browser = browsers.FirstOrDefault(x => x.BrowserId == browserId);
            if (browser != null)
            {
                browser.DestroyJavascriptCallback(id);
            }
        }

        private Task<JavascriptResponse> JavascriptCallbackAsync(int browserId, long callbackId, object[] parameters, TimeSpan? timeout)
        {
            var factory = RenderThreadTaskFactory;
            var browser = browsers.FirstOrDefault(x => x.BrowserId == browserId);
            if (browser == null)
            {
                return TaskExtensions.FromResult(new JavascriptResponse
                {
                    Success = false,
                    Message = string.Format("Browser with Id {0} not found in Render Sub Process.", browserId)
                });
            }

            var task = factory.StartNew(() =>
            {
                try
                {
                    var response = browser.DoCallback(callbackId, parameters);

                    return response;
                }
                catch (Exception ex)
                {
                    return new JavascriptResponse
                    {
                        Success = false,
                        Message = ex.ToString()
                    };
                }
            }, TaskCreationOptions.AttachedToParent);

            return timeout.HasValue ? task.WithTimeout(timeout.Value) : task;
        }      
    }
}
