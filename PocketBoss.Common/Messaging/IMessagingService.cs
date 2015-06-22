using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketBoss.Common.Messaging
{
    public interface IMessagingService: IDisposable
    {
        void OpenConnection(IDictionary<string, string> connectionDetails);
        void CloseConnection();
        void AddSubscription<T>();
        void RemoveSubscription<T>();
        void Send<T>(T message);
        void Send<T>(string endpoint, T message);
        Task<R> Send<T, R>(T message) where R : class, new();
        Task<R> Send<T, R>(string endpoint, T message) where R : class, new();
        void Reply<T>(T message, int statusCode = 1);
        void Publish<T>(T message);
    }
}
