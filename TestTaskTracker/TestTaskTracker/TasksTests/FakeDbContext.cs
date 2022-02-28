using Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;

namespace TasksTests
{
    public static class FakeDbContext
    {
        public static Func<TasksContext> Get(string dbName)
        {
            var options = new DbContextOptionsBuilder<TasksContext>()
                          .UseInMemoryDatabase(dbName, new InMemoryDatabaseRoot())
                          .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                          .ConfigureWarnings(w => w.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning))
                          .Options;

            return () => new TasksContext(options);
        }
    }
}
