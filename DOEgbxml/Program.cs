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

            XmlReader reader = XmlReader.Create("/Users/weilixu/Desktop/data/test/test17.gbxml");

            parser.StartTest(reader, "test17", "dummy tester");

            Console.WriteLine("How is the overall performance? " + parser.overallPassTest);
            Console.WriteLine(parser.output);
            Console.WriteLine(parser.failCounter);

            //List<DOEgbXMLReportingObj> reportList = parser.ReportList;
            //foreach(DOEgbXMLReportingObj report in reportList)
            //{
            //    List<String> msgList = report.MessageList;

            //    foreach(String msg in msgList)
            //    {
            //        Console.WriteLine(msg);
            //    }
            //}

            //Console.WriteLine(parser.output);
            Console.WriteLine("End of testing...");
        }
    }
}
