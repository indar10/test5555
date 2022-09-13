using System.Threading.Tasks;

namespace Infogroup.IDMS.Net.Sms
{
    public interface ISmsSender
    {
        Task SendAsync(string number, string message);
    }
}