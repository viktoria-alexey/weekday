using Microsoft.AspNetCore.Identity;

namespace Weeday.UnitTests
{
    public class TestIdentityResult:IdentityResult
    {
        public TestIdentityResult()
        {
            Succeeded = true;
        }
    }
}