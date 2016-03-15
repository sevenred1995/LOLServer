using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOLServer.cache {
    public class CacheFactory {
        public readonly static IAccountCache accountCache;
        public readonly static IUserCache userCache;
        static CacheFactory() {
            accountCache=Activator.CreateInstance(Type.GetType("LOLServer.cache.impl.AccountCache")) as IAccountCache;
            userCache = Activator.CreateInstance(Type.GetType("LOLServer.cache.impl.UserCache")) as IUserCache;
            //accountCache =new AccountCache();
            //userCache = new UserCache();
        }
    }
}
