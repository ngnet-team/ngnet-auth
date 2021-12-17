using Common.Json.Service;
using Database;
using Database.Models.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Services
{
    public abstract class BaseService
    {
        protected readonly NgnetAuthDbContext database;
        protected readonly JsonService jsonService;

        protected BaseService(NgnetAuthDbContext database, JsonService jsonService)
        {
            this.database = database;
            this.jsonService = jsonService;
        }

        protected HashSet<IBaseModel> ExcludeDeleted(HashSet<IBaseModel> collection)
        {
            return collection.Where(x => !x.IsDeleted).ToHashSet();
        }
    }
}
