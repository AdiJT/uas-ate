using Kruskal.Core;
using System.Numerics;
using UASATE.Web.Models;

namespace UASATE.Web.Services;

public class DestinasiWisataService : IDestinasiWisataService
{
    private readonly List<DestinasiWisata> _daftarDestinasiWisata = 
    [
        new DestinasiWisata { Nama = "Lasiana Beach", PlaceId = "ChIJaYd2jqyDViwRVEgG40jMVEE", TitikKoordinaat = new Vector2(-10.14223301028807f, 123.67063269361208f)},
        new DestinasiWisata { Nama = "Pantai Pasir Panjang Kupang", PlaceId = "ChIJp1ZpHNKcViwRmZqHeYuHwOg", TitikKoordinaat = new Vector2(-10.149979190735445f, 123.60518947564036f)},
        new DestinasiWisata { Nama = "Nostalgia Park", PlaceId = "ChIJibLTWWeDViwRFQb7CH8AxVw", TitikKoordinaat = new Vector2(-10.159086795623251f, 123.61595105241574f)},
        new DestinasiWisata { Nama = "Bukit Cinta", PlaceId = "ChIJN3KBbp2DViwRDrbh7b7suto", TitikKoordinaat = new Vector2(-10.162322342701815f, 123.66508988918743f)},
        new DestinasiWisata { Nama = "Monkeys Cave and Natural Park", PlaceId = "ChIJCeVwbwGcViwROIY-TtyF5p4", TitikKoordinaat = new Vector2(-10.181181041158709f, 123.53240726085126f)},
        new DestinasiWisata { Nama = "Subasuka Water Park", PlaceId = "ChIJgUQ188-cViwRcBQOK-3NEhk", TitikKoordinaat = new Vector2(-10.14890301459608f, 123.6075340984483f)},
        new DestinasiWisata { Nama = "Goa Kristal Bolok", PlaceId = "ChIJg7zq0XWZViwRQ8CwEXzHRhY", TitikKoordinaat = new Vector2(-10.2244290668636f, 123.51099725505591f)},
        new DestinasiWisata { Nama = "Alun-Alun Kota Kupang", PlaceId = "ChIJU6LzYdudViwRMcSNJQVBBEA", TitikKoordinaat = new Vector2(-10.148521427548744f, 123.61100754788913f)},
        new DestinasiWisata { Nama = "Trans Studio Mini Kupang", PlaceId = "ChIJ5Xs4i5-dViwRx3tYRT1zbS4", TitikKoordinaat = new Vector2(-10.16976084748916f, 123.60958767939964f)},
        new DestinasiWisata { Nama = "Namosain Beach", PlaceId = "ChIJjTeABnCcViwRn0iRrwPVRDs", TitikKoordinaat = new Vector2(-10.172243205592205f, 123.56245605573605f)},
        new DestinasiWisata { Nama = "Kelapa Lima Beach", PlaceId = "ChIJH5SnW9KcViwRgDsDoxZKMWM", TitikKoordinaat = new Vector2(-10.144309917726684f, 123.61737584077605f)},
        new DestinasiWisata { Nama = "Air Terjun Oenesu", PlaceId = "ChIJC5SSs1GRViwRFFc7CPryaVg", TitikKoordinaat = new Vector2(-10.265882847823844f, 123.56665756405691f)},
        new DestinasiWisata { Nama = "Harper Kupang", PlaceId = "ChIJ61F7R-qdViwRUlfE2zDRpyc", TitikKoordinaat = new Vector2(-10.168839860369372f, 123.61179114262872f)},
    ];

    private readonly Graph<DestinasiWisata> _graph;

    public DestinasiWisataService()
    {
        _graph = new(_daftarDestinasiWisata, _daftarDestinasiWisata.SelectMany(d => _daftarDestinasiWisata.Where(x => )))
    }

    public DestinasiWisata Get(string nama) => _daftarDestinasiWisata.First(d => d.Nama == nama);

    public List<DestinasiWisata> GetAll() => _daftarDestinasiWisata.ToList();\
}
