using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virtual_Machine_Assembler
{

    public struct Opcode_Data
    {
        public int param_amount, byte_index;
        public string param1;
        public string param2;
        public Opcode.paramtype paramtype1;
        public Opcode.paramtype paramtype2;
        public int reg1value;
        public int reg2value;
        public string memnonic;

        public bool success;

        public Opcode_Data(int param_amount, int byte_index, string userparam_1, string userparam_2, Opcode.paramtype param1, Opcode.paramtype param2, bool succ, int reg1, int reg2, string memnonic)
        {
            this.byte_index = byte_index;
            this.param_amount = param_amount;
            this.param1 = userparam_1;
            this.param2 = userparam_2;
            this.paramtype1 = param1;
            this.paramtype2 = param2;
            //Use this to tell whether or not it was succesful at matching.
            this.success = succ;
            //These are used to store the indexes of the registers for later (like you know just in case)
            this.reg1value = reg1;
            this.reg2value = reg2;
            this.memnonic = memnonic;

        }
    }

    public struct Opcode_ByteData
    {
        public int total_instr_length;
        public int[] bytes;
        public string memnonic;
        public string param2;
        public string param1;
    }


    public class Assembler_Program
    {
        public static ValidParams validparams = new ValidParams();
        public static Dictionary<String, int> labelbytepointers = new Dictionary<string, int>();
        public static bool firstrun = true;

        
        //string[] opcodenumbers = {"Nop", "LD", "LD", "LD", "ST", "ST", "ADD", "ADD", "ADD", "SUB", "SUB", "SUB", "ST8", "ST8", "ST16", "ST16", "LD8", "LD8", "LD16", "LD16", "CMP", "CMP", "CMP", "CMP16", "CMP8", "BEQ", "BEQ", "BNEQ", "BNEQ", "CALL", "RET", "JMP", "JMP", "OR", "OR", "AND", "AND", "XOR", "XOR"};
        public static List<Opcode> validcodes = new List<Opcode>();
        
        static void Main(string[] args)
        {
            validparams.init();
            init();


            //foreach (Opcode n in validcodes)
            // {
            //Console.WriteLine("Memnonic was " + n.memnonic + " and # was " + n.byteindex);
            // }


            bool found_opcode = false;
            Console.WriteLine("A test is being performed to see if all opcodes are in the list");
            //Console.WriteLine(validcodes.Count);
            Console.WriteLine("Now to test if this program can identify an opcode type an assembly command");
            string userline = Console.ReadLine();

                //Kind of cheat to do a two-pass compile.
            compile_(userline);
            Console.WriteLine("First pass completed, recompiling with label pointers");
            firstrun = false;
            compile_(userline);

            new object();

            // int num = getopcode(userline);
            // Console.WriteLine("The input was maybe identified as opcode # " + num);
            // Console.WriteLine("Double Checking the opcode");
            // List<Opcode> thislist = doublecheckopcode(userline, num);
            //if (thislist.Count < 5) { found_opcode = true; }
            // if (found_opcode == true)
            // {
            //Opcode_Data userdata = opcode_matchtest(thislist, userline);
            // string opstring = make_opcode_string(userdata.byte_index);
            // Console.WriteLine("The opcode has been identified as " + opstring + " Opcode # " + userdata.byte_index);
            // bool success = userdata.success;
            // if (success) { Opcode_ByteData byte_data = get_bytes(userdata);

            //Next
            //For now just print out the byte data, later this will turn into streaming the data to a file
            // int length = byte_data.total_instr_length;
            //string opcode_string = "Opcode Instruction was "+ byte_data.memnonic + ": Opcode #"+byte_data.bytes[0]+" ";
            // for (int t=1; t<length+1; t++)
            //{
            // opcode_string = opcode_string + "|" + byte_data.bytes[t];
            // if (t == length) { opcode_string = opcode_string + "|"; }
            //}
            //Console.WriteLine(opcode_string);

            //}
            // }


            System.Threading.Thread.Sleep(60000);


        }

        public static string compile_(string filename)
        {
            string line;
            byte[] bytestream = new byte[600000];
            int bytepointer = 0;
            string out_string = "An error occured or the file did not exist";
            if (System.IO.File.Exists(filename) == true)
            {
                out_string = "The file was succesfully read";
                System.IO.StreamReader objreader;
                objreader = new System.IO.StreamReader(filename);

                bytepointer = 0;



                int old_pointer = 0;
                int old_bytes_to_write = 0;
                do
                {



                    line = objreader.ReadLine();

                    //Now we're compiling!!

                    
                    Opcode_ByteData tempdata = compile_line(line, bytepointer);
                    int bytes_to_write = tempdata.total_instr_length;
                    
                    if (tempdata.param1 == "label") { Console.WriteLine("label was found outside compile line function at " + (bytepointer-old_bytes_to_write)); }
                    for (int t = 0; t < bytes_to_write+1 ; t++)
                    {
                        if (bytestream != null)
                        {
                                
                                bytestream[bytepointer] = (byte)tempdata.bytes[t];

                            //Console.ReadLine();

                            bytepointer++;
                        }
                    }
                    old_pointer = bytepointer;
                    old_bytes_to_write = bytes_to_write;

                } while (objreader.Peek() != -1);

            }
            try
            {
                using (var fs = new FileStream("out.bin", FileMode.Create, FileAccess.Write))
                {
                    fs.Write(bytestream, 0, bytepointer - 1);
                    //return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in process: {0}", ex);
                //return false;
            }
            return out_string;
        }

        public static Opcode_ByteData compile_line(string userline, int bpointer)
        {
            userline.Replace("/r/n", "");
            bool found_opcode = false;
            int num = getopcode(userline);
            Opcode_ByteData byte_data = new Opcode_ByteData();

            IEnumerable<Opcode> thislist = doublecheckopcode(userline, num);


            if (thislist.Count() > 0) { found_opcode = true; }
            if (found_opcode == true)
            {

                Opcode_Data userdata = opcode_matchtest(thislist, userline);
                string opstring = make_opcode_string(userdata.byte_index);
                if (userdata.success)
                {
                    byte_data = get_bytes(userdata, bpointer);
                    byte_data.param1 = userdata.param1;
                    byte_data.param2 = userdata.param2;

                    //Next
                    //For now just print out the byte data, later this will turn into streaming the data to a file
                    int length = byte_data.total_instr_length;
                    string opcode_string = "Opcode Instruction was " + byte_data.memnonic + ": Opcode #" + byte_data.bytes[0] + " ";
                    for (int t = 1; t < length + 1; t++)
                    {
                        opcode_string = opcode_string + "|" + byte_data.bytes[t];
                        if (t == length) { opcode_string = opcode_string + "|"; }
                    }
                    Console.WriteLine(opcode_string);

                }
            }

           
            return byte_data;
        }

        public static int getopcode(string line)
        {
            int opcodenum = 0;
            //Get the string and convert it to an array
            string[] opcodeparams = toarray(line);
            //Get the first identity of the opcode
            string identity = opcodeparams[0];
            opcodenum = identify(identity);
            return opcodenum;
        }
        //This returns a structure that contains everything the user entered, and the opcode information
        //From the internal opcode table 
        static Opcode_Data opcode_matchtest(IEnumerable<Opcode> opcodelist, string inputline)
        {
            //this should again patch the pesky issue
            string userp1 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Opcode_Data outdata = new Opcode_Data();
            Opcode.paramtype userparam1;
            Opcode.paramtype userparam2;
            userparam1 = Opcode.paramtype.DEFAULT;
            userparam2 = Opcode.paramtype.DEFAULT;
            string[] userline = toarray(inputline);
            if (!inputline.Contains("NOP"))
            {
                userp1 = userline[1];
            }
            //This little patch allows the if check later on userp2 as an array even if the userp2 string isn't filled
            //by a conversion from userp2 to char array.
            string userp2 = "ABCDEFGHIJKLKMNOPQRSTUVWXYZ";

            Console.WriteLine(userline.Count());
            bool islabel = false;
            bool isgoto = false;
            if (userline.Count() > 2 && userline[2].Length > 0) { userp2 = userline[2]; }
            if (userline[0] == "label") { userp2 = userline[1]; userp1 = userline[0]; islabel = true; }
            if (userline[0] == "goto") { userp2 = userline[1]; userp1 = userline[0]; isgoto = true; }
            int param_amount = 0;
            bool match = false;
            int matched_index = 0;
            Opcode oc = new Opcode();

            //check param1 and 2 if registers
            for (int t = 0; t < 12 + 1; t++)
            {
                if (userp1 == validparams.RegisterParams[t])
                {
                    userparam1 = Opcode.paramtype.REGISTER;
                    outdata.reg1value = t;
                }
                if (userp1 == validparams.RegisterAddressParams[t])
                {
                    userparam1 = Opcode.paramtype.REGISTERADDRESS;
                    outdata.reg1value = t;
                }
                if (userp2 == validparams.RegisterParams[t])
                {
                    userparam2 = Opcode.paramtype.REGISTER;
                    outdata.reg2value = t;
                }
                if (userp2 == validparams.RegisterAddressParams[t])
                {
                    userparam2 = Opcode.paramtype.REGISTERADDRESS;
                    outdata.reg2value = t;
                }
                

            }
            char[] line1 = userp1.ToCharArray();
            char[] line2 = userp2.ToCharArray();
            if (line1[0] == 'V') { userparam1 = Opcode.paramtype.VALUE; }
            if (line2[0] == 'V') { userparam2 = Opcode.paramtype.VALUE; }
            if (line1[0] == '_') { userparam1 = Opcode.paramtype.ADDRESS; }
            if (line2[0] == '_') { userparam2 = Opcode.paramtype.ADDRESS; }
            if (userp1 == "goto") { userparam1 = Opcode.paramtype.GOTO; userparam2 = Opcode.paramtype.STRING; }
           
           
            if (userp2.Length > 0) { param_amount = 2; }

            if (param_amount == 1)
            {
                foreach (Opcode n in opcodelist)
                {
                    if (userparam1 == n.param1)
                    {
                        match = true; matched_index = n.byteindex; oc = n;
                        break;
                    }
                }

            }

            if (param_amount == 2)
            {
                foreach (Opcode n in opcodelist)
                {
                    Console.WriteLine("The memnonic was " + n.memnonic + " and the first userline was " + userp1);
                    if (userparam1 == n.param1 && userparam2 == n.param2)
                    {
                        match = true; matched_index = n.byteindex; oc = n;
                        break;
                    }

                    if (islabel)
                    {
                        if (n.memnonic == "label")
                        {
                            match = true; matched_index = n.byteindex; oc = n;
                        }
                    }

                    if (isgoto)
                    {
                        if (n.memnonic == "goto")
                        {
                            match = true; matched_index = n.byteindex; oc = n;
                        }
                    }


                }

            }



            if (match == true)
            {

                outdata.byte_index = oc.byteindex;
                outdata.param1 = userp1;
                outdata.param2 = userp2;
                outdata.param_amount = param_amount;
                outdata.success = true;
                outdata.paramtype1 = oc.param1;
                outdata.paramtype2 = oc.param2;
                outdata.memnonic = oc.memnonic;
            }
            if (match == false)
            {
                outdata.success = false;
            }
            return outdata;

        }

        public static Opcode_ByteData get_bytes(Opcode_Data data, int bpointer)
        {
            int[] bytes = new int[30]; //We'll use extra just to be safe
            int firstbyte = data.byte_index;

            //handle replace goto with JMP because it's basically the same thing but with label
            if (firstbyte == 42) { firstbyte = 33; }


            int currentbyte = 1;
            bytes[0] = firstbyte;
            int param1length = data.param1.Length;
            int param2length = data.param2.Length;
            int param1bytelength = 1;
            int param2bytelength = 1;
            int value = 0;



            //This takes care of only two special cases where the value may not be 4 bytes long
            if (data.memnonic == "ST8") { param1bytelength = 1; }
            if (data.memnonic == "ST16") { param1bytelength = 2; }

            if (data.paramtype1 == Opcode.paramtype.REGISTER || data.paramtype1 == Opcode.paramtype.REGISTERADDRESS)
            {
                //If the first parameter is a register then the first byte is just the register's value
                bytes[currentbyte] = data.reg1value;
                currentbyte = currentbyte + 1;
            }

            if (data.paramtype1 == Opcode.paramtype.VALUE || data.paramtype1 == Opcode.paramtype.ADDRESS)
            {
                bool writtenvalue = false;
                value = System.Convert.ToInt32(Right(data.param1, param1length - 1));
                //Console.WriteLine("The value was " + secondbyte);
                byte[] results = INT2LE(value);
                //Console.WriteLine("Bytes are " + results[0] + "," + results[1] + "," + results[2] + "," + results[3]);
                int[] r = new int[10];
                get(value, 8, 0, r);
                //Console.WriteLine(" for comparison other bytes are " + r[0] + "," + r[1] + "," + r[2] + "," +r[3]);
                //Pad the value bytes to two
                if (param1bytelength == 2 && data.memnonic == "ST16")
                {
                    value = value & 65535;
                    int highbyte = value & 255;     // high byte (0x12)
                    int lowbyte = value >> 8; // low byte  (0x34)
                    bytes[currentbyte] = lowbyte;
                    bytes[currentbyte + 1] = highbyte;
                    currentbyte = currentbyte + 1;
                    currentbyte = currentbyte + 1;
                    writtenvalue = true;
                    //Console.WriteLine("highbyte was " + highbyte + " and lowbyte was " + lowbyte);

                }

                //Pad the value bytes to 1
                if (param1bytelength == 1 && data.memnonic == "ST8")
                {
                    value = value & 255;
                    bytes[currentbyte] = value;
                    currentbyte = currentbyte + 1;
                    writtenvalue = true;
                }

                if (writtenvalue == false)
                    if (data.memnonic != "ST16" && data.memnonic != "ST8")
                    {
                        {
                            results = INT2LE(value);
                            bytes[currentbyte] = results[0];
                            currentbyte++;
                            bytes[currentbyte] = results[1];
                            currentbyte++;
                            bytes[currentbyte] = results[2];
                            currentbyte++;
                            bytes[currentbyte] = results[3];
                            currentbyte++;

                        }
                    }


            }


            if (data.paramtype2 == Opcode.paramtype.REGISTER || data.paramtype2 == Opcode.paramtype.REGISTERADDRESS)
            {
                value = data.reg2value;
                //Write the current value
                bytes[currentbyte] = value;


            }

           

            if (data.paramtype2 == Opcode.paramtype.STRING)
            {

                if (data.paramtype1 != Opcode.paramtype.GOTO)
                {
                    value = 0;
                    //if (data.paramtype2 == Opcode.paramtype.VALUE || data.paramtype2 == Opcode.paramtype.ADDRESS) { value = System.Convert.ToInt32(Right(data.param2, param2length - 1)); }
                    //patch in checking for byte pointers, only if not label type.

                    if (firstrun)
                    {
                        value = bpointer;
                        if (data.paramtype1 == Opcode.paramtype.LABEL) { labelbytepointers.Add(data.param2, value); }
                       
                    }

                }

                if (data.paramtype1 == Opcode.paramtype.GOTO)
                {
                    value = 0; //we don't know what it could be until recompile :)
                       
                    {
                        //A bit redundant since we declare 'value' 0 later for goto but. whatever.
                        //in theory this should compile ALL the needed bytes and still get the correct pointer
                        if (!firstrun) {
                            if (data.param1 == "goto" && (data.param1 != "label")) {
                            value = labelbytepointers[data.param2]; }
                            Console.WriteLine("Value injection found for goto statement " + data.param2 + " and the injection value was " + value);
                        }
                        byte[] results = INT2LE(value);
                        bytes[currentbyte] = results[0];
                        currentbyte++;
                        bytes[currentbyte] = results[1];
                        currentbyte++;
                        bytes[currentbyte] = results[2];
                        currentbyte++;
                        bytes[currentbyte] = results[3];
                        currentbyte++;
                       
                       
                    }

                }

                
                    

            }

            //Console.WriteLine("Just to confirm the bytes written were " + "opcode byte " + bytes[0] + " , " + bytes[1] + "," + bytes[2] + "," + bytes[3] + "," + bytes[4] + "," + bytes[5]+","+bytes[6]);

            Opcode_ByteData opcode_bytes = new Opcode_ByteData();
            opcode_bytes.total_instr_length = currentbyte;
            opcode_bytes.bytes = bytes;
            opcode_bytes.memnonic = data.memnonic;
            return opcode_bytes;
        }

        static byte[] INT2LE(int data)
        {
            byte[] b = new byte[4];
            b[0] = (byte)data;
            b[1] = (byte)(((int)data >> 8) & 0xFF);
            b[2] = (byte)(((int)data >> 16) & 0xFF);
            b[3] = (byte)(((int)data >> 24) & 0xFF);
            return b;
        }

        static string make_opcode_string(int opcodenum)
        {
            string param1 = "";
            string param2 = "";
            string opcode_string = "";
            int param_amount = validcodes[opcodenum].param_amount;

            //This is so ugly but there's no direct translation
            if (validcodes[opcodenum].param1 == Opcode.paramtype.ADDRESS) { param1 = "$ADDRESS"; }
            if (validcodes[opcodenum].param1 == Opcode.paramtype.REGISTER) { param1 = "REGISTER"; }
            if (validcodes[opcodenum].param1 == Opcode.paramtype.REGISTERADDRESS) { param1 = "$REGISTER"; }
            if (validcodes[opcodenum].param1 == Opcode.paramtype.VALUE) { param1 = "VALUE"; }

            if (validcodes[opcodenum].param2 == Opcode.paramtype.ADDRESS) { param2 = "$ADDRESS"; }
            if (validcodes[opcodenum].param2 == Opcode.paramtype.REGISTER) { param2 = "REGISTER"; }
            if (validcodes[opcodenum].param2 == Opcode.paramtype.REGISTERADDRESS) { param2 = "$REGISTER"; }
            if (validcodes[opcodenum].param2 == Opcode.paramtype.VALUE) { param2 = "VALUE"; }

            opcode_string = opcode_string + validcodes[opcodenum].memnonic + " " + param1 + ", " + param2;



            return opcode_string;
        }

        static int[] get(int x, int size, int num, int[] array)
        {
            for (int i = 0; i < size; i++)
            {
                array[i] = x >> size * i & (1 << size) - 1;
            }
            return array;
        }

        public static IEnumerable<Opcode> doublecheckopcode(string line, int opcodenum)
        {
            string[] opcodeparams = toarray(line);
            string identity = opcodeparams[0];
            //This is beautiful!!!

            //extra patching to try to force the list cleared.

            IEnumerable<Opcode> result = validcodes.FindAll(c => (c.memnonic == identity));
            //List<Opcode> result = new List<Opcode>();

            //Console.WriteLine(result.ToString());

            new object();
            return result;
        }

        //easily split the line into seperate strings
        static string[] toarray(string line)
        {
            string[] array = line.Split(' ', ',');
            return array;
        }

        static int identify(string identity)
        {
            //Set to zero by default
            int opcodenum = 0;
            foreach (Opcode n in validcodes)
            {
                string memnonic = n.memnonic;
                if (memnonic == identity) { opcodenum = n.byteindex; }
            }

            return opcodenum;
        }


        // Explicit predicate delegate.
        private bool FindOpcodeByByteIndex(Opcode op, int qualifier)
        {

            if (op.byteindex == qualifier)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public string Left(string param, int length)
        {
            string result = param.Substring(0, length);
            return result;
        }
        public static string Right(string param, int length)
        {
            string result = param.Substring(param.Length - length, length);
            return result;
        }
        public string Mid(string param, int startIndex, int length)
        {
            string result = param.Substring(startIndex, length);
            return result;
        }

        public static void init()
        {
            Opcode nop = new Opcode();
            nop.setbyteindex(0);
            nop.memnonic = "NOP";

            nop.setparamamount(0);
            nop.setparam1(Opcode.paramtype.DEFAULT);
            validcodes.Add(nop);

            Opcode ld0 = new Opcode();
            ld0.setbyteindex(1);
            ld0.memnonic = "LD";

            ld0.setparamamount(2);
            ld0.setparam1(Opcode.paramtype.REGISTER);
            ld0.setparam2(Opcode.paramtype.VALUE);
            validcodes.Add(ld0);

            Opcode ld1 = new Opcode();
            ld1.setbyteindex(2);
            ld1.memnonic = "LD";

            ld1.setparamamount(2);
            ld1.setparam1(Opcode.paramtype.REGISTER);
            ld1.setparam2(Opcode.paramtype.REGISTER);
            validcodes.Add(ld1);

            Opcode ld2 = new Opcode();
            ld2.setbyteindex(3);
            ld2.memnonic = "LD";

            ld2.setparamamount(2);
            ld2.setparam1(Opcode.paramtype.REGISTER);
            ld2.setparam2(Opcode.paramtype.REGISTERADDRESS);
            validcodes.Add(ld2);

            Opcode st0 = new Opcode();
            st0.setbyteindex(4);
            st0.memnonic = "ST";

            st0.setparamamount(2);
            st0.setparam1(Opcode.paramtype.VALUE);
            st0.setparam2(Opcode.paramtype.REGISTER);
            validcodes.Add(st0);

            Opcode st1 = new Opcode();
            st1.setbyteindex(5);
            st1.memnonic = "ST";

            st1.setparamamount(1);
            st1.setparam1(Opcode.paramtype.VALUE);
            st1.setparam2(Opcode.paramtype.REGISTERADDRESS);
            validcodes.Add(st1);

            Opcode st2 = new Opcode();
            st2.setbyteindex(40); //When the first byte of this opcode is written it will write 40
            st2.memnonic = "ST"; //This will add another valid ST variant to the opcodes checking routine
            st2.setparamamount(2);
            st2.setparam1(Opcode.paramtype.REGISTER);
            st2.setparam2(Opcode.paramtype.REGISTERADDRESS);
            validcodes.Add(st2);

            Opcode add0 = new Opcode();
            add0.setbyteindex(6);
            add0.memnonic = "ADD";

            add0.setparamamount(2);
            add0.setparam1(Opcode.paramtype.REGISTER);
            add0.setparam2(Opcode.paramtype.REGISTER);
            validcodes.Add(add0);

            Opcode add1 = new Opcode();
            add1.setbyteindex(7);
            add1.memnonic = "ADD";

            add1.setparamamount(2);
            add1.setparam1(Opcode.paramtype.REGISTER);
            add1.setparam2(Opcode.paramtype.VALUE);
            validcodes.Add(add1);

            Opcode add2 = new Opcode();
            add2.setbyteindex(8);
            add2.memnonic = "ADD";

            add2.setparamamount(2);
            add2.setparam1(Opcode.paramtype.REGISTER);
            add2.setparam2(Opcode.paramtype.REGISTERADDRESS);
            validcodes.Add(add2);

            Opcode sub0 = new Opcode();
            sub0.setbyteindex(9);
            sub0.memnonic = "SUB";

            sub0.setparamamount(2);
            sub0.setparam1(Opcode.paramtype.REGISTER);
            sub0.setparam2(Opcode.paramtype.REGISTER);
            validcodes.Add(sub0);

            Opcode sub1 = new Opcode();
            sub1.setbyteindex(10);
            sub1.memnonic = "SUB";

            sub1.setparamamount(2);
            sub1.setparam2(Opcode.paramtype.REGISTER);
            sub1.setparam2(Opcode.paramtype.VALUE);
            validcodes.Add(sub1);

            Opcode sub2 = new Opcode();
            sub2.setbyteindex(11);
            sub2.memnonic = "SUB";

            sub2.setparamamount(2);
            sub2.setparam2(Opcode.paramtype.REGISTER);
            sub2.setparam2(Opcode.paramtype.REGISTERADDRESS);
            validcodes.Add(sub2);

            Opcode st8_0 = new Opcode();
            st8_0.setbyteindex(12);
            st8_0.memnonic = "ST8";

            st8_0.setparamamount(2);
            st8_0.setparam1(Opcode.paramtype.VALUE);
            st8_0.setparam2(Opcode.paramtype.REGISTER);
            validcodes.Add(st8_0);

            Opcode st8_1 = new Opcode();
            st8_1.setbyteindex(13);
            st8_1.memnonic = "ST8";

            st8_1.setparamamount(2);
            st8_1.setparam1(Opcode.paramtype.VALUE);
            st8_1.setparam2(Opcode.paramtype.REGISTERADDRESS);
            validcodes.Add(st8_1);



            Opcode st16_0 = new Opcode();
            st16_0.setbyteindex(14);
            st16_0.memnonic = "ST16";

            st16_0.setparamamount(2);
            st16_0.setparam1(Opcode.paramtype.VALUE);
            st16_0.setparam2(Opcode.paramtype.REGISTER);
            validcodes.Add(st16_0);

            Opcode st16_1 = new Opcode();
            st16_1.setbyteindex(15);
            st16_1.memnonic = "ST16";

            st16_1.setparamamount(2);
            st16_1.setparam1(Opcode.paramtype.VALUE);
            st16_1.setparam2(Opcode.paramtype.REGISTERADDRESS);
            validcodes.Add(st16_1);

            Opcode ld8_0 = new Opcode();
            ld8_0.setbyteindex(16);
            ld8_0.memnonic = "LD8";

            ld8_0.setparamamount(2);
            ld8_0.setparam1(Opcode.paramtype.REGISTER);
            ld8_0.setparam2(Opcode.paramtype.ADDRESS);
            validcodes.Add(ld8_0);

            Opcode ld8_1 = new Opcode();
            ld8_1.setbyteindex(17);
            ld8_1.memnonic = "LD8";

            ld8_1.setparamamount(2);
            ld8_1.setparam1(Opcode.paramtype.REGISTER);
            ld8_1.setparam2(Opcode.paramtype.REGISTER);
            validcodes.Add(ld8_1);

            Opcode ld16_0 = new Opcode();
            ld16_0.setbyteindex(18);
            ld16_0.memnonic = "LD16";

            ld16_0.setparamamount(2);
            ld16_0.setparam1(Opcode.paramtype.REGISTER);
            ld16_0.setparam2(Opcode.paramtype.ADDRESS);
            validcodes.Add(ld16_0);

            Opcode ld16_1 = new Opcode();
            ld16_1.setbyteindex(19);
            ld16_1.memnonic = "LD16"; // ld1.memnonic = "LD16"; THIS OLD LINE CAUSED ME A HEADACHE it was not recognizing LD REGISTER, REGISTER finally found ld1 doesn't
                                      //exist when I commented out ld1's decleration
                                      //it jumped to THIS line!!

            ld16_1.setparamamount(2);
            ld16_1.setparam1(Opcode.paramtype.REGISTER);
            ld16_1.setparam2(Opcode.paramtype.REGISTER);
            validcodes.Add(ld16_1);

            Opcode cmp0 = new Opcode();
            cmp0.setbyteindex(20);
            cmp0.memnonic = "CMP";

            cmp0.setparamamount(2);
            cmp0.setparam1(Opcode.paramtype.REGISTER);
            cmp0.setparam2(Opcode.paramtype.REGISTER);
            validcodes.Add(cmp0);

            Opcode cmp1 = new Opcode();
            cmp1.setbyteindex(21);
            cmp1.memnonic = "CMP";

            cmp1.setparamamount(2);
            cmp1.setparam1(Opcode.paramtype.REGISTER);
            cmp1.setparam2(Opcode.paramtype.REGISTERADDRESS);
            validcodes.Add(cmp1);

            Opcode cmp2 = new Opcode();
            cmp2.setbyteindex(22);
            cmp2.memnonic = "CMP";

            cmp2.setparamamount(2);
            cmp2.setparam1(Opcode.paramtype.REGISTER);
            cmp2.setparam2(Opcode.paramtype.VALUE);
            validcodes.Add(cmp2);

            Opcode cmp16 = new Opcode();
            cmp16.setbyteindex(23);
            cmp16.memnonic = "CMP16";

            cmp16.setparamamount(2);
            cmp16.setparam1(Opcode.paramtype.REGISTER);
            cmp16.setparam2(Opcode.paramtype.REGISTERADDRESS);
            validcodes.Add(cmp16);

            Opcode cmp8 = new Opcode();
            cmp8.setbyteindex(24);
            cmp8.memnonic = "CMP8";

            cmp8.setparamamount(2);
            cmp8.setparam1(Opcode.paramtype.REGISTER);
            cmp8.setparam2(Opcode.paramtype.REGISTERADDRESS);
            validcodes.Add(cmp8);

            Opcode beq0 = new Opcode();
            beq0.setbyteindex(25);
            beq0.memnonic = "BEQ";

            beq0.setparamamount(1);
            beq0.setparam1(Opcode.paramtype.ADDRESS);
            validcodes.Add(beq0);

            Opcode beq1 = new Opcode();
            beq1.setbyteindex(26);
            beq1.memnonic = "BEQ";

            beq1.setparamamount(1);
            beq1.setparam1(Opcode.paramtype.REGISTERADDRESS);
            validcodes.Add(beq1);

            Opcode bneq0 = new Opcode();
            bneq0.setbyteindex(27);
            bneq0.memnonic = "BNEQ";

            bneq0.setparamamount(1);
            bneq0.setparam1(Opcode.paramtype.REGISTERADDRESS);
            validcodes.Add(bneq0);

            Opcode bneq1 = new Opcode();
            bneq1.setbyteindex(28);
            bneq1.memnonic = "BNEQ";

            bneq1.setparamamount(1);
            bneq1.setparam1(Opcode.paramtype.ADDRESS);
            validcodes.Add(bneq1);

            Opcode call0 = new Opcode();
            call0.setbyteindex(29);
            call0.memnonic = "CALL";

            call0.setparamamount(1);
            call0.setparam1(Opcode.paramtype.ADDRESS);
            validcodes.Add(call0);

            Opcode call1 = new Opcode();
            call1.setbyteindex(30);
            call1.memnonic = "CALL";

            call1.setparamamount(1);
            call1.setparam1(Opcode.paramtype.REGISTERADDRESS);
            validcodes.Add(call1);

            Opcode ret = new Opcode();
            ret.setbyteindex(31);
            ret.memnonic = "RET";

            ret.setparamamount(0);
            validcodes.Add(ret);

            Opcode jmp0 = new Opcode();
            jmp0.setbyteindex(32);
            jmp0.memnonic = "JMP";

            jmp0.setparamamount(1);
            jmp0.setparam1(Opcode.paramtype.REGISTERADDRESS);
            validcodes.Add(jmp0);

            Opcode jmp1 = new Opcode();
            jmp1.setbyteindex(33);
            jmp1.memnonic = "JMP";

            jmp1.setparamamount(1);
            jmp1.setparam1(Opcode.paramtype.ADDRESS);
            validcodes.Add(jmp1);

            Opcode or0 = new Opcode();
            or0.setbyteindex(34);
            or0.memnonic = "OR";

            or0.setparamamount(2);
            or0.setparam1(Opcode.paramtype.REGISTER);
            or0.setparam2(Opcode.paramtype.VALUE);
            validcodes.Add(or0);

            Opcode or1 = new Opcode();
            or1.setbyteindex(35);
            or1.memnonic = "OR";

            or1.setparamamount(2);
            or1.setparam1(Opcode.paramtype.REGISTER);
            or1.setparam2(Opcode.paramtype.REGISTERADDRESS);
            validcodes.Add(or1);

            Opcode and0 = new Opcode();
            and0.setbyteindex(36);
            and0.memnonic = "AND";

            and0.setparamamount(2);
            and0.setparam1(Opcode.paramtype.REGISTER);
            and0.setparam2(Opcode.paramtype.VALUE);
            validcodes.Add(and0);

            Opcode and1 = new Opcode();
            and1.setbyteindex(37);
            and1.memnonic = "AND";

            and1.setparamamount(2);
            and1.setparam1(Opcode.paramtype.REGISTER);
            and1.setparam2(Opcode.paramtype.REGISTERADDRESS);
            validcodes.Add(and1);

            Opcode xor0 = new Opcode();
            xor0.setparamamount(38);
            xor0.memnonic = "XOR";

            xor0.setparam1(Opcode.paramtype.REGISTER);
            xor0.setparam2(Opcode.paramtype.VALUE);
            validcodes.Add(xor0);

            Opcode xor1 = new Opcode();
            xor1.setbyteindex(39);
            xor1.memnonic = "XOR";

            xor1.setparamamount(2);
            xor1.setparam1(Opcode.paramtype.REGISTER);
            xor1.setparam2(Opcode.paramtype.REGISTERADDRESS);
            validcodes.Add(xor1);

            Opcode label = new Opcode();
            label.setparamamount(2);
            label.memnonic = "label";
            label.setparam1(Opcode.paramtype.LABEL);
            label.setparam2(Opcode.paramtype.STRING);
            label.setbyteindex(41);
            validcodes.Add(label);

            Opcode goto_ = new Opcode();
            goto_.setparamamount(2);
            goto_.memnonic = "goto";
            goto_.setparam2(Opcode.paramtype.STRING);
            goto_.setparam1(Opcode.paramtype.GOTO);
            goto_.setbyteindex(42);
            validcodes.Add(goto_);


        }
    }
}
