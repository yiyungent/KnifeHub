// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAnalysisPlugin.Models;
using Microsoft.AspNetCore.Mvc;

namespace DataAnalysisPlugin.Controllers
{
    [ApiController]
    [Route($"Plugins/{(nameof(DataAnalysisPlugin))}/[controller]/[action]")]
    public class SimpleTimeTrackerController : ControllerBase
    {
        [HttpPost]
        public async Task<BaseResponseModel> Upload()
        {
            BaseResponseModel responseModel = new BaseResponseModel();
            try
            {



                responseModel.Code = 1;
                responseModel.Message = "success";
            }
            catch (Exception ex)
            {
                responseModel.Code = -1;
                responseModel.Message = "failure";
            }

            return responseModel;
        }
    }
}
