using Mdk.Shared;

namespace Northwind.Mvc;

public record HomeIndexViewModel
(
    int VisitorCount,
    IList<Category> Categories,
    IList<Product> Products
);
