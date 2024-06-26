using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

// Add DI - AddService
builder.Services.AddDbContext<TodoDb>(options => options.UseInMemoryDatabase("TodoList"));

var app = builder.Build();

// Configure pipeline - UseMethod
app.MapGet("/todoitems", async (TodoDb db) => await db.Todos.ToListAsync());

app.MapGet("/todoitems/{id}", async (int id, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);

    if (todo is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(todo);
});

app.MapPost("/todoitems", async (TodoItem todo, TodoDb db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/todoitems/{todo.Id}", todo);
});

app.MapPut("/todoitems/{id}", async (int id, TodoItem inputTodo, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo == null) return Results.NotFound();

    todo.Name = inputTodo.Name;
    todo.IsComplete = inputTodo.IsComplete;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/todoitems/{id}", async (int id, TodoDb db) =>
{
    var todoItem = await db.Todos.FindAsync(id);
    if (todoItem == null)
    {
        return Results.NotFound();
    }

    db.Todos.Remove(todoItem);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();