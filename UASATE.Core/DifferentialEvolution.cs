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
        int pasanganMutasi = 1,
        double minDeltaBestFitness = 0.001,
        double differentialWeight = 0.9,
        double crossoverRate = 0.5,
        bool verbose = false)
    {
        //Iniliasasi
        var random = new Random();

        if (jumlahPopulasi < pasanganMutasi * 2 + 2)
            throw new ArgumentException(nameof(jumlahPopulasi), "Tidak cukup");

        if (populasiAwal.Count != jumlahPopulasi)
            throw new ArgumentException(nameof(populasiAwal), $"Panjang tidak sama dengan {nameof(jumlahPopulasi)}");

        if (differentialWeight < 0 || differentialWeight > 1)
            throw new ArgumentOutOfRangeException(nameof(differentialWeight), "Harus antara 0 dan 1");

        if (crossoverRate < 0 || crossoverRate > 1)
            throw new ArgumentOutOfRangeException(nameof(crossoverRate), "Harus antara 0 dan 1");

        var populasi = populasiAwal.Select(p => new Vector(p)).ToList();
        int generasi = 0;
        double deltaBestFitness = double.PositiveInfinity;
        double globalBestFitness = jenisOptimasi == JenisOptimasi.Maks ? double.MinValue : double.MaxValue;
        List<Vector> localBests = new List<Vector>();

        var isBest = (double a, double b) => jenisOptimasi == JenisOptimasi.Maks ? a > b : a < b;

        while (deltaBestFitness > minDeltaBestFitness)
        {
            for (int i = 0; i < jumlahPopulasi; i++)
            {
                //Mutasi
                var donor = Mutasi(
                    populasi,
                    fungsiObjektif,
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
                if (jenisOptimasi == JenisOptimasi.Maks)
                    populasi[i] = fungsiObjektif(crossover) > fungsiObjektif(populasi[i]) ? crossover : populasi[i];
                else
                    populasi[i] = fungsiObjektif(crossover) < fungsiObjektif(populasi[i]) ? crossover : populasi[i];
            }

            generasi++;
            var localBest = populasi.Zip(populasi.Select(p => fungsiObjektif(p))).Aggregate((b, c) => isBest(b.Second, c.Second) ? b : c).First;
            localBests.Add(new Vector(localBest));
            var localBestFitness = fungsiObjektif(localBest);
            if (isBest(localBestFitness, globalBestFitness))
            {
                deltaBestFitness = Math.Abs(globalBestFitness - localBestFitness);
                globalBestFitness = localBestFitness;
            }
        }

        var globalBest = localBests.Zip(localBests.Select(p => fungsiObjektif(p))).Aggregate(
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
            var randomVectors = random.GetItems(populasi.ToArray(), pasanganMutasi * 2 + 1);
            var donor = randomVectors[0];

            for (int i = 0; i < pasanganMutasi; i++)
                donor = donor + differentialWeight * (randomVectors[i * 2 + 1] - randomVectors[i * 2 + 2]);

            return donor;
        }
        else
        {
            var randomVectors = random.GetItems(populasi.ToArray(), pasanganMutasi * 2);
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