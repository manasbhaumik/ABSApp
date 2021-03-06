﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using APS.BusinessEntity.Models.Authentication;

namespace APS.WebAPI.Models.Authentication
{
    public class PasswordQuestionsResponse
    {
        public int ResponseCode { get; set; }

        public string ResponseMessage { get; set; }

        public IEnumerable<PasswordQuestionsModel> PasswordQuestions { get; set; }
    }


}