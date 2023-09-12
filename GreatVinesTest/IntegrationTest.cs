using GreatVibesSchedule.helper;
namespace GreatVinesTest;

public class IntegrationTest {
    [Fact]
    public void CanExecuteStoredProcedure()
    {
        string storedProcedureName = "Products";
        var result = SqlServer_Helper.ExecuteStoredProcedure(storedProcedureName);
        Assert.NotNull(result);
    }
}
