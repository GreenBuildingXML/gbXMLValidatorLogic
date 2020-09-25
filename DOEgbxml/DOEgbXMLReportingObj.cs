using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DOEgbXML
{
    public class DOEgbXMLReportingObj
    {
        //new ReportingObj strings created Feb 13 2013
        public string testSummary;
        public string testReasoning;
      //  public Dictionary<string, bool> globalTestCriteria;
        //original ReportingObj created Jan 1 2013
        public List<string> standResult;
        public List<string> testResult;
        public List<string> idList;
        public double tolerance;
        public double lengthtol;
        public double vectorangletol;
        public double coordtol;
        public TestType testType;
        public int subTestIndex = -1;
        public string unit;
        public Dictionary<string, bool> TestPassedDict;
        public Dictionary<string, OutPutEnum> OutputTypeDict;
        public bool passOrFail;
        public List<string> MessageList;
        public string longMsg;
        public Exception e;
        public OutPutEnum outputType;
     //   public Dictionary<string, List<string>> MatchedSurfaceIds;
     //   public Dictionary<string, List<string>> MatchedOpening;

        public void Clear()
        {
            testSummary = "";
            testReasoning = "";

            if (standResult != null)
                standResult.Clear();

            if (testResult != null)
                testResult.Clear();

            if (idList != null)
                idList.Clear();
            tolerance = DOEgbXMLBasics.Tolerances.ToleranceDefault;
            lengthtol = DOEgbXMLBasics.Tolerances.ToleranceDefault;
            vectorangletol = DOEgbXMLBasics.Tolerances.ToleranceDefault;
            coordtol = DOEgbXMLBasics.Tolerances.ToleranceDefault;
            testType = TestType.None;
            outputType = OutPutEnum.None;
            subTestIndex = -1;
            passOrFail = false;
            if (MessageList != null) { MessageList.Clear(); }
            if (TestPassedDict != null) { TestPassedDict.Clear(); }
            if (OutputTypeDict != null) { OutputTypeDict.Clear(); }
            longMsg = "";

        }

        public DOEgbXMLReportingObj Copy()
        {
            DOEgbXMLReportingObj report = new DOEgbXMLReportingObj();
     
            report.standResult = new List<string>(this.standResult);
            report.testResult = new List<string>(this.testResult);
            report.idList = new List<string>(this.idList);
            report.TestPassedDict = new Dictionary<string, bool>(this.TestPassedDict);
            report.OutputTypeDict = new Dictionary<string, OutPutEnum>(this.OutputTypeDict);
            report.MessageList = new List<string>(this.MessageList);
          //  if (this.MatchedSurfaceIds != null)
          //      report.MatchedSurfaceIds = new Dictionary<string, List<string>>(this.MatchedSurfaceIds);

           
            report.tolerance = this.tolerance;
            report.testType = this.testType;
            report.outputType = this.outputType;
            report.subTestIndex = this.subTestIndex;
            report.unit = this.unit;
            report.passOrFail = this.passOrFail;
            report.longMsg = this.longMsg;

            report.testSummary = this.testSummary;

            return report;
        }

    }

    public class DOEgbXMLPhase2Report
    {
        //new ReportingObj strings created Feb 13 2013
        public string testSummary;
        public string testReasoning;
        //  public Dictionary<string, bool> globalTestCriteria;
        //original ReportingObj created Jan 1 2013
        public List<string> standResult;
        public List<string> testResult;
        public List<string> idList;
        public double tolerance;
        public double lengthtol;
        public double vectorangletol;
        public double coordtol;
        public TestType testType;
        public OutPutEnum outputType;
        public int subTestIndex = -1;
        public string unit;
        public Dictionary<string, bool> TestPassedDict;
        public Dictionary<string, OutPutEnum> OutputTypeDict;
        public bool passOrFail;
        public Dictionary<string,List<string>> MessageList;
        public string longMsg;
        public Exception e;
        //   public Dictionary<string, List<string>> MatchedSurfaceIds;
        //   public Dictionary<string, List<string>> MatchedOpening;

        public void Clear()
        {
            testSummary = "";
            testReasoning = "";

            if (standResult != null)
                standResult.Clear();

            if (testResult != null)
                testResult.Clear();

            if (idList != null)
                idList.Clear();
            tolerance = DOEgbXMLBasics.Tolerances.ToleranceDefault;
            lengthtol = DOEgbXMLBasics.Tolerances.ToleranceDefault;
            vectorangletol = DOEgbXMLBasics.Tolerances.ToleranceDefault;
            coordtol = DOEgbXMLBasics.Tolerances.ToleranceDefault;
            testType = TestType.None;
            outputType = OutPutEnum.None;
            subTestIndex = -1;
            passOrFail = false;
            if (MessageList != null) { MessageList.Clear(); }
            if (TestPassedDict != null) { TestPassedDict.Clear(); }
            if (OutputTypeDict != null) { OutputTypeDict.Clear(); }
            longMsg = "";

        }

        public DOEgbXMLPhase2Report Copy()
        {
            DOEgbXMLPhase2Report report = new DOEgbXMLPhase2Report();

            report.standResult = new List<string>(this.standResult);
            report.testResult = new List<string>(this.testResult);
            report.idList = new List<string>(this.idList);
            report.TestPassedDict = new Dictionary<string, bool>(this.TestPassedDict);
            report.OutputTypeDict = new Dictionary<string, OutPutEnum>(this.OutputTypeDict);
            report.MessageList = new Dictionary<string,List<string>>(this.MessageList);
            //  if (this.MatchedSurfaceIds != null)
            //      report.MatchedSurfaceIds = new Dictionary<string, List<string>>(this.MatchedSurfaceIds);


            report.tolerance = this.tolerance;
            report.testType = this.testType;
            report.outputType = this.outputType;
            report.subTestIndex = this.subTestIndex;
            report.unit = this.unit;
            report.passOrFail = this.passOrFail;
            report.longMsg = this.longMsg;

            report.testSummary = this.testSummary;

            return report;
        }

    }
}