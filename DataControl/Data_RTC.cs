using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace SNT.DataControl
{
    class Data_RTC
    {
        delegate string InvertedString(string input);

        private string _timeAndData;                    //Текущее время и дата.
        private string _serialNumber;                   //Заводской номер.
        private string _accessCode;                     //Код доступа.
        private string _configurator;                   //Конфигуратор.
        private string _interface;                      //Интерфейс.
        private string _levelSignalU1Channel1;          //Уровень сингнала U1 - Канала №1.          
        private string _levelSignalU2Channel1;          //Уровень сингнала U2 - Канала №1.          
        private string _levelSignalU3Channel1;          //Уровень сингнала U3 - Канала №1.
        private string _levelSignalU1Channel2;          //Уровень сингнала U1 - Канала №2.          
        private string _levelSignalU2Channel2;          //Уровень сингнала U2 - Канала №2.          
        private string _levelSignalU3Channel2;          //Уровень сингнала U3 - Канала №2.
        private string _instraction;                    //Инструкция.
        private string _errorChannel1;                  //Ошибки канала №1.
        private string _errorChannel2;                  //Ошибки канала №2.
        private string _errorSystem;                    //Ошибки системы.
        private string _temperatureChanel1;             //Температура канал №1.
        private string _temperatureChanel2;             //Температура канал №2.
        private string _temperature_T3;                 //Температура Т3.
        private string _temperature_T4_T5;              //Температура Т4/T5.
        private string _commissioning;                  //Ввод в эксплуатацию.
        private string _lowDifferenceTemp;              //Малая разность температур.
        private string _pressure_P3;                    //Давление Р3.
        private string _pressure_P4_P5;                 //Давление Р4/P5.
        private string _downtime;                       //Время простоя.
        private string _runningTime;                    //Время наработки.
        private string _irregularFlowTime;              //Ненармированный расход (время).
        private string _excessFlowTime;                 //Превышение расхода (время).
        private string _noFlowTime;                     //Отсутствие расхода (время).
        private string _negativeFlowTime;               //Отрицательный расход (время).
        private string _defectTime;                     //Неисправность (время).
        private string _noPowerTime;                    //Отсутствие питания (время).
        private string _maxSensorsPressure;             //Максимум датчиков давления.

        public string TimeAndData 
        { 
            get => _timeAndData;
            set => _timeAndData = invertedString(value);
        }

        public string SerialNumber 
        { 
            get => _serialNumber; 
            set => _serialNumber = value; 
        }

        public string AccessCode 
        { 
            get => _accessCode; 
            set => _accessCode = value; 
        }

        public string Configurator 
        { 
            get => _configurator; 
            set => _configurator = value; 
        }

        public string Interface 
        { 
            get => _interface; 
            set => _interface = value; 
        }

        public string LevelSignalU1Channel1 
        { 
            get => _levelSignalU1Channel1; 
            set => _levelSignalU1Channel1 = value; 
        }

        public string LevelSignalU2Channel1 
        { 
            get => _levelSignalU2Channel1; 
            set => _levelSignalU2Channel1 = value; 
        }

        public string LevelSignalU3Channel1 
        { 
            get => _levelSignalU3Channel1; 
            set => _levelSignalU3Channel1 = value; 
        }

        public string LevelSignalU1Channel2 
        { 
            get => _levelSignalU1Channel2; 
            set => _levelSignalU1Channel2 = value; 
        }

        public string LevelSignalU2Channel2 
        { 
            get => _levelSignalU2Channel2; 
            set => _levelSignalU2Channel2 = value; 
        }

        public string LevelSignalU3Channel2 
        { 
            get => _levelSignalU3Channel2; 
            set => _levelSignalU3Channel2 = value; 
        }

        public string Instraction 
        { 
            get => _instraction; 
            set => _instraction = value; 
        }

        public string ErrorChannel1 
        { 
            get => _errorChannel1; 
            set => _errorChannel1 = value; 
        }

        public string ErrorChannel2 
        { 
            get => _errorChannel2; 
            set => _errorChannel2 = value; 
        }

        public string ErrorSystem 
        { 
            get => _errorSystem; 
            set => _errorSystem = value; 
        }

        public string TemperatureChanel1 
        { 
            get => _temperatureChanel1; 
            set => _temperatureChanel1 = value; 
        }

        public string TemperatureChanel2 
        { 
            get => _temperatureChanel2; 
            set => _temperatureChanel2 = value; 
        }

        public string Temperature_T3 
        { 
            get => _temperature_T3; 
            set => _temperature_T3 = value; 
        }

        public string Temperature_T4_T5 
        { 
            get => _temperature_T4_T5; 
            set => _temperature_T4_T5 = value; 
        }

        public string Commissioning 
        { 
            get => _commissioning; 
            set => _commissioning = value; 
        }

        public string LowDifferenceTemp 
        { 
            get => _lowDifferenceTemp; 
            set => _lowDifferenceTemp = value; 
        }

        public string Pressure_P3 
        { 
            get => _pressure_P3; 
            set => _pressure_P3 = value; 
        }

        public string Pressure_P4_P5 
        { 
            get => _pressure_P4_P5; 
            set => _pressure_P4_P5 = value; 
        }

        public string Downtime 
        { 
            get => _downtime; 
            set => _downtime = value; 
        }

        public string RunningTime 
        { 
            get => _runningTime; 
            set => _runningTime = value; 
        }

        public string IrregularFlowTime 
        { 
            get => _irregularFlowTime; 
            set => _irregularFlowTime = value; 
        }

        public string ExcessFlowTime 
        { 
            get => _excessFlowTime; 
            set => _excessFlowTime = value; 
        }

        public string NoFlowTime 
        { 
            get => _noFlowTime; 
            set => _noFlowTime = value; 
        }

        public string NegativeFlowTime 
        { 
            get => _negativeFlowTime; 
            set => _negativeFlowTime = value; 
        }

        public string DefectTime 
        { 
            get => _defectTime; 
            set => _defectTime = value; 
        }

        public string NoPowerTime 
        { 
            get => _noPowerTime; 
            set => _noPowerTime = value; 
        }

        public string MaxSensorsPressure 
        { 
            get => _maxSensorsPressure; 
            set => _maxSensorsPressure = value; 
        }


        InvertedString invertedString = inputStr =>
        {
            string tempstr = "";
            for (int i = 0; i < inputStr.Length; i+=2)
            {
                tempstr = tempstr.Insert(0, inputStr.Substring(i, 2));
            }
            return tempstr;
        };
    }
}
