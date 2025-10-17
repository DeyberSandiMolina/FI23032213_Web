using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Xml.Serialization;

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

app.MapGet("/", () => Results.Redirect("/swagger/index.html"));

app.MapPost("/include/{position:int}", async (HttpContext context, int position) =>
{
    string? value = context.Request.Query["value"];
    string? text = context.Request.Form["text"];
    bool xml = false;

    if (context.Request.Headers.TryGetValue("xml", out var xmlHeader))
    {
        bool.TryParse(xmlHeader, out xml);
    }

    if (position < 0)
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsJsonAsync(new { error = "'position' must be 0 or higher" });
        return;
    }

    if (string.IsNullOrWhiteSpace(value))
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsJsonAsync(new { error = "'value' cannot be empty" });
        return;
    }

    if (string.IsNullOrWhiteSpace(text))
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsJsonAsync(new { error = "'text' cannot be empty" });
        return;
    }

    var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

    if (position >= words.Count)
        words.Add(value);
    else
        words.Insert(position, value);

    var newText = string.Join(" ", words);

    var result = new { OriginalText = text, NewText = newText };

    if (xml)
    {
        context.Response.ContentType = "application/xml";

        var xmlResult = new Result
        {
            OriginalText = text,
            NewText = newText
        };

        var serializer = new XmlSerializer(typeof(Result));
        var stringWriter = new StringWriter(new StringBuilder());
        serializer.Serialize(stringWriter, xmlResult);

        await context.Response.WriteAsync(stringWriter.ToString());
    }
    else
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(result);
    }
});


app.MapPut("/replace/{length:int}", async (HttpContext context, int length) =>
{
    string? value = context.Request.Query["value"];
    string? text = context.Request.Form["text"];
    bool xml = false;

    if (context.Request.Headers.TryGetValue("xml", out var xmlHeader))
    {
        bool.TryParse(xmlHeader, out xml);
    }

    if (length <= 0)
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsJsonAsync(new { error = "'length' must be greater than 0" });
        return;
    }

    if (string.IsNullOrWhiteSpace(value))
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsJsonAsync(new { error = "'value' cannot be empty" });
        return;
    }

    if (string.IsNullOrWhiteSpace(text))
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsJsonAsync(new { error = "'text' cannot be empty" });
        return;
    }

    var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

    for (int i = 0; i < words.Count; i++)
    {
        string cleanWord = new string(words[i].Where(char.IsLetter).ToArray());
        if (cleanWord.Length == length)
            words[i] = value;
    }

    var newText = string.Join(" ", words);

    var result = new
    {
        OriginalText = text,
        NewText = newText
    };

    if (xml)
    {
        context.Response.ContentType = "application/xml";
        var xmlResult = new Result
        {
            OriginalText = text,
            NewText = newText
        };

        var serializer = new XmlSerializer(typeof(Result));
        var stringWriter = new StringWriter(new StringBuilder());
        serializer.Serialize(stringWriter, xmlResult);
        await context.Response.WriteAsync(stringWriter.ToString());
    }
    else
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(result);
    }
});

app.MapDelete("/erase/{length:int}", async (HttpContext context, int length) =>
{
    string? text = context.Request.Form["text"];
    bool xml = false;

    if (context.Request.Headers.TryGetValue("xml", out var xmlHeader))
    {
        bool.TryParse(xmlHeader, out xml);
    }

    if (length <= 0)
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsJsonAsync(new { error = "'length' must be greater than 0" });
        return;
    }

    if (string.IsNullOrWhiteSpace(text))
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsJsonAsync(new { error = "'text' cannot be empty" });
        return;
    }

    var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
    var filteredWords = words.Where(w =>
    {
        string cleanWord = new string(w.Where(char.IsLetter).ToArray());
        return cleanWord.Length != length;
    }).ToList();

    var newText = string.Join(" ", filteredWords);

    var result = new
    {
        OriginalText = text,
        NewText = newText
    };

    if (xml)
    {
        context.Response.ContentType = "application/xml";
        var xmlResult = new Result
        {
            OriginalText = text,
            NewText = newText
        };

        var serializer = new XmlSerializer(typeof(Result));
        var stringWriter = new StringWriter(new StringBuilder());
        serializer.Serialize(stringWriter, xmlResult);
        await context.Response.WriteAsync(stringWriter.ToString());
    }
    else
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(result);
    }
});


app.Run();

public class Result
{
    public string OriginalText { get; set; } = string.Empty;
    public string NewText { get; set; } = string.Empty;
}
