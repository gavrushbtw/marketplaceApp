using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace marketplaceApp
{
    internal class UserSession
    {
        public static int CurrentUserID { get; set; }
        public static string CurrentUserName { get; set; }
        public static string CurrentUserEmail { get; set; }
        public static string CurrentUserRole { get; set; }
    }
}
