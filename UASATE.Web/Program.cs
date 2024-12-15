using Kruskal.Core;
using Microsoft.AspNetCore.Mvc;
using UASATE.Core;
using UASATE.Core.Encoding;
using UASATE.Web.Models;
using UASATE.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IDestinasiWisataService, DestinasiWisataService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapGet("/destinasi-wisata", (IDestinasiWisataService service) =>
{
    return Results.Ok(service.GetAll());
});

app.MapGet("/destinasi-wisata/edges", (IDestinasiWisataService service) =>
{
    return Results.Ok(service.GetGraph().EdgesDistinct);
});

app.MapGet("/destinasi-wisata/{nama}", (IDestinasiWisataService service, string nama) =>
{
    var destinasi = service.Get(nama);

    if (destinasi is null) return Results.NotFound();

    return Results.Ok(destinasi);
});

app.MapPost("/destinasi-wisata", (IDestinasiWisataService service, [FromBody]SetDistanceRequest request) =>
{
    var destinasiA = service.Get(request.A);
    var destinasiB = service.Get(request.B);

    if (destinasiA is null || destinasiB is null) return Results.BadRequest();

    service.SetDistance(destinasiA, destinasiB, request.Distance);

    return Results.Ok();
});

app.MapGet(
    "/destinasi-wisata/lintasan", 
    ([FromServices]IDestinasiWisataService service, [FromServices]ILogger<DestinasiWisata> logger, [FromQuery]string[] d) =>
    {
        var daftarDestinasi = new List<DestinasiWisata>();
        foreach(var namaDestinasi in d)
        {
            var destinasi = service.Get(namaDestinasi);
            if (destinasi is null) return Results.NotFound();

            daftarDestinasi.Add(destinasi);
        }

        var graph = service.GetGraph();

        var subGraph = new Graph<DestinasiWisata>(daftarDestinasi, []);

        foreach (var destinasi in daftarDestinasi)
            foreach (var destinasi2 in daftarDestinasi)
                if (destinasi != destinasi2 && !subGraph.EdgesDistinct.Contains(new Edge<DestinasiWisata>(destinasi, destinasi2, 0)))
                    subGraph.AddEdge(new Edge<DestinasiWisata>(
                        destinasi,
                        destinasi2,
                        graph.EdgeCost(new Vertex<DestinasiWisata>(destinasi), new Vertex<DestinasiWisata>(destinasi2))));

        var maksGenerasi = 1000;
        var jumlahPopulasi = 20;
    
        var random = new Random();
        var encoding = new OrdinalEncoding(subGraph.Vertices.Count);
    
        var fungsiObjektif = (Vector v) =>
        {
            var cost = 0d;
    
            for (var i = 0; i < v.Dimension; i++)
            {
                if (i < v.Dimension - 1)
                {
                    var v1 = subGraph.Vertices[(int)v[i]];
                    var v2 = subGraph.Vertices[(int)v[i + 1]];
    
                    cost += subGraph.EdgeCost(v1, v2);
                }
                else
                {
                    var v1 = subGraph.Vertices[(int)v[i]];
                    var v2 = subGraph.Vertices[(int)v[0]];
    
                    cost += subGraph.EdgeCost(v1, v2);
                }
            }
    
            return cost;
        };

        var vector2Destinasi = (Vector v) =>
        {
            var list = new List<string>();

            for (int i = 0; i < v.Dimension; i++)
                list.Add(subGraph.Vertices[(int)v[i]].Value.Nama);

            return list;
        };
    
        var populasiAwal = encoding.GeneratePopulasi(jumlahPopulasi);
    
        logger.LogInformation($"Jumlah Gen : {subGraph.Vertices.Count}");
        logger.LogInformation($"Jumlah Populasi: {jumlahPopulasi}");
    
        logger.LogInformation($"Populasi Awal : \n{string.Join("\n", populasiAwal.Select(v => encoding.Decode(v)))}");
    
        var result = DifferentialEvolution.Run(
            jumlahGen: subGraph.Vertices.Count,
            jumlahPopulasi: jumlahPopulasi,
            jenisOptimasi: JenisOptimasi.Min,
            skemaMutasi: SkemaMutasi.Best,
            jenisCrossover: JenisCrossover.Binomial,
            fungsiObjektif: fungsiObjektif,
            populasiAwal: populasiAwal,
            encoding: encoding,
            maksGenerasi: maksGenerasi,
            patience:40);
    
        logger.LogInformation("\nHasil :");
        logger.LogInformation($"Global Best : {encoding.Decode(result.GlobalBest)}");
        logger.LogInformation($"Global Best Fitness : {fungsiObjektif(encoding.Decode(result.GlobalBest)):F6}");
        logger.LogInformation($"Jumlah Generasi : {result.JumlahGenerasi}");
    
        var lintasan = new List<DestinasiWisata>();
        var decoded = encoding.Decode(result.GlobalBest);
        for (int i = 0; i < decoded.Dimension; i++)
            lintasan.Add(subGraph.Vertices[(int)decoded[i]].Value);
    
        return Results.Ok(new
        {
            lintasan,
            totalJarak = fungsiObjektif(decoded),
            globalBest = result.GlobalBest.ToString(),
            globalBestDecoded = vector2Destinasi(decoded),
            globalBestFitness = fungsiObjektif(decoded),
            localBests = result.LocalBests.Select(l => l.ToString()),
            localBestsDecoded = result.LocalBests.Select(l => vector2Destinasi(encoding.Decode(l))).ToList(),
            localBestsFitness = result.LocalBests.Select(l => fungsiObjektif(encoding.Decode(l))).ToList(),
            populasiAwal = populasiAwal.Select(l => l.ToString()),
            populasiAwalDecoded = populasiAwal.Select(l => vector2Destinasi(encoding.Decode(l))).ToList(),
            populasiAwalFitness = populasiAwal.Select(l => fungsiObjektif(encoding.Decode(l))).ToList(),
            populasiAkhir = result.Populasi.Select(l => l.ToString()),
            populasiAkhirDecoded = result.Populasi.Select(l => vector2Destinasi(encoding.Decode(l))).ToList(),
            populasiAkhirFitness = result.Populasi.Select(l => fungsiObjektif(encoding.Decode(l))).ToList(),
            jumlahPopulasi,
            jumlahGenerasi = result.JumlahGenerasi,
            jumlahGen = result.JumlahGen,
            maksGenerasi
        });
    });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
