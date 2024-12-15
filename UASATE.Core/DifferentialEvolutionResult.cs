namespace UASATE.Core;

public record DifferentialEvolutionResult(
    int JumlahGen,
    int JumlahPopulasi,
    int JumlahGenerasi,
    List<Vector> Populasi,
    List<Vector> LocalBests,
    Vector GlobalBest)
{
}
