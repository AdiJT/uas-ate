using UASATE.Core.Abstracts;

namespace UASATE.Core.Encoding;

public class OrdinalEncoding : IEncoding
{
    public int JumlahItem { get; }

    public OrdinalEncoding(int jumlahItem)
    {
        JumlahItem = jumlahItem;
    }

    public Vector Decode(Vector vector)
    {
        var result = new Vector(JumlahItem);
        var daftarItem = Enumerable.Range(0, JumlahItem).ToList();

        for(int i = 0; i < vector.Dimension; i++)
        {
            result[i] = daftarItem[(int)vector[i]];
            daftarItem.RemoveAt((int)vector[i]);
        }

        return result;
    }

    public Vector Encode(Vector vector)
    {
        var result = new Vector(JumlahItem);
        var daftarItem = Enumerable.Range(0, JumlahItem).ToList();

        for(int i = 0; i < JumlahItem; i++)
        {
            result[i] = daftarItem.IndexOf((int)vector[i]);
            daftarItem.Remove((int)vector[i]);
        }

        return result;
    }

    public List<Vector> GeneratePopulasi(int jumlahPopulasi)
    {
        var populasi = new List<Vector>(jumlahPopulasi);
        var daftarItem = Enumerable.Range(0, JumlahItem).Select(i => (double)i).ToArray();

        var random = new Random();
        for (int i = 0; i < jumlahPopulasi; i++)
        {
            random.Shuffle(daftarItem);
            var v = new Vector(JumlahItem, daftarItem);

            populasi.Add(Encode(v));
        }

        return populasi;
    }

    public Vector Constraint(Vector vector)
    {
        var result = new Vector(JumlahItem);

        for(int i = 0; i < JumlahItem; i++)
        {
            result[i] =  Math.Clamp(Math.Round(vector[i]), 0, JumlahItem - i - 1);
        }

        return result;
    }
}
