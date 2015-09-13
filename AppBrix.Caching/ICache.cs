// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AppBrix.Caching
{
    public interface ICache
    {
        void Connect();
        Task ConnectAsync();

        void Refresh(string key);
        Task RefreshAsync(string key);

        T Get<T>(string key);
        Task<T> GetAsync<T>(string key);

        void Set<T>(string key, T item);
        Task SetAsync<T>(string key, T item);

        void Remove(string key);
        Task RemoveAsync(string key);
    }
}
