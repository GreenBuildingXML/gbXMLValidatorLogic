using System;
using System.Collections.Generic;
using System.Xml;

namespace DOEgbXML
{
    public class DOEgbXMLPTAC
    {
        public double DesignOAFlowPerPerson; //CFM
        public double DesignOAFlowPerArea; //CFMPerSquareFoot
        public double DesignHeatTemp; //F
        public double DesignCoolTemp; //F

        //air loop
        public DOEgbXMLFan fan;
        public DOEgbXMLCoil CoolCoil;
        public DOEgbXMLCoil HeatCoil;
        public string OperationScheduleId;
        public List<string> errorMessageList;

        public DOEgbXMLPTAC(XmlDocument xmldoc, XmlNamespaceManager xmlns)
        {
            //1. Get design information
            errorMessageList = new List<string>();
            XmlNodeList nodes = xmldoc.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Zone", xmlns);
            //only one zone allowed in the model
            if(nodes.Count > 0)
            {
                XmlNode ZoneNode = nodes[0];
                XmlNodeList ZoneChildren = ZoneNode.ChildNodes;
                foreach(XmlNode nd in ZoneChildren)
                {
                    if(nd.Name == "OAFlowPerPerson")
                    {
                        DesignOAFlowPerPerson = Convert.ToDouble(nd.InnerText);
                    }else if(nd.Name == "OAFlowPerArea")
                    {
                        DesignOAFlowPerArea = Convert.ToDouble(nd.InnerText);
                    }else if(nd.Name == "DesignHeatT")
                    {
                        DesignHeatTemp = Convert.ToDouble(nd.InnerText);
                    }else if(nd.Name == "DesignCoolT")
                    {
                        DesignCoolTemp = Convert.ToDouble(nd.InnerText);
                    }

                }
            }
            else
            {
                DesignOAFlowPerPerson = 0.0;
                DesignOAFlowPerArea = 0.0;
                DesignHeatTemp = 0.0;
                DesignCoolTemp = 0.0;
            }

            //2. Get air loop information
            nodes = xmldoc.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:AirLoop", xmlns);
            if(nodes.Count > 0)
            {
                XmlNode AirLoopNode = nodes[0];
                XmlNodeList AirLoopNodeList = AirLoopNode.ChildNodes;
                foreach(XmlNode nd in AirLoopNodeList)
                {
                    if(nd.Name == "AirLoopEquipment")
                    {
                        XmlAttributeCollection attributes = nd.Attributes;
                        foreach (XmlAttribute attr in attributes)
                        {
                            if (attr.Name == "equipmentType")
                            {
                                string equipment = attr.Value;
                                if(equipment == "Fan")
                                {
                                    fan = new DOEgbXMLFan(nd);
                                    OperationScheduleId = fan.OperationScheduleId;
                                }else if(equipment == "Coil")
                                {
                                    DOEgbXMLCoil tempCoil = new DOEgbXMLCoil(nd);
                                    if(tempCoil.CoilType == "Heat")
                                    {
                                        HeatCoil = tempCoil;
                                    }
                                    else
                                    {
                                        CoolCoil = tempCoil;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public class DOEgbXMLFan
    {
        public double MotorInStream;
        public double AirStreamFraction;
        public double DeltaP;
        public string FanControl;
        public double FanEff;
        public string OperationScheduleId;

        public DOEgbXMLFan(XmlNode Fan)
        {
            if (Fan.HasChildNodes)
            {
                XmlNodeList children = Fan.ChildNodes;

                foreach(XmlNode nd in children)
                {
                    if(nd.Name == "MotorInAirstream")
                    {
                        MotorInStream = Convert.ToDouble(nd.InnerText);
                    }else if(nd.Name == "AirStreamFraction")
                    {
                        AirStreamFraction = Convert.ToDouble(nd.InnerText);
                    }else if(nd.Name == "DeltaP")
                    {
                        DeltaP = Convert.ToDouble(nd.InnerText);
                    }else if(nd.Name == "Efficiency")
                    {
                        FanEff = Convert.ToDouble(nd.InnerText);
                    }else if (nd.Name == "OperationSchedule")
                    {
                        XmlAttributeCollection attributes = nd.Attributes;
                        foreach (XmlAttribute attr in attributes)
                        {
                            if (attr.Name == "scheduleIdRef")
                            {
                                OperationScheduleId = attr.Value;
                            }
                        }
                    }
                    else if(nd.Name == "Control")
                    {
                        XmlAttributeCollection attributes = nd.Attributes;
                        foreach(XmlAttribute attr in attributes)
                        {
                            if(attr.Name == "operationType")
                            {
                                FanControl = attr.Value;
                            }
                        }
                    }
                }
            }
            else
            {
                MotorInStream = 0.0;
                AirStreamFraction = 0.0;
                DeltaP = 0.0;
                FanEff = 0.0;
                FanControl = "None";
            }
        }
    }

    public class DOEgbXMLCoil
    {
        public double Capacity;
        public double CoilEff;
        public string ResourceType;
        public string CoilType; //"Heat" or "Cool"

        public DOEgbXMLCoil(XmlNode Coil)
        {
            if (Coil.HasChildNodes)
            {
                XmlNodeList children = Coil.ChildNodes;

                foreach (XmlNode nd in children)
                {
                    if (nd.Name == "Efficiency")
                    {
                        CoilEff = Convert.ToDouble(nd.InnerText);
                    }
                    else if (nd.Name == "Capacity")
                    {
                        Capacity = Convert.ToDouble(nd.InnerText);

                        XmlAttributeCollection attributes = nd.Attributes;
                        foreach (XmlAttribute attr in attributes)
                        {
                            if (attr.Name == "capacityType")
                            {
                                if(attr.Value == "Heating")
                                {
                                    CoilType = "Heat";
                                }
                                else
                                {
                                    CoilType = "Cool";
                                }
                            }
                        }

                    }
                    else if(nd.Name == "Energy")
                    {
                        XmlAttributeCollection attributes = nd.Attributes;
                        foreach (XmlAttribute attr in attributes)
                        {
                            if (attr.Name == "resourceType")
                            {
                                ResourceType = attr.Value;
                            }
                        }
                    }
                }
            }
            else
            {
                CoilEff = 0.0;
                Capacity = 0.0;
                ResourceType = "N/A";
            }
        }
    }

}
