using System.Collections.Generic;
using ShortUrl.Repository.Entities;

namespace ShortUrl.Repository.Interfaces
{
    public interface IUrlInfoRepository
    {
        bool Insert(UrlInfoEntity urlInfo);
        bool Update(UrlInfoEntity urlInfo);
        UrlInfoEntity GetUrlInfoByShortCode(string shortCode);
        UrlInfoEntity GetUrlInfoByOriginalUrl(string originalUrl);
        IEnumerable<UrlInfoEntity> GetAll(int pageNumber, int pageSize);
        bool Delete(long id);
    }
}
