using System;
using DOEgbXML;
using System.Xml;
using System.Collections.Generic;

namespace DOEgbxml
{
    internal class Program
    {

        static void Main()
        {
            XMLParser parser = new XMLParser();

            XmlReader reader = XmlReader.Create("/Users/weilixu/Desktop/data/test/test1-2.gbxml");

            parser.StartTest(reader, "test1", "dummy tester");

            List<DOEgbXMLReportingObj> reportList = parser.ReportList;
            foreach(DOEgbXMLReportingObj report in reportList)
            {
                List<String> msgList = report.MessageList;

                foreach(String msg in msgList)
                {
                    Console.WriteLine(msg);
                }
            }

           // Console.WriteLine(parser.summaryTable);
            Console.WriteLine("End of testing...");

        }
    }
}
