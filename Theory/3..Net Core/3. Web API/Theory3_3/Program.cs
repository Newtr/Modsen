using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

List<Product> myProducts = new List<Product>() 
{
    new Product("Coffe", 12, "DCoffe"),
    new Product("Tea", 16, "DTea"),
    new Product("Candy", 22, "DCandy")
};

app.MapGet("/MyProducts", () => 
{

    return Results.Ok(myProducts);

}).WithName("GetMyProducts");

app.MapGet("/MyProduct/{Pname}", (string Pname) => 
{

    foreach (var item in myProducts)
    {
        if (item.Name == Pname)
        {
            return Results.Ok(item);
        }
    } 
    return Results.NoContent();

}).WithName("GetMyProductName");

app.MapPost("/CreateProduct", (Product newProduct) =>
{

    myProducts.Add(newProduct);
    return Results.Created($"/MyProduct/{newProduct.Name}", newProduct);

}).WithName("PostCreateProduct");

app.MapPut("/UpdateProduct", ([FromBody] ProductUpdateRequest updateRequest) =>
{
    foreach (var item in myProducts)
    {
        if (item.Name == updateRequest.OldName)
        {
            item.Name = updateRequest.NewName;
            return Results.Ok(item);
        }
    }

    return Results.NotFound($"Product with name '{updateRequest.OldName}' not found.");
}).WithName("PutUpdateProduct");

app.MapDelete("/DeleteProduct/{PName}", (string PName) =>
{
    var listlength = myProducts.ToArray().Length;
    for (int i = 0; i < listlength; i++)
    {
        if (myProducts[i].Name == PName)
        {
            myProducts.RemoveAt(i);
            return Results.Ok($"Product with name {PName} was deleted!");
        }
    }
    return Results.NoContent();
});


app.Run();

public class Product
{
    public string Name {get; set;}
    public int Price  {get; set;}
    public string Description {get; set;}

    public Product(string name, int price, string description)
    {
        Name = name;
        Price = price;
        Description = description;
    }
}

public class ProductUpdateRequest
{
    public string OldName { get; set; }
    public string NewName { get; set; }
}
