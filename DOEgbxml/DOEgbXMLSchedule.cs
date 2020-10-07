using System;
using System.Collections.Generic;
using System.Xml;


namespace DOEgbXML
{
    /**
     * 
     * This class is set up for RP1810 related HVAC checking. It is a simplified gbXML schedule reader that interprets
     * the annually schedule. Days such as DesignHeatingDay, Custom, Holiday, will need further extension of this class.
     * 
     */
    public class DOEgbXMLSchedule
    {
        public Dictionary<string, string> SchedTypeMap;
        public Dictionary<string, List<double>> SchedValueMap;
        private Dictionary<string, XmlNode> WeekSchedMap;
        private Dictionary<string, XmlNode> DaySchedMap;
        public Dictionary<string, string> ErrorMessage;

        public DOEgbXMLSchedule(XmlDocument xmldoc, XmlNamespaceManager xmlns)
        {
            //0. Initialize all the empty field
            SchedTypeMap = new Dictionary<string, string>();
            SchedValueMap = new Dictionary<string, List<double>>();
            WeekSchedMap = new Dictionary<string, XmlNode>();
            DaySchedMap = new Dictionary<string, XmlNode>();
            ErrorMessage = new Dictionary<string, string>();

            //1. Get schedule information
            XmlNodeList WeekSchedList = xmldoc.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:WeekSchedule", xmlns);
            XmlNodeList DaySchedList = xmldoc.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:DaySchedule", xmlns);

            foreach (XmlNode node in WeekSchedList)
            {
                string id = SearchAnAttribute(node, "id");
                if(id != null)
                {
                    WeekSchedMap.Add(id, node);
                }
                else
                {
                    ErrorMessage.Add("Error", "One of the week schedules have no ID, invalid gbXML model, process ceased.");
                    return;//stop the process
                }
            }

            foreach(XmlNode node in DaySchedList)
            {
                string id = SearchAnAttribute(node, "id");
                if (id != null)
                {
                    DaySchedMap.Add(id, node);
                }
                else
                {
                    ErrorMessage.Add("Error", "One of the day schedules have no ID, invalid gbXML model, process ceased.");
                    return; //stop the process

                }
            }


            XmlNodeList nodes = xmldoc.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Schedule", xmlns);
            if(nodes.Count > 0)
            {
                foreach (XmlNode node in nodes)
                {
                    if (!processSchedule(node))
                    {
                        return;
                    }
                    
                }
            }
        }

        private Boolean processSchedule(XmlNode SchedNode)
        {
            //Step 1. Set up the id for both dictionary
            string id = SearchAnAttribute(SchedNode, "id");
            if (id != null)
            {
                SchedTypeMap.Add(id, null);
                SchedValueMap.Add(id, new List<double>());
            }
            else
            {
                ErrorMessage.Add("Error", "One of the schedules has no ID, invalid gbXML model, process ceased.");
                return false;
            }

            //insert the type
            string type = SearchAnAttribute(SchedNode, "type");
            if (type != null)
            {
                SchedTypeMap[id] = type;
            }
            else
            {
                ErrorMessage.Add("Error", "Schedule ID: <a class='"+id+"'>" + id + "</a> is missing schedule type, invalid gbXML model, process ceased.");
                return false;
            }

            //Step 2. Loop through the week schedule in the year schedule
            if (SchedNode.HasChildNodes)
            {
                XmlNodeList YearSchedList = SchedNode.ChildNodes;
                if(YearSchedList.Count == 0)
                {
                    ErrorMessage.Add("Error", "Schedule ID: <a class='" + id + "'>" + id + "</a> has no year schedule, invalid gbXML model, process ceased.");
                    return false;
                }

                foreach(XmlNode YearSched in YearSchedList)
                {
                    string YearSchedID = SearchAnAttribute(YearSched, "id");
                    if (YearSchedID == null)
                    {
                        ErrorMessage.Add("Error", "One of the year schedules has no ID, invalid gbXML model, process ceased.");
                        return false;
                    }

                    if (YearSched.HasChildNodes)
                    {
                        XmlNodeList YearSchedChildren = YearSched.ChildNodes;
                        //this is a fixed format - BeginDate, EndDate and WeekScheduleId, repeat
                        //We need to check if this year schedule compliant with the ruleset.
                        if (YearSchedChildren.Count % 3 != 0)
                        {
                            ErrorMessage.Add("Error", "Schedule ID: <a class='" + YearSchedID + "'>" + YearSchedID + "</a> has incomplete elements. The element list should strictly follow: BeginDate, EndDate, WeekScheduleId. Invalid gbXML model, process ceased.");
                            return false;
                        }

                        for (int i = 0; i < YearSchedChildren.Count; i += 3)
                        {
                            DateTime BeginDate = DateTime.Parse(YearSchedChildren[i + 0].InnerText);//begining date
                            DateTime EndDate = DateTime.Parse(YearSchedChildren[i + 1].InnerText);

                            //Get week Id,
                            string WeekId = SearchAnAttribute(YearSchedChildren[i + 2], "weekScheduleIdRef");

                            XmlNode WeekNode = WeekSchedMap[WeekId];
                            //set up the week day map
                            Dictionary<string, string> WeekDayMap = null;

                            XmlNodeList DayList = WeekNode.ChildNodes;
                            //Fill up the WeekDayMap
                            if (DayList.Count > 0)
                            {
                                XmlNode DayNode = DayList[0];
                                string DefaultDayScheduleID = SearchAnAttribute(DayNode, "dayScheduleIdRef");

                                WeekDayMap = InitializeWeekDayMap(DefaultDayScheduleID);
                                //Loop over the week element to collect days
                                foreach (XmlNode day in DayList)
                                {
                                    if (day.Name == "Day")
                                    {
                                        string DayType = SearchAnAttribute(day, "dayType");
                                        string dayScheduleIdRef = SearchAnAttribute(day, "dayScheduleIdRef");
                                        if (DayType == "All")
                                        {
                                            WeekDayMap["Monday"] = dayScheduleIdRef;
                                            WeekDayMap["Tuesday"] = dayScheduleIdRef;
                                            WeekDayMap["Wednesday"] = dayScheduleIdRef;
                                            WeekDayMap["Thursday"] = dayScheduleIdRef;
                                            WeekDayMap["Friday"] = dayScheduleIdRef;
                                            WeekDayMap["Saturday"] = dayScheduleIdRef;
                                            WeekDayMap["Sunday"] = dayScheduleIdRef;
                                        }
                                        else if (DayType == "Weekend" || DayType == "WeekendOrHoliday")
                                        {
                                            WeekDayMap["Saturday"] = dayScheduleIdRef;
                                            WeekDayMap["Sunday"] = dayScheduleIdRef;
                                        }
                                        else if (DayType == "Weekday")
                                        {
                                            WeekDayMap["Monday"] = dayScheduleIdRef;
                                            WeekDayMap["Tuesday"] = dayScheduleIdRef;
                                            WeekDayMap["Wednesday"] = dayScheduleIdRef;
                                            WeekDayMap["Thursday"] = dayScheduleIdRef;
                                            WeekDayMap["Friday"] = dayScheduleIdRef;
                                        }
                                        else if (DayType == "Mon")
                                        {
                                            WeekDayMap["Monday"] = dayScheduleIdRef;

                                        }
                                        else if (DayType == "Tue")
                                        {
                                            WeekDayMap["Tuesday"] = dayScheduleIdRef;
                                        }
                                        else if (DayType == "Wed")
                                        {
                                            WeekDayMap["Wednesday"] = dayScheduleIdRef;
                                        }
                                        else if (DayType == "Thu")
                                        {
                                            WeekDayMap["Thursday"] = dayScheduleIdRef;
                                        }
                                        else if (DayType == "Fri")
                                        {
                                            WeekDayMap["Friday"] = dayScheduleIdRef;
                                        }
                                        else if (DayType == "Sat")
                                        {
                                            WeekDayMap["Saturday"] = dayScheduleIdRef;
                                        }
                                        else if (DayType == "Sun")
                                        {
                                            WeekDayMap["Sunday"] = dayScheduleIdRef;
                                        }
                                    }
                                }
                            }

                            //Step 3: Loop the days in the week from begin date to end date to fill out the data
                            foreach (DateTime day in EachDay(BeginDate, EndDate))
                            {
                                //fill in the data for everyday from begining to end.
                                string DaySchedule = WeekDayMap[day.DayOfWeek.ToString()];

                                XmlNode DayNode = DaySchedMap[DaySchedule];
                                string DaySchedId = SearchAnAttribute(DayNode, "id");
                                if (DayNode.HasChildNodes)
                                {
                                    XmlNodeList SchedValueList = DayNode.ChildNodes;
                                    int Counter = 0;
                                    for(int j=0; j<SchedValueList.Count; j++)
                                    {
                                        if(SchedValueList[j].Name == "ScheduleValue")
                                        {
                                            Counter++;
                                        }
                                    }
                                    //one day has 24 hours. We will skip the rest schedule value
                                    //if 24%Count > 0;
                                    int interval = (int)(24 / Counter);

                                    foreach (XmlNode SchedValue in SchedValueList)
                                    {
                                        if(SchedValue.Name == "ScheduleValue")
                                        {
                                            for (int j = 0; j < interval; j++)
                                            {
                                                SchedValueMap[id].Add(Convert.ToDouble(SchedValue.InnerText));
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    ErrorMessage.Add("Error", "Schedule ID: <a class='" + DaySchedId + "'>" + DaySchedId + "</a> has no schedule value, invalid gbXML model, process ceased.");
                                    return false;
                                }
                            }
                        }
                    }
                } 
            }
            return true;
        }

        private IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }


        private Dictionary<string, string> InitializeWeekDayMap(string DefaultDayScheduleID)
        {
            Dictionary<string, string> WeekDayMap = new Dictionary<string, string>();
            WeekDayMap.Add("Monday", DefaultDayScheduleID);
            WeekDayMap.Add("Tuesday", DefaultDayScheduleID);
            WeekDayMap.Add("Wedesday", DefaultDayScheduleID);
            WeekDayMap.Add("Thursday", DefaultDayScheduleID);
            WeekDayMap.Add("Friday", DefaultDayScheduleID);
            WeekDayMap.Add("Saturday", DefaultDayScheduleID);
            WeekDayMap.Add("Sunday", DefaultDayScheduleID);
            return WeekDayMap;
        }

        private string SearchAnAttribute(XmlNode node, string Name)
        {
            XmlAttributeCollection attributes = node.Attributes;

            foreach (XmlAttribute attr in attributes)
            {
                if (attr.Name == Name)
                {
                    return attr.Value;
                }
            }
            return null;
        }
    }
}
