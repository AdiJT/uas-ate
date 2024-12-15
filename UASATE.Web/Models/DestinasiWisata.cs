using System.Drawing;
using System.Numerics;

namespace UASATE.Web.Models;

public class DestinasiWisata : IEquatable<DestinasiWisata>
{
    public string Nama { get; set; } = string.Empty;
    public string PlaceId { get; set; } = string.Empty;
    public Vector2 TitikKoordinaat { get; set; }

    public override int GetHashCode() => Nama.GetHashCode();

    public override bool Equals(object? obj) => obj is not null && obj is DestinasiWisata other && other.Nama == Nama;

    public bool Equals(DestinasiWisata? other) => other is not null && other.Nama == Nama;
}