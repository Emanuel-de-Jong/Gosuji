using GosujiServer.Controllers;
using GosujiServer.Data;

namespace GosujiServer.Services
{
    public class KataGoService
    {
        private KataGoVersion? version;

        public Task<KataGo> GetKataGoAsync()
        {
            return Task.FromResult(new KataGo());
        }

        public KataGoVersion GetVersion()
        {
            if (version == null)
            {
                ApplicationDbContext context = new DbService().GetContext();

                version = context.KataGoVersions
                    .Where(k => k.Model == KataGoVersion.MODEL)
                    .Where(k => k.Version == KataGoVersion.VERSION)
                    .Where(k => k.Config == KataGoVersion.GetConfig())
                    .FirstOrDefault();

                if (version == null)
                {
                    version = new KataGoVersion();
                    context.Add(version);
                    context.SaveChanges();
                }

                context.Dispose();
            }

            return version;
        }
    }
}
