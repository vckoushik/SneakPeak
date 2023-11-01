using SneakPeak.Models;

namespace SneakPeak.Services
{
    public interface IMailService
    {
        public bool SendEmail(Message message);
    }
}
