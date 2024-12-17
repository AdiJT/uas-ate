using UASATE.Core.Abstracts;

namespace UASATE.Core;

public static class DifferentialEvolution
{
    public static DifferentialEvolutionResult Run(
        int jumlahGen,
        int jumlahPopulasi,
        JenisOptimasi jenisOptimasi,
        SkemaMutasi skemaMutasi,
        JenisCrossover jenisCrossover,
        Func<Vector, double> fungsiObjektif,
        List<Vector> populasiAwal,
        int maksGenerasi,
        IEncoding? encoding = null,
        Func<Vector, Vector>? contraint = null,
        int pasanganMutasi = 1,
        double minDeltaGlobalBestFitness = 0.001,
        double differentialWeight = 0.7,
        double crossoverRate = 0.7,
        int patience = 15,
        bool verbose = false,
        bool elitism = false)
    {
        //Iniliasasi
        var random = new Random();

        if (jumlahPopulasi < pasanganMutasi * 2 + 2)
            throw new ArgumentException("Tidak cukup", nameof(jumlahPopulasi));

        if (populasiAwal.Count != jumlahPopulasi)
            throw new ArgumentException($"Panjang tidak sama dengan {nameof(jumlahPopulasi)}", nameof(populasiAwal));

        if (differentialWeight < 0 || differentialWeight > 1)
            throw new ArgumentOutOfRangeException(nameof(differentialWeight), "Harus antara 0 dan 1");

        if (crossoverRate < 0 || crossoverRate > 1)
            throw new ArgumentOutOfRangeException(nameof(crossoverRate), "Harus antara 0 dan 1");

        var populasi = populasiAwal.Select(p => new Vector(p)).ToList();
        int generasi = 0;
        int generationsWithoutChanges = 0;
        double deltaGlobalBestFitness = double.PositiveInfinity;
        double globalBestFitness = jenisOptimasi == JenisOptimasi.Maks ? double.MinValue : double.MaxValue;
        List<Vector> localBests = new List<Vector>();

        var isBest = (double a, double b) => jenisOptimasi == JenisOptimasi.Maks ? a > b : a < b;
        var fObjektif = (Vector v) => encoding is null ? fungsiObjektif(v) : fungsiObjektif(encoding.Decode(v));

        while (generasi < maksGenerasi && (generationsWithoutChanges < patience || deltaGlobalBestFitness >= minDeltaGlobalBestFitness))
        {
            var populasiBaru = new Vector[jumlahPopulasi];
            var indexIndividuTerbaik = populasi.Select(p => fungsiObjektif(p)).ToList().ArgMin();

            Parallel.For(0, jumlahPopulasi, (i) =>
            {
                //Mutasi
                var donor = Mutasi(
                    populasi,
                    fObjektif,
                    jenisOptimasi,
                    skemaMutasi,
                    pasanganMutasi,
                    differentialWeight);

                //Crossover
                var crossover = Crossover(
                    populasi[i],
                    donor,
                    jumlahGen,
                    jenisCrossover,
                    crossoverRate);

                //Seleksi
                if (encoding is not null)
                    crossover = encoding.Constraint(crossover);

                if (elitism)
                    populasiBaru[i] = isBest(fObjektif(crossover), fObjektif(populasi[i])) && i != indexIndividuTerbaik ? crossover : populasi[i];
                else
                    populasiBaru[i] = isBest(fObjektif(crossover), fObjektif(populasi[i])) ? crossover : populasi[i];
            });

            populasi = populasiBaru.ToList();
            generasi++;
            var localBest = populasi.Zip(populasi.Select(p => fObjektif(p))).Aggregate((b, c) => isBest(b.Second, c.Second) ? b : c).First;
            localBests.Add(new Vector(localBest));
            var localBestFitness = fObjektif(localBest);
            if (isBest(localBestFitness, globalBestFitness))
            {
                deltaGlobalBestFitness = Math.Abs(globalBestFitness - localBestFitness);
                globalBestFitness = localBestFitness;
                generationsWithoutChanges = 0;
            }
            else
            {
                deltaGlobalBestFitness = Math.Abs(globalBestFitness - globalBestFitness);
                generationsWithoutChanges++;
            }
        }

        var globalBest = localBests.Zip(localBests.Select(p => fObjektif(p))).Aggregate(
            (b, c) => isBest(b.Second, c.Second) ? b : c).First;

        return new DifferentialEvolutionResult(
            jumlahGen,
            jumlahPopulasi,
            generasi,
            populasi,
            localBests,
            globalBest);
    }

    private static Vector Mutasi(
        List<Vector> populasi,
        Func<Vector, double> fungsiObjektif,
        JenisOptimasi jenisOptimasi,
        SkemaMutasi skemaMutasi,
        int pasanganMutasi,
        double differentialWeight)
    {
        var random = new Random();
        if (skemaMutasi == SkemaMutasi.Rand)
        {
            var randomVectors = random.GetDistinctItems(populasi.ToArray(), pasanganMutasi * 2 + 1);
            var donor = randomVectors[0];

            for (int i = 0; i < pasanganMutasi; i++)
                donor = donor + differentialWeight * (randomVectors[i * 2 + 1] - randomVectors[i * 2 + 2]);

            return donor;
        }
        else
        {
            var randomVectors = random.GetDistinctItems(populasi.ToArray(), pasanganMutasi * 2);
            var donor = populasi.Zip(populasi.Select(p => fungsiObjektif(p))).Aggregate(
                (b, c) => (jenisOptimasi == JenisOptimasi.Maks ? c.Second > b.Second : c.Second < b.Second) ? c : b).First;

            for (int i = 0; i < pasanganMutasi; i++)
                donor = donor + differentialWeight * (randomVectors[i * 2] - randomVectors[i * 2 + 1]);

            return donor;
        }
    }

    private static Vector Crossover(
        Vector asli,
        Vector donor,
        int jumlahGen,
        JenisCrossover jenisCrossover,
        double crossoverRate)
    {
        var random = new Random();
        var result = new Vector(jumlahGen);

        if (jenisCrossover == JenisCrossover.Binomial)
        {
            for (int i = 0; i < jumlahGen; i++)
            {
                var r = random.NextDouble();
                if (r <= crossoverRate)
                    result[i] = donor[i];
                else
                    result[i] = asli[i];
            }
        }
        else  
        {
            var k = random.Next(0, jumlahGen);
            var l = random.Next(1, jumlahGen + 1);

            for (int i = 0; i < jumlahGen; i++)
            {
                if (i >= k && i <= k + l - 1)
                    result[i] = donor[i];
                else
                    result[i] = asli[i];
            }
        }

        return result;
    }
}