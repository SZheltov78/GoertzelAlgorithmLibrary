using System;

namespace GoertzelAlgorithmLibrary
{
    public static class GoertzelAlgorithm
    {
        //resample rate, default = 4
        //Calculation on frequency 44 KHz / 4 = 11 KHz, we can use 8 KHz and even less, that enough
        public static int RateK = 4;

        //number of data sampling
        public static int CalcCycles = 500;

        //length of array, * 2 because we have 2 channels
        public static int Length = CalcCycles * 2 * RateK;

        //default frequency of data flow
        public static float CurrentFrequency = 44100;

        //array of data
        static short[] Data;

        //is the channel difference be used?
        static bool DefChannel = true;

        //Magnitudes values
        public static float M697, M770, M852, M941, M1209, M1336, M1477, M1633;

        //
        public static float Sensitivity = 20;

        public static float GetMagnitude(int TargetFrequency)
        {
            int Step;
            float Q2 = 0;
            float Q1 = 0;
            float Q0 = 0;
            float Resultat = 0;

            float SamplingRate = CurrentFrequency / RateK;

            Step = 0;
            float Coeff = (float)(2 * Math.Cos(2 * Math.PI * TargetFrequency / SamplingRate)); //coefficient for a given frequency

            float PrevResultat = 0; //preveous result

            for (int index = 0; index <= CalcCycles; index++)
            {
                if (DefChannel)//channel difference be used
                {
                    Q0 = Coeff * Q1 - Q2 + (Data[Step] / 1000 - Data[Step + 1] / 1000); //   /1000 arbitrary coefficient
                }
                else //if one channel
                {
                    Q0 = Coeff * Q1 - Q2 + Data[Step] / 1000;
                }

                Step = Step + RateK * 2; // step = 2 because it stereo, array = left | right | left | right... and so on

                Q2 = Q1;
                Q1 = Q0; //Q0 save to next cycle as Q1

                Resultat = Q1 * Q1 + Q2 * Q2 - Coeff * Q1 * Q2; //math formula

                if (PrevResultat < Resultat)
                {
                    PrevResultat = (float)(Resultat * 0.8); //  *0.8 - arbitrary coefficient, sensitivity depends on this, partially (the sharpness of magnitude peak)
                }
            }

            PrevResultat = (float)(PrevResultat * 0.0001); //* 0.0001 arbitrary coefficient
            return PrevResultat;
        }

        public static string GetDTMFSymdol(short[] _data)
        {
            string ResultMask = "";
            string FreqBit = "";
            string DTMF_symbol = "";

            Data = _data;

            //get magnitudes of all frequencys for DTMF standard
            M697 = GetMagnitude(697);
            M770 = GetMagnitude(770);
            M852 = GetMagnitude(852);
            M941 = GetMagnitude(941);
            M1209 = GetMagnitude(1209);
            M1336 = GetMagnitude(1336);
            M1477 = GetMagnitude(1477);
            M1633 = GetMagnitude(1633);

            //check on sensitivity
            FreqBit = (M697 > Sensitivity) ? "1" : "0";
            ResultMask = ResultMask + FreqBit;

            FreqBit = (M770 > Sensitivity) ? "1" : "0";
            ResultMask = ResultMask + FreqBit;

            FreqBit = (M852 > Sensitivity) ? "1" : "0";
            ResultMask = ResultMask + FreqBit;

            FreqBit = (M941 > Sensitivity) ? "1" : "0";
            ResultMask = ResultMask + FreqBit;

            FreqBit = (M1209 > Sensitivity) ? "1" : "0";
            ResultMask = ResultMask + FreqBit;

            FreqBit = (M1336 > Sensitivity) ? "1" : "0";
            ResultMask = ResultMask + FreqBit;

            FreqBit = (M1477 > Sensitivity) ? "1" : "0";
            ResultMask = ResultMask + FreqBit;

            FreqBit = (M1633 > Sensitivity) ? "1" : "0";
            ResultMask = ResultMask + FreqBit;

            //does we have symbol?
            switch (ResultMask)
            {
                case "10001000":
                    DTMF_symbol = "1";
                    break;
                case "10000100":
                    DTMF_symbol = "2";
                    break;
                case "10000010":
                    DTMF_symbol = "3";
                    break;
                case "01001000":
                    DTMF_symbol = "4";
                    break;
                case "01000100":
                    DTMF_symbol = "5";
                    break;
                case "01000010":
                    DTMF_symbol = "6";
                    break;
                case "00101000":
                    DTMF_symbol = "7";
                    break;
                case "00100100":
                    DTMF_symbol = "8";
                    break;
                case "00100010":
                    DTMF_symbol = "9";
                    break;
                case "00011000":
                    DTMF_symbol = "*";
                    break;
                case "00010100":
                    DTMF_symbol = "0";
                    break;
                case "00010010":
                    DTMF_symbol = "#";
                    break;
                case "10000001":
                    DTMF_symbol = "A";
                    break;
                case "01000001":
                    DTMF_symbol = "B";
                    break;
                case "00100001":
                    DTMF_symbol = "C";
                    break;
                case "00010001":
                    DTMF_symbol = "D";
                    break;
            }
            return DTMF_symbol;
        }
    }
}
