using System.ComponentModel.DataAnnotations;
namespace Northwind.Mvc.Models;

public record Thing([Range(1,10)]int? Id, [Required]string Colour, [EmailAddress]string? Email);
