using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DOEgbXML
{
    public class DOEgbXMLTestDetail
    {
        public string testName;
        public string testSummary;
        public string passString;
        public string failString;
        public string shortTitle;
        //holds a bunch of strings for a given test
        //this list will have this format:  TestShortTitle, Pass String, Fail String, Summary String
      //  public List<string> testString = new List<string>();

        //this List will store all the test Detail
        public List<DOEgbXMLTestDetail> TestDetailList;

        public void InitializeTestResultStrings()
        {
            //holds the Detail object for all the tests
            TestDetailList = new List<DOEgbXMLTestDetail>();

            //get the strings for the summary page table
            //initialize the testdetails for all the tests
            DOEgbXMLTestDetail test1detail = new DOEgbXMLTestDetail();
            DOEgbXMLTestDetail test2detail = new DOEgbXMLTestDetail();
            DOEgbXMLTestDetail test3detail = new DOEgbXMLTestDetail();
            DOEgbXMLTestDetail test4detail = new DOEgbXMLTestDetail();
            DOEgbXMLTestDetail test5detail = new DOEgbXMLTestDetail();
            DOEgbXMLTestDetail test7detail = new DOEgbXMLTestDetail();
            DOEgbXMLTestDetail test8detail = new DOEgbXMLTestDetail();
            DOEgbXMLTestDetail test25detail = new DOEgbXMLTestDetail();
            DOEgbXMLTestDetail test28detail = new DOEgbXMLTestDetail();
            DOEgbXMLTestDetail genericphase2 = new DOEgbXMLTestDetail();
            //create the strings
            //Test1
            test1detail.testName = "Test1";
            test1detail.shortTitle = "2 Walls of Different Thicknesses with Parallel Aligned Faces";
            test1detail.testSummary = "This test is designed to make sure that when walls of different thicknesses are joined with their faces aligned, that the centerline offset does not create extra walls during the gbXML creation process.  If these extra sliver walls are found in the gbXML file, this test will fail.";
            test1detail.passString = "This test has passed.";
            test1detail.failString = "This test has failed.";
            //ADD list to local testStrings List of Lists
            TestDetailList.Add(test1detail);

            //test 2
            test2detail.testName = "Test2";
            test2detail.shortTitle = "Single window with overhang that bisects the window's height.";
            test2detail.testSummary = "A 1-zone, one story, simple model with exterior shading devices that act as overhangs and exterior light shelves for windows on the south façade.  Light shelves are 1” thick and split a single window instance in the BIM along its centerline.  This test is designed to ensure that this window should be represented as two windows in gbXML, the one window that is above the overhang, and the other that is below.";
            test2detail.passString = "This test has passed.";
            test2detail.failString = "This test has failed.";
            //ADD list to local testStrings List of Lists
            TestDetailList.Add(test2detail);

            //test 3
            test3detail.testName = "Test3";
            test3detail.shortTitle = "Interior walls and Floor Second Level Space Boundary Test  ";
            test3detail.testSummary = "A 5-zone model with overlapping zones and a double-height zone.  This test is designed to ensure that the tool used to create the zones can properly follow the basic conventions for second level space boundaries.";
            test3detail.passString = "This test has passed.";
            test3detail.failString = "This test has failed.";
            //ADD list to local testStrings List of Lists
            TestDetailList.Add(test3detail);

            //test 4 
            test4detail.testName = "Test4";
            test4detail.shortTitle = "Double height space with hole cut in floor and a skylight";
            test4detail.testSummary = "This test is a large open atrium with a hole cut in the floor to allow light to penetrate through to the floor below.";
            test4detail.passString = "This test has passed.";
            test4detail.failString = "This test has failed.";
            //ADD list to local testStrings List of Lists
            TestDetailList.Add(test4detail);

            //test 5 
            test5detail.testName = "Test5";
            test5detail.shortTitle = "Basement walls that extend above grade and bound two different spaces";
            test5detail.testSummary = "A two zone model that ensures exterior walls can properly be defined as underground and above grade.  A single wall has been drawn by the user that begins below grade, and terminates above grade.  Above grade, the walls bound a space that is above grade.  Below grade, the walls bound a space that is entirely below grade.";
            test5detail.passString = "This test has passed.";
            test5detail.failString = "This test has failed.";
            //ADD list to local testStrings List of Lists
            TestDetailList.Add(test5detail);

            //test 7 
            test7detail.testName = "Test7";
            test7detail.shortTitle = "Folded roof element.";
            test7detail.testSummary = "This is the first in a proposed series of tests that focus on roof elements that grow in geometric complexity.";
            test7detail.passString = "This test has passed.";
            test7detail.failString = "This test has failed.";
            //ADD list to local testStrings List of Lists
            TestDetailList.Add(test7detail);

            //test 8 
            test8detail.testName = "Test8";
            test8detail.shortTitle = "Sloping slab on grade";
            test8detail.testSummary = "Ensures that sloping slab on grade comes through properly in gbXML, and that walls, which terminate at grade, are turned into the appropriate surfaceType (\"UndergroundWall\")";
            test8detail.passString = "This test has passed.";
            test8detail.failString = "This test has failed.";
            //ADD list to local testStrings List of Lists
            TestDetailList.Add(test8detail);

            //test 25
            test25detail.testName = "Test25";
            test25detail.shortTitle = "Stacked interior walls with openings";
            test25detail.testSummary = "A simplified 4-zone model of a building that has interior walls stacked on top of one another.  The interior walls each have openings cut into them, to simulate something that may be drawn as a hallway by a designer.";
            test25detail.passString = "This test has passed.";
            test25detail.failString = "This test has failed.";
            //ADD list to local testStrings List of Lists
            TestDetailList.Add(test25detail);

            //test 28
            test28detail.testName = "Test28";
            test28detail.shortTitle = "Roof eaves are turned into shading devices automatically";
            test28detail.testSummary = "A simplified 3-zone model of a building shaped like a residential home has been created.  The home is a simple two story example that has a small attic formed by a roof with a 30 degree pitch which slopes along one of the site’s Cartesian axes.  This test is a simple test that ensures the authoring tool is able to automatically break the roof into a space bounding object and shade object appropriately without any user intervention.";
            test28detail.passString = "This test has passed.";
            test28detail.failString = "This test has failed.";
            //ADD list to local testStrings List of Lists
            TestDetailList.Add(test28detail);

            //Generic Phase 2 Test
            genericphase2.testName = "GenericPhase2";
            genericphase2.shortTitle = "A validation tool for any gbXML geometry.";
            genericphase2.testSummary = "gbXML has recently released this validation tool for you to test any of your gbXML files.  Simply upload your gbXML file, choose the schema (most recent is 5.12) and hit the green button to generate your report, and view the results by browsing.";
            genericphase2.passString = "This test has passed.";
            genericphase2.failString = "This test has failed.";
            //ADD list to local testStrings List of Lists
            TestDetailList.Add(genericphase2);


        }
    }

}