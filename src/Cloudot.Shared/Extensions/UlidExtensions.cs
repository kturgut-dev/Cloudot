namespace Cloudot.Shared.Extensions;

// public static class UlidExtensions
// {
//     // GUID'i ULID'e dönüştürme
//     public static Ulid ToUlid(this Guid guid)
//     {
//         byte[] guidBytes = guid.ToByteArray();
//         return new Ulid(guidBytes);
//     }
//     
//     // ULID'i GUID'e dönüştürme
//     public static Guid ToGuid(this Ulid ulid)
//     {
//         byte[] ulidBytes = ulid.ToByteArray();
//         return new Guid(ulidBytes);
//     }
//     
//     // String'den ULID oluşturma
//     public static Ulid ParseUlid(string value)
//     {
//         return Ulid.Parse(value);
//     }
// }