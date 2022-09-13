using System.Globalization;

namespace Infogroup.IDMS.Localization
{
    public interface IApplicationCulturesProvider
    {
        CultureInfo[] GetAllCultures();
    }
}