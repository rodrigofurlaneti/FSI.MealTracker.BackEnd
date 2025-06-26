using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.MealTracker.Infrastructure.Context
{
    public interface IDbContext
    {
        IDbConnection CreateConnection();
    }
}
