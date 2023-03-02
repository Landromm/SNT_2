using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SNT
{
    internal class Program
    {
        static int temp_Timeout = 1000;
        //--------------------------
        static string temp_PortName;
        static string temp_BaudRate;
        static string temp_Parity;
        static string temp_StopBits;
        static string temp_DataBits;
        readonly static string check_hex = "AAAAAA0F6210D8";    //Сообщение для инициализации обмена данными со счетчиком.
        readonly static string RTC_write_hex = "AAAAAA0362100000D4";    //Сообщение для записи данных на страницу '0' в счетчике (128 байт).
        readonly static string read_hex = "AAAAAA046210D3"; //Сообщение для чтения данных со счетчика.
        
        readonly static string NV_write_hex = "AAAAAA036210004094";    //Сообщение для записи данных на страницу '0' в счетчике (128 байт).


        static bool checkSumCRC = false;

        static CommunicationManager comm = new CommunicationManager();
        static Data_RTC data_RTC = new Data_RTC();



        static void Main(string[] args)
        {

            for (int i = 0; i < data_RTC.CountNumberCounter; i++)
            {
                Console.WriteLine("Отправляемые пакеты данных для счетчика #{0}:", data_RTC.NumbersCounters[i]);
                Console.WriteLine(data_RTC.SendStartSessionHex[i] + " - сообщение инициализации обмена.");
                Console.WriteLine(data_RTC.SendWritePage128Hex[i] + " - сообщение записи страницы 128 байт данных.");
                Console.WriteLine(data_RTC.SendWritePage256Hex[i] + " - сообщение записи страницы 256 байт данных.");
                Console.WriteLine(data_RTC.SendReadDataHex[i] + " - сообщение чтения данных со страницы.");
                Console.WriteLine("\n");
            }


            int countReadData = 0;
            ParamFromConfiguration_Load();
            OpenComPort();

            for (int i = 0; i < data_RTC.CountNumberCounter; i++)
            {
                comm.WriteData(data_RTC.SendStartSessionHex[i]);      //Инициализации обмена данными со счетчиком.
                Wait(250);

                do
                {
                    WriteDataRTC(data_RTC.SendWritePage128Hex[i], data_RTC.SendReadDataHex[i]);
                    if(comm.DataByteList.Count != 0)
                        checkSumCRC = CheckSumCRC(comm.DataByteList);

                    if (!checkSumCRC)
                    {
                        Console.WriteLine("Повторный запрос на чтение данных RTC");
                        countReadData++;
                    }
                }
                while (!checkSumCRC && countReadData < 3);
            }            

            comm.ClosePort();

            Console.ReadLine();
        }

        #region Wait TimeOut - реализация задержки.
        // Методы реализации задержки отображения без блокировки потока пользовательского интерфейса.
        public static void Wait(int interval)
        {
            ExecuteWait(() => Thread.Sleep(interval));
        }
        public static void ExecuteWait(Action action)
        {
            var waitFrame = new DispatcherFrame();
            // Use callback to "pop" dispatcher frame
            IAsyncResult op = action.BeginInvoke(dummy => waitFrame.Continue = false, null);
            // this method will block here but window messages are pumped
            Dispatcher.PushFrame(waitFrame);
            // this method may throw if the action threw. caller responsibility to handle.
            action.EndInvoke(op);
        }
        #endregion

        //Метод чтения параметров COM-порт из ini. файла
        static void ParamFromConfiguration_Load()
        {
            try
            {
                IniFile INI = new IniFile(@ConfigurationManager.AppSettings["pathConfig"]);
                temp_PortName = INI.ReadINI("COMportSettings", "PortName");
                temp_BaudRate = INI.ReadINI("COMportSettings", "BaudRate");
                temp_Parity = INI.ReadINI("COMportSettings", "Parity");
                temp_StopBits = INI.ReadINI("COMportSettings", "StopBits");
                temp_DataBits = INI.ReadINI("COMportSettings", "DataBits");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка чтения config.ini файла!\n" + ex,
                                "Ошибка !");
            }
        }

        //Метод открытия COM-порта.
        static public void OpenComPort()
        {
            comm.Parity = temp_Parity;
            comm.StopBits = temp_StopBits;
            comm.DataBits = temp_DataBits;
            comm.BaudRate = temp_BaudRate;
            comm.PortName = temp_PortName;
            comm.CurrentTransmissionType = CommunicationManager.TransmissionType.Hex;
            comm.OpenPort();
            if (comm.ComPortIsOpen())
            {

                Console.WriteLine("Параметры COM-порта:\n" +
                     "Четность: " + temp_Parity + "\n" +
                     "Стоповые биты: " + temp_StopBits + "\n" +
                     "Биты данных: " + temp_DataBits + "\n" +
                     "Бит в секунду: " + temp_BaudRate + "\n" +
                     "Таймаут: " + temp_Timeout + "\n");
            }
        }

        //Метод опроса счетчика - пакет данных 128 байт - RTC
        static void WriteDataRTC(string writePageMsg, string readDataMsg)
        {
            comm.WriteData(writePageMsg);  //Записи данных на страницу '0' в счетчике (128 байт).
            Wait(250);

            comm.Count = 0;
            comm.DataByteList.Clear();
            comm.WriteData(readDataMsg);   //Чтения данных со счетчика.
            Wait(temp_Timeout);

            Console.WriteLine("-----------------------STOP----------------------");
            
            //foreach (var item in comm.DataByteList)
            //{
            //    Console.WriteLine(item);
            //}
        }

        //Метод подсчета CRC - пакета данных 128 байт.
        static bool CheckSumCRC(List<string> dataList)
        {
            int resultCRC = 0x00;
            for(int i = 0; i < dataList.Count-1; i++)
            {
                resultCRC ^= Convert.ToInt32(dataList[i], 16);
            }
            resultCRC ^= 0xA5;


            Console.WriteLine(resultCRC);
            Console.WriteLine(Convert.ToInt32(dataList.Last(), 16));

            return resultCRC == Convert.ToInt32(dataList.Last(), 16) ? true : false;
        }
    }
}
