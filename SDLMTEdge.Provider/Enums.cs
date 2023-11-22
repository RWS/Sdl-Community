namespace Sdl.Community.MTEdge.Provider
{
    public enum APIVersion
    {
        Unknown,
        v1,
        v2
    }

    public enum ErrorHResult
    {
        HandshakeFailure = -2146232800,
        ServerInaccessible = -2147467259,
        RequestTimeout = -2146233029,
    }

    public enum Parameters
    {
        Inverted
    }
}