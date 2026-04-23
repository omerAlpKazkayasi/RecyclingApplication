using System.Security.Cryptography;

namespace RecyclingApp.SharedKernel.Domain;

/// <summary>
/// UUID v7 generator. Produces time-ordered UUIDs per RFC 9562.
/// Time-ordered IDs are B-tree friendly and provide natural chronological ordering.
/// </summary>
public static class UuidV7
{
    /// <summary>
    /// Creates a new UUID v7 with millisecond-precision timestamp and random payload.
    /// </summary>
    public static Guid Create()
    {
        return Guid.CreateVersion7();
    }
}
