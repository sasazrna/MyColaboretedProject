﻿using System;
using Java.Lang;
using Xamarin.Facebook;
using Android.Runtime;

namespace PrigovorHR.Droid
{
    public class FacebookCallback<TResult> : Java.Lang.Object, IFacebookCallback where TResult : Java.Lang.Object
    {
        public Action HandleCancel { get; set; }
        public Action<FacebookException> HandleError { get; set; }
        public Action<TResult> HandleSuccess { get; set; }

        public void OnCancel()
        {
            HandleCancel?.Invoke();
        }

        public void OnError(FacebookException error)
        {
            HandleError?.Invoke(error);
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            HandleSuccess?.Invoke(result.JavaCast<TResult>());
        }
    }
}
