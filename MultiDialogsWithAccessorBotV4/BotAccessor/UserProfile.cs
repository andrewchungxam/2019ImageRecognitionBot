using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimplifiedWaterfallDialogBotV4
{
    /// <summary>
    /// This is our application state. Just a regular serializable .NET class.
    /// </summary>
    public class UserProfile
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Color { get; set; }
        public string Food { get; set;  }

        public string ITName { get; set; }
        public string ITEmail { get; set; }
        public string ITBarcode { get; set; }

        public string PassResetEmail { get; set; }
        public string PassResetMobileNumber { get; set; }
        public string PassResetOTPDevice { get; set; }

        public string PassResetBirthDate { get; set; }


    }
}
