using System;
using System.Collections.Generic;
using System.Text;

namespace ShortUrl.Repository.Interfaces
{
    //TODO: if more entities/business are present, better to use BaseRepo, like analytic
    public interface IBaseRepository<TModel> : IDisposable
    {
        TModel Insert(TModel model);
        TModel Update(TModel model);
        bool Delete(long id);
        TModel Find(long id);
        IEnumerable<TModel> GetAll(int pageNumber, int pageSize);
    }
}
