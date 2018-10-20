using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DocSasCorp.Core.WebAction
{
    /// <summary>
    /// HttpRequestAction
    /// </summary>
    public class HttpRequestAction
    {
        /// <summary>
        /// GetAsync
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<string> GetAsync(string url)
        {
            string result = string.Empty;
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    using (HttpContent content = response.Content)
                    {
                        string data = await content.ReadAsStringAsync();

                        if (!string.IsNullOrWhiteSpace(data))
                        {
                            result = data;
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// PostAsync
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public static async Task<string> PostAsync(string url, string postData)
        {
            string result = string.Empty;
            HttpContent cont = new StringContent(postData);
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.PostAsync(url, cont))
                {
                    using (HttpContent content = response.Content)
                    {
                        string data = await content.ReadAsStringAsync();

                        if (!string.IsNullOrWhiteSpace(data))
                        {
                            result = data;
                        }
                    }
                }
            }

            return result;
        }
    }
}
