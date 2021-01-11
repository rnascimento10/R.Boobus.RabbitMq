using R.BooBus.Core;

namespace Subscriber
{
    public class PublishVideoEvent : Event
    {
        public PublishVideoEvent()
        {
        }

        public string Message { get; set; }
    }

}
