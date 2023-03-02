using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SNT
{
    class Data_RTC
    {
        const string prefixSendhex = "AAAAAA";

        private int countNumberCounter;
        private string[] numbersCounters;       //Массив количества счетчиков.
        private string[] sendStartSessionHex;   //Массив команд инициализации обмена данными.
        private string[] sendWritePage128Hex;   //Массив команд записи данных на страницу счетчика 128 байт. 
        private string[] sendWritePage256Hex;   //Массив команд записи данных на страницу счетчика 256 байт
        private string[] sendReadDataHex;       //Массив команд чтения данных со счетчика.
        private List<string> numberCounters = new List<string>();

        string tempNumber = "";

        public string[] NumbersCounters
        {
            get { return numbersCounters; }
        }
        public string[] SendStartSessionHex
        {
            get { return sendStartSessionHex; }
        }
        public string[] SendWritePage128Hex
        {
            get { return sendWritePage128Hex; }
        }
        public string[] SendWritePage256Hex
        {
            get { return sendWritePage256Hex; }
        }
        public string[] SendReadDataHex
        {
            get { return sendReadDataHex; }
        }
        public int CountNumberCounter
        {
            get { return countNumberCounter; }
            set { countNumberCounter = value; }
        }

        IniFile INI = new IniFile(@ConfigurationManager.AppSettings["pathConfig"]);

        public Data_RTC()
        {
            Initialization();
            InitializationHexString();
        }
        
        public void Initialization()
        {
            string tempNumberCounters = INI.ReadINI("SNTConfig", "NumberCounter");
            string[] counters = tempNumberCounters.Replace(" ", "")
                                                .Split(',');
            numbersCounters = counters;
            

            for(int i = 0; i < counters.Length; i ++)
            {
                if (counters[i].Length == 4)
                {
                    tempNumber = counters[i].Substring(2,2);
                    tempNumber += counters[i].Substring(0, 2);
                    numberCounters.Add(tempNumber);
                }                
            }
            countNumberCounter = counters.Length;            
        }

        public void InitializationHexString()
        {
            sendStartSessionHex = new string[countNumberCounter];
            sendWritePage128Hex = new string[countNumberCounter];
            sendWritePage256Hex = new string[countNumberCounter];
            sendReadDataHex = new string[countNumberCounter];
            for (int i = 0; i < numberCounters.Count; i++)
            {
                string tempSendHex = CheckSumCRC(prefixSendhex + "0F" + numberCounters[i], 0x0F);
                string tempWritePage128Hex = CheckSumCRC(prefixSendhex + "03" + numberCounters[i] + "0000", 0x0F);
                string tempWritePage256Hex = CheckSumCRC(prefixSendhex + "03" + numberCounters[i] + "0040", 0x0F);
                string tempReadDataHex = CheckSumCRC(prefixSendhex + "04" + numberCounters[i], 0x0F);
                sendStartSessionHex[i] += prefixSendhex + "0F" + numberCounters[i] + tempSendHex;
                sendWritePage128Hex[i] += prefixSendhex + "03" + numberCounters[i] + "0000" + tempWritePage128Hex;
                sendWritePage256Hex[i] += prefixSendhex + "03" + numberCounters[i] + "0040" + tempWritePage256Hex;
                sendReadDataHex[i] += prefixSendhex + "04" + numberCounters[i] + tempReadDataHex;
            }
        }

        //Метод подсчета CRC - пакета данных 128 байт.
        private string CheckSumCRC(string dataString, int extraByte)
        {
            int resultCRC = 0x00;
            for (int i = 0; i < dataString.Length - 1; i+=2)
                resultCRC ^= Convert.ToInt32(dataString.Substring(i, 2), 16);

            resultCRC ^= extraByte;

            return Convert.ToString(resultCRC, 16).ToUpper();
        }
    }
}
