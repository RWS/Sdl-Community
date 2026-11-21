using System;
using System.ComponentModel;
using System.Net.Http;

namespace Sdl.Community.IATETerminologyProvider.Interface
{
    public interface IConnectionProvider : INotifyPropertyChanged, IDisposable
    {
        HttpClient HttpClient { get; }

        bool AccessTokenExpired { get; }

        bool RefreshTokenExpired { get; }

        string AccessToken { get; }

        string RefreshToken { get; }

        DateTime? ExpireDate { get; }

        bool Login(string userName, string password);

        bool ReLogin();

        bool ExtendLogin();

        bool EnsureConnection();
    }
}
