using Mdk.Shared;

namespace Northwind.Common.UnitTests;

public class EntityModelTests
{
    [Fact]
    public void SqliteDatabaseConnectTest()
    {
        using(Mdk.Shared.Sqlite.NorthwindContext db = new())
        {
            Assert.True(db.Database.CanConnect());
        }
    }

    [Fact]
    public void SqliteCategoryCountTest()
    {
        using(Mdk.Shared.Sqlite.NorthwindContext db = new())
        {
            int expected = 8;
            int actual = db.Categories.Count();
            Assert.Equal(expected, actual);
        }    
    }

        [Fact]
    public void SqlServerDatabaseConnectTest()
    {
        using(Mdk.Shared.SqlServer.NorthwindContext db = new())
        {
            Assert.True(db.Database.CanConnect());
        }
    }

    [Fact]
    public void SqlServerCategoryCountTest()
    {
        using(Mdk.Shared.SqlServer.NorthwindContext db = new())
        {
            int expected = 8;
            int actual = db.Categories.Count();
            Assert.Equal(expected, actual);
        }    
    }
}