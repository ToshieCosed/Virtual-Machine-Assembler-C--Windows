using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virtual_Machine_Assembler
{
    public class Opcode
    {
        public enum paramtype{DEFAULT, REGISTER, VALUE, ADDRESS, REGISTERADDRESS, LABEL, STRING, GOTO};
        public int param_amount;
        public string memnonic;
        public paramtype param1 = paramtype.DEFAULT;
        public paramtype param2 = paramtype.DEFAULT;
        public int byteindex = 0;

        public paramtype get_param1() {
            return param1;
            }

        public paramtype get_param2()
        {
            return param2;
        }

        public void setparam1(paramtype t)
        {
            param1 = t;
        }

        public void setparam2(paramtype t)
        {
            param2 = t;
        }

        public void setparamamount(int amount)
        {
            param_amount = amount;
        }

        public void setbyteindex(int index)
        {
            byteindex = index;
        }
    }
}
