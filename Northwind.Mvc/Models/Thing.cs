using System.ComponentModel.DataAnnotations;
namespace Northwind.Mvc;

public record Thing([Range(1,10)]int? Id, [Required]string Colour, [EmailAddress]string? Email);
