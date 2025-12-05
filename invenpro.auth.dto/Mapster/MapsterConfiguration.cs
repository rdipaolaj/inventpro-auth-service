using Mapster;

namespace invenpro.auth.dto.Mapster;

public static class MapsterConfiguration
{
    public static TypeAdapterConfig Configuration()
    {
        TypeAdapterConfig config = new();


        return config;
    }
}