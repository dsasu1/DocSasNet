using System;

namespace DocSasCorp.Mail.Contract
{
    /// <summary>
    /// IMailDataStore
    /// </summary>
    public interface IMailDataStore
    {
        /// <summary>
        /// StoreKey
        /// </summary>
        string StoreKey { get; set; }
        /// <summary>
        /// Add
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireDate"></param>
        /// <returns></returns>
        bool Add(string key ,object value, DateTimeOffset? expireDate = null);
        /// <summary>
        /// Get
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object Get(string key);
    }
}