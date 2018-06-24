using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virtual_Machine_Assembler
{
    public class ValidParams
    {

        //RegisterAddressParam = new string[13];
        //RegisterParams = new string[13];
        public string[] RegisterAddressParams;
        public string[] RegisterParams;

        public void init()
        {

            this.RegisterAddressParams = init_RegisterAddressParams();
            this.RegisterParams = init_RegisterParams();
        }
        public string[] init_RegisterAddressParams()
        {
            //RegisterAddressParam = new string[13];
           
            string[] RegisterAddressParams = { "$REGISTER0", "$REGISTER1", "$REGISTER2", "$REGISTER3", "$REGISTER4",
                "$REGISTER5", "$REGISTER6", "$REGISTER7", "$REGISTER8", "$REGISTER9", "$REGISTER10", "$REGISTER11",
                "$REGISTER12", "$REGISTER13" };
            return RegisterAddressParams;

        }

        public string[] init_RegisterParams()
        {
             //RegisterParams = new string[13];
            string[] RegisterParams = { "REGISTER0", "REGISTER1", "REGISTER2", "REGISTER3", "REGISTER4",
                "REGISTER5", "REGISTER6", "REGISTER7", "REGISTER8", "REGISTER9", "REGISTER10","REGISTER11",
                "REGISTER12", "REGISTER13" };
            return RegisterParams;
    }
       


        }

}


   










