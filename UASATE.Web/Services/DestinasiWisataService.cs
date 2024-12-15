using Kruskal.Core;
using System.Drawing;
using System.Numerics;
using UASATE.Web.Models;

namespace UASATE.Web.Services;

public class DestinasiWisataService : IDestinasiWisataService
{
    private readonly List<DestinasiWisata> _daftarDestinasiWisata = 
    [
        new DestinasiWisata { Nama = "Lasiana Beach", PlaceId = "ChIJaYd2jqyDViwRVEgG40jMVEE", TitikKoordinat = new PointF(-10.14223301028807f, 123.67063269361208f)},
        new DestinasiWisata { Nama = "Pantai Pasir Panjang Kupang", PlaceId = "ChIJp1ZpHNKcViwRmZqHeYuHwOg", TitikKoordinat = new PointF(-10.149979190735445f, 123.60518947564036f)},
        new DestinasiWisata { Nama = "Nostalgia Park", PlaceId = "ChIJibLTWWeDViwRFQb7CH8AxVw", TitikKoordinat = new PointF(-10.159086795623251f, 123.61595105241574f)},
        new DestinasiWisata { Nama = "Bukit Cinta", PlaceId = "ChIJN3KBbp2DViwRDrbh7b7suto", TitikKoordinat = new PointF(-10.162322342701815f, 123.66508988918743f)},
        new DestinasiWisata { Nama = "Monkeys Cave and Natural Park", PlaceId = "ChIJCeVwbwGcViwROIY-TtyF5p4", TitikKoordinat = new PointF(-10.181181041158709f, 123.53240726085126f)},
        new DestinasiWisata { Nama = "Subasuka Water Park", PlaceId = "ChIJgUQ188-cViwRcBQOK-3NEhk", TitikKoordinat = new PointF(-10.14890301459608f, 123.6075340984483f)},
        new DestinasiWisata { Nama = "Goa Kristal Bolok", PlaceId = "ChIJg7zq0XWZViwRQ8CwEXzHRhY", TitikKoordinat = new PointF(-10.2244290668636f, 123.51099725505591f)},
        new DestinasiWisata { Nama = "Alun-Alun Kota Kupang", PlaceId = "ChIJU6LzYdudViwRMcSNJQVBBEA", TitikKoordinat = new PointF(-10.148521427548744f, 123.61100754788913f)},
        new DestinasiWisata { Nama = "Trans Studio Mini Kupang", PlaceId = "ChIJ5Xs4i5-dViwRx3tYRT1zbS4", TitikKoordinat = new PointF(-10.16976084748916f, 123.60958767939964f)},
        new DestinasiWisata { Nama = "Namosain Beach", PlaceId = "ChIJjTeABnCcViwRn0iRrwPVRDs", TitikKoordinat = new PointF(-10.172243205592205f, 123.56245605573605f)},
        new DestinasiWisata { Nama = "Kelapa Lima Beach", PlaceId = "ChIJH5SnW9KcViwRgDsDoxZKMWM", TitikKoordinat = new PointF(-10.144309917726684f, 123.61737584077605f)},
        new DestinasiWisata { Nama = "Air Terjun Oenesu", PlaceId = "ChIJC5SSs1GRViwRFFc7CPryaVg", TitikKoordinat = new PointF(-10.265882847823844f, 123.56665756405691f)},
        new DestinasiWisata { Nama = "Harper Kupang", PlaceId = "ChIJ61F7R-qdViwRUlfE2zDRpyc", TitikKoordinat = new PointF(-10.168839860369372f, 123.61179114262872f)},
    ];

    private readonly Graph<DestinasiWisata> _graph;

    public DestinasiWisataService()
    {
        _graph = new(_daftarDestinasiWisata, _daftarDestinasiWisata.SelectMany(d => _daftarDestinasiWisata.Where(x => x != d).Select(x => (d, x, 0d))).ToList());
    }

    public DestinasiWisata? Get(string nama) => _daftarDestinasiWisata.FirstOrDefault(d => d.Nama == nama);

    public List<DestinasiWisata> GetAll() => _daftarDestinasiWisata.ToList();

    public Graph<DestinasiWisata> GetGraph() => _graph;

    public void SetDistance(DestinasiWisata destinasiWisata1, DestinasiWisata destinasiWisata2, double distance)
    {
        _graph.SetEdgeCost(new Vertex<DestinasiWisata>(destinasiWisata1), new Vertex<DestinasiWisata>(destinasiWisata2), distance);
    }
}
