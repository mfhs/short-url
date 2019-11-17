using System;
using System.Collections.Generic;
using System.Linq;
using ShortUrl.Repository.Entities;
using ShortUrl.Repository.Interfaces;

namespace ShortUrl.Repository.Implementations
{
    public class UrlInfoRepository : IUrlInfoRepository
    {
        private readonly AppDbContext _context;
        private readonly object _lock = new object();

        public UrlInfoRepository(AppDbContext context)
        {
            _context = context;
        }

        public UrlInfoEntity GetUrlInfoByShortCode(string shortCode)
        {
            using (_context)
            {
                try
                {
                    var urlInfo = _context.UrlInfo.FirstOrDefault(u => u.ShortCode == shortCode);
                    if (urlInfo == null || urlInfo.UrlHits <= 0) return urlInfo;
                    UpdateUrlHits(urlInfo);
                    return urlInfo;
                }
                catch (Exception exception)
                {
                    throw new Exception("Exception occured while reading url info by short code! Message: " +
                                        exception.Message);
                }
            }
        }

        public UrlInfoEntity GetUrlInfoByOriginalUrl(string originalUrl)
        {
            using (_context)
            {
                try
                {
                    var urlInfo = _context.UrlInfo.FirstOrDefault(u => u.OriginalUrl == originalUrl);
                    if (urlInfo == null || urlInfo.UrlHits <= 0) return urlInfo;
                    UpdateUrlHits(urlInfo);
                    return urlInfo;
                }
                catch (Exception exception)
                {
                    throw new Exception("Exception occured while reading url info by original url! Message: " +
                                        exception.Message);
                }
            }
        }

        private void UpdateUrlHits(UrlInfoEntity urlInfo)
        {
            lock (_lock)
            {
                urlInfo.UrlHits = urlInfo.UrlHits + 1;
                Update(urlInfo);
            }
        }

        //public IEnumerable<UrlInfoEntity> GetAll(int pageNumber, int pageSize)
        //{
        //    using (_context)
        //    {
        //        try
        //        {
        //            var entities = _context.Set<UrlInfoEntity>().Skip((pageNumber - 1) * pageSize).Take(pageSize);
        //            return entities;
        //        }
        //        catch (Exception exception)
        //        {
        //            throw new Exception("Exception occured while reading list of url info! Message: " +
        //                                exception.Message);
        //        }
        //    }
        //}

        public bool Insert(UrlInfoEntity urlInfo)
        {
            using (_context)
            {
                try
                {
                    _context.Add(urlInfo);
                    var numberOfRowsAffected = _context.SaveChanges();
                    return numberOfRowsAffected > 0;
                }
                catch (Exception exception)
                {
                    throw new Exception("Exception occured while inserting url info! Message: " +
                                        exception.Message);
                }
            }
        }

        public bool Update(UrlInfoEntity urlInfo)
        {
            using (_context)
            {
                try
                {
                    _context.Update(urlInfo);
                    var numberOfRowsAffected = _context.SaveChangesAsync();
                    return numberOfRowsAffected.Result > 0;
                }
                catch (Exception exception)
                {
                    throw new Exception("Exception occured while updating url info! Message: " +
                                        exception.Message);
                }
            }
        }       

        //public bool Delete(long id)
        //{
        //    using (_context)
        //    {
        //        try
        //        {
        //            var urlInfo = _context.UrlInfo.Find(id);
        //            _context.Remove(urlInfo);
        //            var numberOfRowsAffected = _context.SaveChanges();
        //            return numberOfRowsAffected > 0;
        //        }
        //        catch (Exception exception)
        //        {
        //            throw new Exception("Exception occured while deleting url info! Message: " +
        //                                exception.Message);
        //        }
        //    }
        //}
    }
}
