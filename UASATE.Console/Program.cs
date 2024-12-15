using Kruskal.Core;
using UASATE.Core;
using UASATE.Core.Encoding;

internal class Program
{

    private static void Main(string[] args)
    {
        UjiPadaGraph();
    }

    private static void UjiPadaGraph()
    {
        var graph = new Graph<string>(
            ["A", "B", "C", "D", "E"],
            [
                ("A", "B", 60d),
                ("A", "C", 100d),
                ("A", "D", 50d),
                ("A", "E", 90d),
                ("B", "C", 70d),
                ("B", "D", 40d),
                ("B", "E", 30d),
                ("C", "D", 65d),
                ("C", "E", 55d),
                ("D", "E", 110d),
            ]
        );

        var maksGenerasi = 1000;
        var jumlahPopulasi = 20;

        var random = new Random();
        var encoding = new OrdinalEncoding(graph.Vertices.Count);

        var fungsiObjektif = (Vector v) =>
        {
            var cost = 0d;

            for (var i = 0; i < v.Dimension; i++)
            {
                if (i < v.Dimension - 1)
                {
                    var v1 = graph.Vertices[(int)v[i]];
                    var v2 = graph.Vertices[(int)v[i + 1]];

                    cost += graph.EdgeCost(v1, v2);
                }
                else
                {
                    var v1 = graph.Vertices[(int)v[i]];
                    var v2 = graph.Vertices[(int)v[0]];

                    cost += graph.EdgeCost(v1, v2);
                }
            }

            return cost;
        };

        var populasiAwal = encoding.GeneratePopulasi(jumlahPopulasi);

        Console.WriteLine($"Jumlah Gen : {graph.Vertices.Count}");
        Console.WriteLine($"Jumlah Populasi: {jumlahPopulasi}");

        Console.WriteLine($"Populasi Awal : \n{string.Join("\n", populasiAwal.Select(v => encoding.Decode(v)))}");

        var result = DifferentialEvolution.Run(
            jumlahGen: graph.Vertices.Count,
            jumlahPopulasi: jumlahPopulasi,
            jenisOptimasi: JenisOptimasi.Min,
            skemaMutasi: SkemaMutasi.Best,
            jenisCrossover: JenisCrossover.Binomial,
            fungsiObjektif: fungsiObjektif,
            populasiAwal: populasiAwal,
            encoding: encoding,
            maksGenerasi: maksGenerasi);

        Console.WriteLine("\nHasil :");
        Console.WriteLine("Global Best : ");
        for (int i = 0; i < result.GlobalBest.Dimension; i++)
            Console.Write($"{graph.Vertices[(int)encoding.Decode(result.GlobalBest)[i]].Value} ");
        Console.WriteLine();

        Console.WriteLine($"Global Best Fitness : {fungsiObjektif(encoding.Decode(result.GlobalBest)):F6}");
        Console.WriteLine($"Jumlah Generasi : {result.JumlahGenerasi}");
        Console.WriteLine($"Local Best : \n{string.Join("\n", result.LocalBests.Select(v => fungsiObjektif(encoding.Decode(v))))}");
    }

    private static void UjiPadaFungsiBenchmark()
    {
        var fungsiObjektif = (Vector v) =>
        {
            var result = 0d;
            for (int i = 0; i < v.Dimension; i++)
                result += Math.Abs(v[i] * Math.Sin(v[i]) + 0.1 * v[i]);
            return result;
        };

        var jumlahGen = 4;
        var jumlahPopulasi = 20;

        var random = new Random();
        var populasiAwal = new List<Vector>(jumlahPopulasi);
        for (int i = 0; i < jumlahPopulasi; i++)
        {
            var v = new Vector(jumlahGen);
            for (int j = 0; j < jumlahGen; j++)
                v[j] = random.NextDouble(-35, 35);
            populasiAwal.Add(v);
        }

        Console.WriteLine($"Jumlah Gen : {jumlahGen}");
        Console.WriteLine($"Jumlah Populasi: {jumlahPopulasi}");

        Console.WriteLine($"Populasi Awal : \n{string.Join("\n", populasiAwal)}");

        var result = DifferentialEvolution.Run(
            jumlahGen,
            jumlahPopulasi,
            JenisOptimasi.Min,
            SkemaMutasi.Rand,
            JenisCrossover.Binomial,
            fungsiObjektif,
            populasiAwal,
            maksGenerasi: 2000);

        Console.WriteLine("\nHasil :");
        Console.WriteLine($"Global Best : {result.GlobalBest}");
        Console.WriteLine($"Global Best Fitness : {fungsiObjektif(result.GlobalBest):F6}");
        Console.WriteLine($"Jumlah Generasi : {result.JumlahGenerasi}");
        Console.WriteLine($"Local Best : \n{string.Join("\n", result.LocalBests.Select(v => fungsiObjektif(v)))}");
    }
}