using System;
using System.Collections.Generic;

namespace DOEgbXML
{
    public class DOEgbXMLMaterial
    {
        public string id;
        public string name;
        public string description;
        public Double rvalue; //HrSqaureFtFPerBTU
        public Double thickness; //Inches
        public Double conductivity; //BtuPerHourFtF
        public Double density; //LbsPerCubicFt
        public Double specificheat; //BTUPerLbF
    }

    public class DOEgbXMLLayer
    {
        public string id;
        public List<DOEgbXMLMaterial> materialList;

        public void addMaterialToLayer(DOEgbXMLMaterial material)
        {
            materialList.Add(material);
        }
    }

    public class DOEgbXMLConstruction
    {
        public string id;
        public DOEgbXMLLayer layer;
        public Double uvalue;
        public Double absorptance;
        public string name;
        public string description;
    }
}
