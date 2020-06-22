using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI4Android.Models
{
    public class VCodeModel
    {
        public int FUserID { get; set; }
        public string FVerificationCode { get; set; }
        public long FDate { get; set; }
    }
}