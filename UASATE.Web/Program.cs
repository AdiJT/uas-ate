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

app.MapGet("/destinasi-wisata/lintasan", ([FromServices]IDestinasiWisataService service, [FromServices]ILogger<DestinasiWisata> logger) =>
{
    var graph = service.GetGraph();

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

    logger.LogInformation($"Jumlah Gen : {graph.Vertices.Count}");
    logger.LogInformation($"Jumlah Populasi: {jumlahPopulasi}");

    logger.LogInformation($"Populasi Awal : \n{string.Join("\n", populasiAwal.Select(v => encoding.Decode(v)))}");

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

    logger.LogInformation("\nHasil :");
    logger.LogInformation($"Global Best : {encoding.Decode(result.GlobalBest)}");
    logger.LogInformation($"Global Best Fitness : {fungsiObjektif(encoding.Decode(result.GlobalBest)):F6}");
    logger.LogInformation($"Jumlah Generasi : {result.JumlahGenerasi}");

    var lintasan = new List<DestinasiWisata>();
    var encoded = encoding.Decode(result.GlobalBest);
    for (int i = 0; i < encoded.Dimension; i++)
        lintasan.Add(graph.Vertices[(int)encoded[i]].Value);

    return Results.Ok(new
    {
        lintasan,
        totalJarak = fungsiObjektif(encoded)
    });
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
