using System.ComponentModel.DataAnnotations;

namespace crudProduct.Models;

public class Product
{
    [Key]
    public int Id { get; set; }
    [Required(ErrorMessage = "The name field is required")]
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
}
