using System;
using DOEgbXML;
using System.Xml;

namespace DOEgbxml
{
    internal class Program
    {

        static void Main()
        {
            XMLParser parser = new XMLParser();

            XmlReader reader = XmlReader.Create("/Users/weilixu/Desktop/data/test/test1.gbxml");

            parser.StartTest(reader, "test1", "dummy tester");

            Console.WriteLine(parser.summaryTable);
            Console.WriteLine("End of testing...");

        }
    }
}
