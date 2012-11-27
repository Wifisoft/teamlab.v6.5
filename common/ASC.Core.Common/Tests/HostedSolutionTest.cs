#if DEBUG
namespace ASC.Core.Common.Tests
{
    using System.Configuration;
    using NUnit.Framework;

    [TestFixture]
    public class HostedSolutionTest
    {
        [Test]
        public void FindTenants()
        {
            var h = new HostedSolution(ConfigurationManager.ConnectionStrings["core"]);
            var tenants = h.FindTenants("76ff727b-f987-4871-9834-e63d4420d6e9");
            CollectionAssert.IsNotEmpty(tenants);
        }
    }
}
#endif
