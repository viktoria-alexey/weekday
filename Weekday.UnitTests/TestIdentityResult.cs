using Microsoft.AspNetCore.Identity;

namespace Weekday.UnitTests
{
    public class TestIdentityResult:IdentityResult
    {
        public TestIdentityResult(bool result = true)
        {
            Succeeded = result;
            if (!result)
            {
                Failed(new IdentityError
                {
                    Code = "red",
                    Description = "error"
                });
            }
        }
    }
}