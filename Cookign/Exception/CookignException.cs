using Cookign.Message;
using Cookign.Extension;
namespace Cookign.Exception
{
    public class CookignException : System.Exception
    {
        public CookignException(ErrorMessagesEnum message) : base (message.GetDescription())
        {

        }

        public CookignException(ErrorMessagesEnum message, System.Exception innerException) : base(message.GetDescription(), innerException)
        {

        }
    }
}
