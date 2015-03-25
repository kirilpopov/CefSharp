// Copyright © 2010-2013 The CefSharp Project. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

#pragma once


#include "Stdafx.h"
#include "include/cef_app.h"
#include "include/cef_base.h"

#include "CefBrowserWrapper.h"
using namespace System;
using namespace System::Collections::Generic;

namespace CefSharp
{
    private class CefAppUnmanagedWrapper : CefApp, CefRenderProcessHandler
    {
    private:
        gcroot<Action<CefBrowserWrapper^>^> _onBrowserCreated;
        gcroot<Action<CefBrowserWrapper^>^> _onBrowserDestroyed;
        gcroot<Action<CefBrowserWrapper^, String^, String^>^> _onUncaughtException;
        gcroot<Dictionary<int, CefBrowserWrapper^>^> _browserWrappers;
        CefBrowserWrapper^ FindBrowserWrapper(CefRefPtr<CefBrowser> browser, bool mustExist);
    public:
        
        CefAppUnmanagedWrapper(Action<CefBrowserWrapper^>^ onBrowserCreated, 
          Action<CefBrowserWrapper^>^ onBrowserDestoryed,
          Action<CefBrowserWrapper^, String^, String^>^ onUncaughtException)
        {
            _onBrowserCreated = onBrowserCreated;
            _onBrowserDestroyed = onBrowserDestoryed;
            _onUncaughtException = onUncaughtException;

            _browserWrappers = gcnew Dictionary<int, CefBrowserWrapper^>();
        }

        ~CefAppUnmanagedWrapper()
        {
            delete _browserWrappers;
            delete _onBrowserCreated;
            delete _onBrowserDestroyed;
        }

        virtual DECL CefRefPtr<CefRenderProcessHandler> GetRenderProcessHandler() OVERRIDE;
        virtual DECL void OnBrowserCreated(CefRefPtr<CefBrowser> browser) OVERRIDE;
        virtual DECL void OnBrowserDestroyed(CefRefPtr<CefBrowser> browser) OVERRIDE;
        virtual DECL void OnContextCreated(CefRefPtr<CefBrowser> browser, CefRefPtr<CefFrame> frame, CefRefPtr<CefV8Context> context) OVERRIDE;
        virtual DECL void OnContextReleased(CefRefPtr<CefBrowser> browser, CefRefPtr<CefFrame> frame, CefRefPtr<CefV8Context> context) OVERRIDE;

        virtual DECL void OnUncaughtException(CefRefPtr<CefBrowser> browser,
          CefRefPtr<CefFrame> frame,
          CefRefPtr<CefV8Context> context,
          CefRefPtr<CefV8Exception> exception,
          CefRefPtr<CefV8StackTrace> stackTrace) OVERRIDE;

        IMPLEMENT_REFCOUNTING(CefAppUnmanagedWrapper);
    };
}
