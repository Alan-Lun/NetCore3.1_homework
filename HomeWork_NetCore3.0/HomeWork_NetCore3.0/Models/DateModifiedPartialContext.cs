using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace HomeWork_NetCore3._0.Models
{
    public partial class ContosouniversityContext
    {
        public override int SaveChanges()
        {
            AddDateModified();

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            AddDateModified();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void AddDateModified()
        {
            var entries = this.ChangeTracker.Entries();
            foreach (var entityEntry in entries)
            {
                if (entityEntry.State == EntityState.Modified)
                {
                    entityEntry.CurrentValues.SetValues(new {DateModified = DateTime.UtcNow});
                }
            }
        }
    }
}
