using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var list = new List<object>();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapPost("/", (HttpContext context) =>
{
    
    bool xml = false;
    
    if (context.Request.Headers.TryGetValue("xml", out var xmlHeaderValue))
    {
        if (bool.TryParse(xmlHeaderValue.ToString(), out bool parsedXml))
        {
            xml = parsedXml;
        }
        
    }

    if (xml)
    {
        var serializer = new System.Xml.Serialization.XmlSerializer(list.GetType());
        using var writer = new StringWriter();
        serializer.Serialize(writer, list);
        return Results.Text(writer.ToString(), "application/xml");
    }

    return Results.Ok(list);
});

app.MapPut("/", (HttpContext context) =>
{
    // Validar que quantity sea un número entero válido
    if (!int.TryParse(context.Request.Form["quantity"], out int quantity))
    {
        return Results.BadRequest(new { error = "'quantity' must be a valid integer" });
    }

    // Validar quantity
    if (quantity <= 0)
    {
        return Results.BadRequest(new { error = "'quantity' must be higher than zero" });
    }

    // Validar type
    var type = context.Request.Form["type"].ToString();
    if (string.IsNullOrEmpty(type) || (type != "int" && type != "float"))
    {
        return Results.BadRequest(new { error = "'type' must be either 'int' or 'float'" });
    }

    var random = new Random();
    if (type == "int")
    {
        for (; quantity > 0; quantity--)
        {
            list.Add(random.Next());
        }
    }
    else if (type == "float")
    {
        for (; quantity > 0; quantity--)
        {
            list.Add(random.NextSingle());
        }
    }

    return Results.Ok();
}).DisableAntiforgery();

app.MapDelete("/", (HttpContext context) =>
{
    if (!int.TryParse(context.Request.Form["quantity"], out int quantity))
    {
        return Results.BadRequest(new { error = "'quantity' must be a valid integer" });
    }

    if (quantity <= 0)
    {
        return Results.BadRequest(new { error = "'quantity' must be higher than zero" });
    }

    if (list.Count < quantity)
    {
        return Results.BadRequest(new { error = $"List has only {list.Count} elements, but {quantity} were requested to be removed" });
    }

    list.RemoveRange(0, quantity);
    
    return Results.Ok();
}).DisableAntiforgery();

app.MapPatch("/", () =>
{
    list.Clear();
    
    return Results.Ok(new { message = "List cleared successfully" });
});

app.Run();
