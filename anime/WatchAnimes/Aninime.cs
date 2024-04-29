
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

        app.MapGet("/animes/{id}", (HttpContext httpContext) => {
            var id = httpContext.Request.RouteValues["id"]?.ToString();
            if (id == null) {
                return Results.NotFound();
            }
            
            var anime = context.Animes.Find(id);
            if (anime != null) {
                return Results.Ok(anime);
            }
            else {
                return Results.NotFound();
            }
        }).Produces <Anime>();



        app.MapPost("/animes/batch", (AppDbContext context, List
            Anime> newAnimes) => {
                context.Animes.AddRange(newAnimes);
                context.SaveChanges();
                return Results.Created($"/animes", newAnimes);
            }).Produces<Anime>();




        app.MapPost("/animes", (AppDbContext context, Anime anime) =>{
    
            context.Animes.Add(anime);
            context.SaveChanges();
            return Results.Created($"/animes/{anime.Id}", anime);
        }).Produces<Anime>();

        app.MapPatch("/animes/{id}/premium", ( Guid id, bool isPremium) => {
            var anime = context.Animes.Find(id);
            if (anime == null) {
                return Results.NotFound("Anime não encontrado.");
            }
            
            var updatedAnime = anime with { IsPremium = isPremium };
            context.Entry(anime).CurrentValues.SetValues(updatedAnime);
            context.SaveChanges();
            
            return Results.Ok(updatedAnime);
        }).Produces<Anime>().WithName("UpdateAnimePremiumStatus");

        app.MapPut("/animes/{id}", (AppDbContext context, string id, Anime updatedAnime) => {
            var existingAnime = context.Animes.Find(id);
            if (existingAnime == null) {
                return Results.NotFound("Anime não encontrado.");
            }
            existingAnime.Title = updatedAnime.Title;
            existingAnime.Description = updatedAnime.Description;
            existingAnime.ReleaseDate = updatedAnime.ReleaseDate;
            existingAnime.IsPremium = updatedAnime.IsPremium;
            context.SaveChanges();
            return Results.Ok(existingAnime);
        }).Produces

        app.MapDelete("/animes/{id}", (AppDbContext context, string id) => {
            var anime = context.Animes.Find(id);
            if (anime == null) {
                return Results.NotFound("Anime não encontrado.");
            }
            context.Animes.Remove(anime);
            context.SaveChanges();
            return Results.Ok();
        }).Produces(HttpStatusCode.OK, HttpStatusCode.NotFound);


        app.Run();
    }
}
