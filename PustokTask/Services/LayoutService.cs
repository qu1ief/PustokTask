

using PustokTask.Data;
using PustokTask.Models;

namespace PPustokTask.Services
{
    public class LayoutService(PustokDbContex _pustokDbContext)
    {
        public List<Setting> GetSettings()
        {
            return _pustokDbContext.Settings.ToList();
        }

        public List<Genre> GetGenres()
        {
            return _pustokDbContext.Genres.ToList();
        }
    }
}
