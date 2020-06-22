using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI4Android.Models
{
    public class TokenModel
    {
        public TokenModel() { }
        public TokenModel(int CheckResult)
        {
            this.CheckResult = CheckResult;
        }
        public TokenModel(int UserID, long TimeStamp, bool isAndroid, bool isFormalDataBase)
        {
            CheckResult = Controllers.BaseController.CheckToken_Success;
            this.UserID = UserID;
            this.TimeStamp = TimeStamp;
            this.isAndroid = isAndroid;
            this.isFormalDataBase = isFormalDataBase;
        }
       

        public int CheckResult { get; set; }
        public int UserID { get; set; }
        public long TimeStamp { get; set; }
        public bool isAndroid { get; set; }
        public bool isFormalDataBase { get; set; }

    }
}