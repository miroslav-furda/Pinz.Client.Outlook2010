using Prism.Events;
using System;

namespace Com.Pinz.Client.Commons.Event
{
    public class TimeoutErrorEvent : PubSubEvent<Exception>
    {
    }
}
