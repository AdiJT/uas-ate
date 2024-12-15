using UASATE.Core;

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
for(int i = 0; i < jumlahPopulasi; i++)
{
    var v = new Vector(jumlahGen);
    for(int j = 0; j < jumlahGen; j++)
        v[j] = random.NextDouble(-35, 35);
    populasiAwal.Add(v);
}

Console.WriteLine($"Jumlah Gen : {jumlahGen}");
Console.WriteLine($"Jumlah Populasi: {jumlahPopulasi}");

Console.WriteLine($"Populasi Awal : {string.Join("\n", populasiAwal)}");

var result = DifferentialEvolution.Run(
    jumlahGen,
    jumlahPopulasi,
    JenisOptimasi.Min,
    SkemaMutasi.Rand,
    JenisCrossover.Binomial,
    fungsiObjektif,
    populasiAwal);

Console.WriteLine("\nHasil :");
Console.WriteLine($"Global Best : {result.GlobalBest}");
Console.WriteLine($"Global Best Fitness : {fungsiObjektif(result.GlobalBest):F6}");
Console.WriteLine($"Jumlah Generasi : {result.JumlahGenerasi}");