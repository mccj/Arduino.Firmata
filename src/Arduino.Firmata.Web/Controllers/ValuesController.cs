using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arduino.Firmata.Protocol.AccelStepper;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Arduino.Firmata.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET: api/<ValuesController>
        [HttpGet]
        public void StepperMove(int deviceNumber = 0, int speed = 10000, int notSpeed = -200)
        {
            var session = StepperHandler.GetStepperHandler();
            session.StepperMove(deviceNumber, speed, notSpeed);
        }
    }
}
