using DocSasCorp.Mail.Contract;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DocSasCorp.Mail.Office365Outlook.TokenCacheModel
{
    public class MicrosoftTokenStore
    {
        private ReaderWriterLockSlim SessionLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private static string CacheId { get; set; }
        private readonly IMailDataStore _dataStore;
        private readonly TokenCache _cache = new TokenCache();
        private readonly DateTimeOffset _expireDate;
        private readonly bool _isLocalEnv;

        public MicrosoftTokenStore(IMailDataStore dataStore, DateTimeOffset expireDate, bool isLocalEnv = false)
        {
            CacheId = dataStore.StoreKey + "_TokenCacheMail";
            _dataStore = dataStore;
            _expireDate = expireDate;
          
            Load();
        }

        public TokenCache GetMsalCacheInstance()
        {
            _cache.SetBeforeAccess(BeforeAccessNotification);
            _cache.SetAfterAccess(AfterAccessNotification);
            Load();
            return _cache;
        }

        public void SaveUserStateValue(string state)
        {
            SessionLock.EnterWriteLock();
            if (_dataStore != null) _dataStore.Add($"{CacheId}_state", state, _expireDate);

            SessionLock.ExitWriteLock();
        }

        public string ReadUserStateValue()
        {
            SessionLock.TryEnterReadLock(5000);

            var state = _dataStore.Get($"{CacheId}_state") as string;

            SessionLock.ExitReadLock();
            return state;
        }

        public void Load()
        {
            SessionLock.TryEnterReadLock(5000);
            var value =  _dataStore.Get($"{CacheId}") as byte[];
            _cache.Deserialize(value);
            SessionLock.ExitReadLock();
        }

        public void Persist()
        {
            SessionLock.EnterWriteLock();

            // Optimistically set HasStateChanged to false. We need to do it early to avoid losing changes made by a concurrent thread.
            _cache.HasStateChanged = false;

            // Reflect changes in the persistent store
            if (_dataStore != null) _dataStore.Add($"{CacheId}", _cache.Serialize(), _expireDate);


            SessionLock.ExitWriteLock();
        }

        // Triggered right before MSAL needs to access the cache.
        // Reload the cache from the persistent store in case it changed since the last access.
        private void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            Load();
        }

        // Triggered right after MSAL accessed the cache.
        private void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            // if the access operation resulted in a cache update
            if (_cache.HasStateChanged)
            {
                Persist();
            }
        }
    }
}
