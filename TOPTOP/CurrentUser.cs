using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace TOPTOP
{
    public class CurrentUser
    {
       [PrimaryKey]
       public string email { get; set; }
    }
}
