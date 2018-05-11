/*
 * home
 *
 * The API for the Home Starter project
 *
 * OpenAPI spec version: 1.0.3
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Swashbuckle.AspNetCore.SwaggerGen;
using Newtonsoft.Json;
using IO.Swagger.Attributes;
using IO.Swagger.Models;

namespace IO.Swagger.Controllers
{ 
    /// <summary>
    /// 
    /// </summary>
    public class EnvironmentApiController : Controller
    { 
        /// <summary>
        /// 
        /// </summary>
        
        /// <param name="days"></param>
        /// <response code="200">the forecast</response>
        [HttpGet]
        [Route("/motta/home/1.0.3/temperature/forecast/{days}")]
        [ValidateModelState]
        [SwaggerOperation("GetForecast")]
        [SwaggerResponse(200, typeof(ForecastResponse), "the forecast")]
        public virtual IActionResult GetForecast([FromRoute]int? days)
        { 
            string exampleJson = null;
            
            var example = exampleJson != null
            ? JsonConvert.DeserializeObject<ForecastResponse>(exampleJson)
            : default(ForecastResponse);
            return new ObjectResult(example);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>gets the state of the heater</remarks>
        /// <param name="zoneId"></param>
        /// <response code="200">heater state</response>
        [HttpGet]
        [Route("/motta/home/1.0.3/temperature/{zoneId}/heater")]
        [ValidateModelState]
        [SwaggerOperation("GetHeaterState")]
        [SwaggerResponse(200, typeof(HeaterState), "heater state")]
        public virtual IActionResult GetHeaterState([FromRoute]string zoneId)
        { 
            string exampleJson = null;
            
            var example = exampleJson != null
            ? JsonConvert.DeserializeObject<HeaterState>(exampleJson)
            : default(HeaterState);
            return new ObjectResult(example);
        }

        /// <summary>
        /// 
        /// </summary>
        
        /// <param name="zoneId"></param>
        /// <response code="200">Zone temperature</response>
        [HttpGet]
        [Route("/motta/home/1.0.3/temperature/{zoneId}")]
        [ValidateModelState]
        [SwaggerOperation("GetZoneTemperature")]
        [SwaggerResponse(200, typeof(TemperatureZoneStatus), "Zone temperature")]
        public virtual IActionResult GetZoneTemperature([FromRoute]string zoneId)
        { 
            string exampleJson = null;
            
            var example = exampleJson != null
            ? JsonConvert.DeserializeObject<TemperatureZoneStatus>(exampleJson)
            : default(TemperatureZoneStatus);
            return new ObjectResult(example);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>turns the heater on or off</remarks>
        /// <param name="zoneId"></param>
        /// <param name="state"></param>
        /// <response code="200">Status of the operation</response>
        [HttpPost]
        [Route("/motta/home/1.0.3/temperature/{zoneId}/heater/{state}")]
        [ValidateModelState]
        [SwaggerOperation("SetHeaterState")]
        [SwaggerResponse(200, typeof(ApiResponse), "Status of the operation")]
        public virtual IActionResult SetHeaterState([FromRoute]string zoneId, [FromRoute]string state)
        { 
            string exampleJson = null;
            
            var example = exampleJson != null
            ? JsonConvert.DeserializeObject<ApiResponse>(exampleJson)
            : default(ApiResponse);
            return new ObjectResult(example);
        }

        /// <summary>
        /// 
        /// </summary>
        
        /// <response code="200">ok</response>
        [HttpGet]
        [Route("/motta/home/1.0.3/temperature")]
        [ValidateModelState]
        [SwaggerOperation("TemperatureSummary")]
        [SwaggerResponse(200, typeof(TemperatureSummary), "ok")]
        public virtual IActionResult TemperatureSummary()
        { 
            string exampleJson = null;
            
            var example = exampleJson != null
            ? JsonConvert.DeserializeObject<TemperatureSummary>(exampleJson)
            : default(TemperatureSummary);
            return new ObjectResult(example);
        }
    }
}
