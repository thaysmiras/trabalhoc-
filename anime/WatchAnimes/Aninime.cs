using Animes.Data;
using NSwag.AspNetCore;
using Videos.Models;

class Aninime
{
    static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOpenApiDocument(config =>
        {
            config.DocumentName = "Aninime";
            config.Title = "Aninime";
            config.Version = "v1";
        });

        builder.Services.AddDbContext<AppDbContext>();
        
        WebApplication app = builder.Build();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseOpenApi();
            app.UseSwaggerUi(config =>
            {
                config.DocumentTitle = "Aninime API";
                config.Path = "/swagger";
                config.DocumentPath = "/swagger/{documentName}/swagger.json";
                config.DocExpansion = "list";
            });
        }

        app.MapGet("/animes", (AppDbContext context) =>
        {
        var videos = context.Animes;
        return videos is not null ? Results.Ok(videos) 
            : Results.NotFound();
        }).Produces<Anime>();

        app.MapGet("/animes/{id}", (AppDbContext context, Guid id) => {
            var anime = context.Animes.Find(id);
            if (anime != null) {
                return Results.Ok(anime);
            }
            else {
                return Results.NotFound();
            }
        }).Produces<Anime>();

        app.MapGet("/animes/premium", (AppDbContext context) => {
            var premiumAnimes = context.Animes.Where(anime => anime.premium);
            return Results.Ok(premiumAnimes);
        }).Produces<Anime>();


        app.MapPost("/animes", (AppDbContext context, Anime anime) =>{
    
            context.Animes.Add(anime);
            context.SaveChanges();
            return Results.Created($"/animes/{anime.Id}", anime);
        }).Produces<Anime>();

        app.MapPatch("/animes/{id}/premium", ( AppDbContext context, Guid id, bool isPremium) => {
            var anime = context.Animes.Find(id);
            if (anime == null) {
                return Results.NotFound("Anime não encontrado.");
            }
            
            var updatedAnime = anime with { premium = isPremium };
            context.Entry(anime).CurrentValues.SetValues(updatedAnime);
            context.SaveChanges();
            
            return Results.Ok(updatedAnime);
        }).Produces<Anime>();

        app.MapPut("/animes/{id}", (AppDbContext context, Guid id, Anime updatedAnime) => {
            var existingAnime = context.Animes.Find(id);
            if (existingAnime == null) {
                return Results.NotFound("Anime não encontrado.");
            }

            context.Animes.Remove(existingAnime);
            context.SaveChanges();

            var newAnime = new Anime(
                id,
                updatedAnime.Name,
                updatedAnime.gender,
                updatedAnime.premium,
                updatedAnime.classification,
                updatedAnime.ultimoep
            );

            context.Animes.Add(newAnime);
            context.SaveChanges();

            return Results.Ok(newAnime);
        }).Produces<Anime>();





        app.MapDelete("/animes/{id}", (AppDbContext context, string id) => {
            var anime = context.Animes.Find(id);
            if (anime == null) {
                return Results.NotFound("Anime não encontrado.");
            }
            context.Animes.Remove(anime);
            context.SaveChanges();
            return Results.Ok();
        }).Produces<Anime>();


        app.Run();
    }
}
