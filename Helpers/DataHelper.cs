using NeoContact.Data;
using Microsoft.EntityFrameworkCore;

//ADD #56 Data Helper
namespace NeoContact.Helpers
{
    public static class DataHelper
    {
        public static async Task ManageDataAsync(IServiceProvider svcProvider)
        {
            // get instance of the db app context
            var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();
            
            //migration: equal to update-database
            await dbContextSvc.Database.MigrateAsync();
        }
    }
}
