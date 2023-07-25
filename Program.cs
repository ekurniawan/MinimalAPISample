using Microsoft.EntityFrameworkCore;
using SampleAPI.Data;
using SampleAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("SqliteConnection"));
});

var app = builder.Build();

app.MapGet("api/todo", async (AppDbContext context) =>
{
    var todos = await context.ToDos.ToListAsync();
    return Results.Ok(todos);
});

app.MapGet("api/todo/{id}", async (AppDbContext context, int id) =>
{
    var todo = await context.ToDos.FindAsync(id);
    if (todo is null)
    {
        return Results.NotFound();
    }
    return Results.Ok(todo);
});

app.MapPost("api/todo", async (AppDbContext context, ToDo todo) =>
{
    await context.ToDos.AddAsync(todo);
    await context.SaveChangesAsync();
    return Results.Created($"/api/todo/{todo.Id}", todo);
});

app.MapPut("api/todo/{id}", async (AppDbContext context, int id, ToDo todo) =>
{
    var toDoModel = await context.ToDos.FirstOrDefaultAsync(x => x.Id == id);
    if (toDoModel is null)
    {
        return Results.NotFound();
    }
    toDoModel.ToDoName = todo.ToDoName;
    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("api/todo/{id}", async (AppDbContext context, int id) =>
{
    var toDoModel = await context.ToDos.FirstOrDefaultAsync(x => x.Id == id);
    if (toDoModel is null)
    {
        return Results.NotFound();
    }
    context.ToDos.Remove(toDoModel);
    await context.SaveChangesAsync();
    return Results.NoContent();
});

//app.UseHttpsRedirection();
app.Run();

