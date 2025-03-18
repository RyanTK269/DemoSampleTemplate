using DemoSampleTemplate.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace DemoSampleTemplate.Controllers
{
    [ApiController]
    public class TestController : Controller
    {
        [HttpGet("TestSuccess")]
        public IActionResult TestSuccess()
        {
            return Ok();
        }

        [HttpGet("TestError")]
        public IActionResult TestError()
        {
            throw new BaseRequestException("Test exception", "ERR001");
        }

        [HttpGet("TestCompressResponse")]
        public IActionResult TestCompressResponse()
        {
            var list = new List<object>();
            for (int i = 0; i < 10000; i++)
            {
                list.Add(new
                {
                    EmpId = Guid.NewGuid(),
                    Name = $"Employee Name {i}",
                    Address = $"Employee Address {i}",
                });
            }

            return Ok(new { list });
        }
    }
}
