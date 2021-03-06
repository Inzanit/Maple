﻿using System;

namespace Maple.Domain
{
    /// <summary>
    /// forwards user friendly messages to the UI and to a <see cref="ILoggingService"/>
    /// </summary>
    /// <autogeneratedoc />
    public interface ILoggingNotifcationService
    {
        void Info(object message);
        void Warn(object message);
        void Error(object message, Exception exception);
        void Fatal(object message, Exception exception);
    }
}