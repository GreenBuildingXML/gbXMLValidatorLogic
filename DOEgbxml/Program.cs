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
            string LocalTestPath = "/Users/weilixu/Desktop/data/test/";
            List<string> testList = new List<string>()
            {
                "test1",
                "test2",
                "test3",
                "test4",
                "test5",
                "test6",
                "test7",
                "test8",
                "test9",
                "test10",
                "test11",
                "test12",
                "test13",
                "test14",
                "test15",
                "test16",
                "test17",
                "test18",
                "test19"
            };


            foreach (string s in testList)
            {
                string path = LocalTestPath + s + ".gbxml";
                XmlReader reader = XmlReader.Create(path);
                XMLParser parser = new XMLParser();
                parser.StartTest(reader, s, "dummy tester");

                if (s == "test1")
                {
                    if (parser.successCounter != 18)
                    {
                        Console.WriteLine("Test 1 does not complete the test - success criteria is less than 18");
                    }
                    else
                    {
                        Console.WriteLine("Test 1 completed all 18 test critera");
                    }

                    if (!parser.overallPassTest)
                    {
                        Console.WriteLine("Test 1 failed the test");

                    }
                    else
                    {
                        Console.WriteLine("Test 1 pass the test");
                    }
                }
                else if (s == "test2")
                {
                    if (parser.successCounter != 23)
                    {
                        Console.WriteLine("Test 2 does not complete the test - success criteria is less than 23");
                    }
                    else
                    {
                        Console.WriteLine("Test 2 completed all 23 test critera");
                    }

                    if (!parser.overallPassTest)
                    {
                        Console.WriteLine("Test 2 failed the test");

                    }
                    else
                    {
                        Console.WriteLine("Test 2 pass the test");
                    }
                }
                else if (s == "test3")
                {
                    if (parser.successCounter != 19)
                    {
                        Console.WriteLine("Test 3 does not complete the test - success criteria is less than 19");
                    }
                    else
                    {
                        Console.WriteLine("Test 3 completed all 19 test critera");
                    }

                    if (!parser.overallPassTest)
                    {
                        Console.WriteLine("Test 3 failed the test");

                    }
                    else
                    {
                        Console.WriteLine("Test 3 pass the test");
                    }
                }
                else if (s == "test4")
                {
                    if (parser.successCounter != 30)
                    {
                        Console.WriteLine("Test 4 does not complete the test - success criteria is less than 30");
                    }
                    else
                    {
                        Console.WriteLine("Test 4 completed all 30 test critera");
                    }

                    if (!parser.overallPassTest)
                    {
                        Console.WriteLine("Test 4 failed the test");

                    }
                    else
                    {
                        Console.WriteLine("Test 4 pass the test");
                    }
                }
                else if (s == "test5")
                {
                    if (parser.successCounter != 35)
                    {
                        Console.WriteLine("Test 5 does not complete the test - success criteria is less than 35");
                    }
                    else
                    {
                        Console.WriteLine("Test 5 completed all 35 test critera");
                    }

                    if (!parser.overallPassTest)
                    {
                        Console.WriteLine("Test 5 failed the test");

                    }
                    else
                    {
                        Console.WriteLine("Test 5 pass the test");
                    }
                }
                else if (s == "test6")
                {
                    if (parser.successCounter != 18)
                    {
                        Console.WriteLine("Test 6 does not complete the test - success criteria is less than 18");
                    }
                    else
                    {
                        Console.WriteLine("Test 6 completed all 18 test critera");
                    }

                    if (!parser.overallPassTest)
                    {
                        Console.WriteLine("Test 6 failed the test");

                    }
                    else
                    {
                        Console.WriteLine("Test 6 pass the test");
                    }
                }
                else if (s == "test7")
                {
                    if (parser.successCounter != 16)
                    {
                        Console.WriteLine("Test 7 does not complete the test - success criteria is less than 16");
                    }
                    else
                    {
                        Console.WriteLine("Test 7 completed all 16 test critera");
                    }

                    if (!parser.overallPassTest)
                    {
                        Console.WriteLine("Test 7 failed the test");

                    }
                    else
                    {
                        Console.WriteLine("Test 7 pass the test");
                    }
                }
                else if (s == "test8")
                {
                    if (parser.successCounter != 25)
                    {
                        Console.WriteLine("Test 8 does not complete the test - success criteria is less than 25");
                    }
                    else
                    {
                        Console.WriteLine("Test 8 completed all 25 test critera");
                    }

                    if (!parser.overallPassTest)
                    {
                        Console.WriteLine("Test 8 failed the test");

                    }
                    else
                    {
                        Console.WriteLine("Test 8 pass the test");
                    }
                }
                else if (s == "test9")
                {
                    if (parser.successCounter != 19)
                    {
                        Console.WriteLine("Test 9 does not complete the test - success criteria is less than 19");
                    }
                    else
                    {
                        Console.WriteLine("Test 9 completed all 19 test critera");
                    }

                    if (!parser.overallPassTest)
                    {
                        Console.WriteLine("Test 9 failed the test");

                    }
                    else
                    {
                        Console.WriteLine("Test 9 pass the test");
                    }
                }
                else if (s == "test10")
                {
                    if (parser.successCounter != 28)
                    {
                        Console.WriteLine("Test 10 does not complete the test - success criteria is less than 28");
                    }
                    else
                    {
                        Console.WriteLine("Test 10 completed all 28 test critera");
                    }

                    if (!parser.overallPassTest)
                    {
                        Console.WriteLine("Test 10 failed the test");

                    }
                    else
                    {
                        Console.WriteLine("Test 10 pass the test");
                    }
                }
                else if (s == "test11")
                {
                    if (parser.successCounter != 11)
                    {
                        Console.WriteLine("Test 11 does not complete the test - success criteria is less than 11");
                    }
                    else
                    {
                        Console.WriteLine("Test 11 completed all 11 test critera");
                    }

                    if (!parser.overallPassTest)
                    {
                        Console.WriteLine("Test 11 failed the test");

                    }
                    else
                    {
                        Console.WriteLine("Test 11 pass the test");
                    }
                }
                else if (s == "test12")
                {
                    if (parser.successCounter != 18)
                    {
                        Console.WriteLine("Test 12 does not complete the test - success criteria is less than 18");
                    }
                    else
                    {
                        Console.WriteLine("Test 12 completed all 18 test critera");
                    }

                    if (!parser.overallPassTest)
                    {
                        Console.WriteLine("Test 12 failed the test");

                    }
                    else
                    {
                        Console.WriteLine("Test 12 pass the test");
                    }
                }
                else if (s == "test13")
                {
                    if (parser.successCounter != 12)
                    {
                        Console.WriteLine("Test 13 does not complete the test - success criteria is less than 12");
                    }
                    else
                    {
                        Console.WriteLine("Test 13 completed all 12 test critera");
                    }

                    if (!parser.overallPassTest)
                    {
                        Console.WriteLine("Test 13 failed the test");

                    }
                    else
                    {
                        Console.WriteLine("Test 13 pass the test");
                    }
                }
                else if (s == "test14")
                {
                    if (parser.successCounter != 10)
                    {
                        Console.WriteLine("Test 14 does not complete the test - success criteria is less than 10");
                    }
                    else
                    {
                        Console.WriteLine("Test 14 completed all 10 test critera");
                    }

                    if (!parser.overallPassTest)
                    {
                        Console.WriteLine("Test 14 failed the test");

                    }
                    else
                    {
                        Console.WriteLine("Test 14 pass the test");
                    }
                }
                else if (s == "test15")
                {
                    if (parser.successCounter != 17)
                    {
                        Console.WriteLine("Test 15 does not complete the test - success criteria is less than 17");
                    }
                    else
                    {
                        Console.WriteLine("Test 15 completed all 17 test critera");
                    }

                    if (!parser.overallPassTest)
                    {
                        Console.WriteLine("Test 15 failed the test");

                    }
                    else
                    {
                        Console.WriteLine("Test 15 pass the test");
                    }
                }
                else if (s == "test16")
                {
                    if (parser.successCounter != 13)
                    {
                        Console.WriteLine("Test 16 does not complete the test - success criteria is less than 13");
                    }
                    else
                    {
                        Console.WriteLine("Test 16 completed all 13 test critera");
                    }

                    if (!parser.overallPassTest)
                    {
                        Console.WriteLine("Test 16 failed the test");

                    }
                    else
                    {
                        Console.WriteLine("Test 16 pass the test");
                    }
                }
                else if (s == "test17")
                {
                    if (parser.successCounter != 10)
                    {
                        Console.WriteLine("Test 17 does not complete the test - success criteria is less than 10");
                    }
                    else
                    {
                        Console.WriteLine("Test 17 completed all 10 test critera");
                        Console.WriteLine(parser.output);
                    }

                    if (!parser.overallPassTest)
                    {
                        Console.WriteLine("Test 17 failed the test");

                    }
                    else
                    {
                        Console.WriteLine("Test 17 pass the test");
                    }
                }
                else if (s == "test18")
                {
                    if (parser.successCounter != 15)
                    {
                        Console.WriteLine("Test 18 does not complete the test - success criteria is less than 15");
                    }
                    else
                    {
                        Console.WriteLine("Test 18 completed all 15 test critera");
                    }

                    if (!parser.overallPassTest)
                    {
                        Console.WriteLine("Test 18 failed the test");

                    }
                    else
                    {
                        Console.WriteLine("Test 18 pass the test");
                    }
                }
                else if (s == "test19")
                {
                    if (parser.successCounter != 14)
                    {
                        Console.WriteLine("Test 19 does not complete the test - success criteria is less than 14");
                    }
                    else
                    {
                        Console.WriteLine("Test 19 completed all 14 test critera");
                    }

                    if (!parser.overallPassTest)
                    {
                        Console.WriteLine("Test 19 failed the test");

                    }
                    else
                    {
                        Console.WriteLine("Test 19 pass the test");
                    }
                }
            }
        }
    }
}
