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
        static int temp_Timeout = 500;
        //--------------------------
        static string temp_PortName;
        static string temp_BaudRate;
        static string temp_Parity;
        static string temp_StopBits;
        static string temp_DataBits;
        readonly static string check_hex = "AAAAAA0F6210D8";    //Сообщение для инициализации обмена данными со счетчиком.
        readonly static string RTC_write_hex = "AAAAAA0362100000D4";    //Сообщение для записи данных на страницу '0' в счетчике (128 байт).
        readonly static string RTC_read_hex = "AAAAAA046210D3"; //Сообщение для чтения данных со счетчика.

        static CommunicationManager comm = new CommunicationManager();


        static void Main(string[] args)
        {
            int countReadData = 0;
            ParamFromConfiguration_Load();
            OpenComPort();

            comm.WriteData(check_hex);      //Инициализации обмена данными со счетчиком.
            Wait(250);

            WriteDataRTC();

            bool checkSumCRC = CheckSumCRC(comm.DataByteList);
            while (checkSumCRC || countReadData == 2)
            {
                if (!checkSumCRC)
                {
                    Console.WriteLine("Повторный запрос на чтение данных RTC");
                    WriteDataRTC();
                    checkSumCRC = CheckSumCRC(comm.DataByteList);
                    countReadData++;
                }
            };
            



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
        static void WriteDataRTC()
        {
            comm.WriteData(RTC_write_hex);  //Записи данных на страницу '0' в счетчике (128 байт).
            Wait(250);

            comm.Count = 0;
            comm.DataByteList.Clear();
            comm.WriteData(RTC_read_hex);   //Чтения данных со счетчика.
            Wait(500);

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
            resultCRC ^= 0x0F;

            return resultCRC == Convert.ToInt32(dataList.Last()) ? true : false;
        }
    }
}
