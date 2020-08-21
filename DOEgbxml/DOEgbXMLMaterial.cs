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

        //when using this function, the precondition is this object is from standard model,
        //the argument DOEgbXMLMaterial shall be from test model (user uploaded)
        public Boolean compare(DOEgbXMLMaterial material, List<String> message)
        {
            //No need to exam the standard model - we already ensure these edge cases will not exist
            if (this.rvalue == 0.0d)
            {
                //convert inches to ft
                this.rvalue = (this.thickness / 12) / this.conductivity;
            }

            //rule out the inadequate date - if both rvalue and conductivity are not available
            //then the material is not correctly defined
            if (material.rvalue == 0.0d)
            {
                if(material.conductivity == 0.0d && material.thickness == 0.0d)
                {
                    message.Add("Test model material: " + id + "/" + name + "Rvalue, conductivity and thickness are not correctly defined.");
                    return false;
                }
                else
                {
                    material.rvalue = (material.thickness / 12) / material.conductivity;
                }
            }

            //we do not need to compare id, name, and description
            if(Math.Abs(this.rvalue - material.rvalue) > DOEgbXMLBasics.Tolerances.RVALUE)
            {
                // r-value does not match
                message.Add("Test model material: " + id + "/" + name + " Rvalue (" + material.rvalue + ")"+
                    "does not match Standard File, the difference was not within tolerance = " + DOEgbXMLBasics.Tolerances.RVALUE +
                    " (Standard: " + this.rvalue + ")");
                return false;
            }

            if(Math.Abs(this.specificheat - material.specificheat) > DOEgbXMLBasics.Tolerances.SPECIFICHEAT)
            {
                // specificheat does not match
                message.Add("Test model material: " + id + "/" + name + "Specific Heat (" + material.specificheat + ")" +
                    "does not match Standard File, the difference was not within tolerance = " + DOEgbXMLBasics.Tolerances.SPECIFICHEAT +
                    " (Standard: " + this.specificheat + ")");
                return false;
            }

            return true;
        }
    }

    public class DOEgbXMLLayer
    {
        public string id;
        public List<DOEgbXMLMaterial> materialList = new List<DOEgbXMLMaterial>();

        public void addMaterialToLayer(DOEgbXMLMaterial material)
        {
            materialList.Add(material);
        }

        //when using this function, the precondition is this object is from standard model,
        //the argument DOEgbXMLLayer shall be from test model (user uploaded)
        public Boolean compare(DOEgbXMLLayer layer, List<String> message)
        {
            if(this.materialList.Count != layer.materialList.Count)
            {
                //number of layers unmatch.
                message.Add("Number of layers does not matche: Standard: " +
                    this.materialList.Count + " Test: " + layer.materialList.Count);
                return false;
            }

            //now exam if materials are the same
            for(int i=0; i<this.materialList.Count; i++)
            {
                if (!this.materialList[i].compare(layer.materialList[i], message))
                {
                    //if the material at i index does not match the material at i index from the compared layer,
                    //then false
                    return false; 
                }
            }

            return true;
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

        //when using this function, the precondition is this object is from standard model,
        //the argument DOEgbXMLConstruction shall be from test model (user uploaded)
        public Boolean compare(DOEgbXMLConstruction construction, List<String> message)
        {
            //uvalue, absorptance, name and description are not the focus in this comparison


            return this.layer.compare(construction.layer, message);
        }
    }
}
