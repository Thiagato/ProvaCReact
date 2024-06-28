using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDataContext>();

var app = builder.Build();


app.MapGet("/", () => "Prova A1");

//ENDPOINTS DE CATEGORIA
//GET: http://localhost:5273/categoria/listar
app.MapGet("/categoria/listar", ([FromServices] AppDataContext ctx) =>
{
    if (ctx.Categorias.Any())
    {
        return Results.Ok(ctx.Categorias.ToList());
    }
    return Results.NotFound("Nenhuma categoria encontrada");
});

//POST: http://localhost:5273/categoria/cadastrar
app.MapPost("/categoria/cadastrar", ([FromServices] AppDataContext ctx, [FromBody] Categoria categoria) =>
{
    ctx.Categorias.Add(categoria);
    ctx.SaveChanges();
    return Results.Created("", categoria);
});

//ENDPOINTS DE TAREFA
//GET: http://localhost:5273/tarefas/listar
app.MapGet("/tarefas/listar", ([FromServices] AppDataContext ctx) =>
{
    if (ctx.Tarefas.Any())
    {
        return Results.Ok(ctx.Tarefas.ToList());
    }
    return Results.NotFound("Nenhuma tarefa encontrada");
});

//POST: http://localhost:5273/tarefas/cadastrar
app.MapPost("/tarefas/cadastrar", ([FromServices] AppDataContext ctx, [FromBody] Tarefa tarefa) =>
{
    Categoria? categoria = ctx.Categorias.Find(tarefa.CategoriaId);
    if (categoria == null)
    {
        return Results.NotFound("Categoria não encontrada");
    }
    tarefa.Categoria = categoria;
    ctx.Tarefas.Add(tarefa);
    ctx.SaveChanges();
    return Results.Created("", tarefa);
});

//PUT: http://localhost:5273/tarefas/alterar/{id}
app.MapPut("/tarefas/alterar/{id}", ([FromServices] AppDataContext ctx, [FromRoute] string id, [FromBody] Tarefa tarefaAtualizada) =>
{
    var tarefa = ctx.Tarefas.Find(id);

    if (tarefa == null)
    {
        return Results.NotFound("Tarefa não encontrada");
    }

    
    tarefa.Titulo = tarefaAtualizada.Titulo;
    tarefa.Descricao = tarefaAtualizada.Descricao;
    tarefa.Status = tarefaAtualizada.Status;
    tarefa.CategoriaId = tarefaAtualizada.CategoriaId;

    ctx.SaveChanges();

    return Results.Ok(tarefa);
});

app.MapDelete("/tarefas/deletar/{id}", ([FromServices] AppDataContext ctx, [FromRoute] string id) =>
{
    var tarefa = ctx.Tarefas.Find(id);

    if (tarefa == null)
    {
        return Results.NotFound("Tarefa não encontrada");
    }

    ctx.Tarefas.Remove(tarefa);
    ctx.SaveChanges();

    return Results.Ok();
});

//GET: http://localhost:5273/tarefas/naoconcluidas
app.MapGet("/tarefas/naoconcluidas", ([FromServices] AppDataContext ctx) =>
{
    var tarefasNaoConcluidas = ctx.Tarefas.Where(t => t.Status != "Concluída").ToList();

    if (tarefasNaoConcluidas.Any())
    {
        return Results.Ok(tarefasNaoConcluidas);
    }

    return Results.NotFound("Nenhuma tarefa não concluída encontrada");
});

app.MapGet("/tarefas/concluidas", ([FromServices] AppDataContext ctx) =>
{
    var tarefasConcluidas = ctx.Tarefas.Where(t => t.Status == "Concluída").ToList();

    if (tarefasConcluidas.Any())
    {
        return Results.Ok(tarefasConcluidas);
    }

    return Results.NotFound("Nenhuma tarefa concluída encontrada");
});

app.Run();
