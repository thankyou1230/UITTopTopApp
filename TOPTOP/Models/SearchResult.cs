using System;
using System.Collections.Generic;
using System.Text;

namespace TOPTOP.Models
{
    public class SearchResult
    {
        public List<MatchedUserResult> UserResult { get; set; }
        public List<MatchedVideoResult> VideoResult { get; set; }
    }
}
