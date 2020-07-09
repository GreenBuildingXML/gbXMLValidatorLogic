using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DOEgbXML
{
    public class gbXMLMatches
    {
        public Dictionary<string, List<string>> MatchedSurfaceIds;
        public Dictionary<string, List<string>> MatchedOpeningIds;

        public void Init()
        {
            MatchedSurfaceIds = new Dictionary<string,List<string>>();
            MatchedOpeningIds = new Dictionary<string,List<string>>();
        }
    }
}