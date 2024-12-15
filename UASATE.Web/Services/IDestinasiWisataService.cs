using Kruskal.Core;
using UASATE.Web.Models;

namespace UASATE.Web.Services;

public interface IDestinasiWisataService
{
    List<DestinasiWisata> GetAll();
    DestinasiWisata? Get(string nama);
    Graph<DestinasiWisata> GetGraph();
    void SetDistance(DestinasiWisata destinasiWisata1, DestinasiWisata destinasiWisata2, double distance);
}