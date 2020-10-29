using System;
using System.IO;
using System.Net;
using System.Collections;
using System.Text.RegularExpressions;

namespace Parser
{
    class Period //класс периода
    {
        public DateTime date1; // дата 1
        public DateTime date2; // дата 2
        public float price;    // цена
        
        public  Period(DateTime date1, DateTime date2, float price)
        {
            this.date1 = date1;
            this.date2 = date2;
            this.price = price;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            string command, //команда
            link = "https://data.gov.ru/opendata/7710349494-urals/data-20200817T1010-structure-20200817T1010.csv?encoding=CP1251", //ссылка на скачивание
            path = "download/data.csv";//куда скачивать
            DateTime date1; //начаьная дата
            DateTime date2; //конечная дата
            long ticks;//для подсчета времени выполнения работы
            while (true)
            {
                Console.WriteLine("Введите команду:");
                command = Console.ReadLine();
                //api 1
                if (command == "1"){
                    ticks = DateTime.Now.Ticks;
                    ArrayList arrayPeriod = new ArrayList(); // обхявление массива периодов
                    FileLoad(link, path, ref arrayPeriod);// функция скачивание файла и переноса данных в массив
                    Console.Write("Введите дату: ");
                    date1 = DateTime.Parse(Console.ReadLine());
                    //Console.WriteLine(date1.ToString("dd.MM.yyyy"));
                    Console.WriteLine("Price: " +Search(date1, ref arrayPeriod)); //вывод цен на заданную дату
                    ticks = DateTime.Now.Ticks - ticks; //фискисрем время выполнеия
                    //Console.WriteLine("Время выполнения: "+ (ticks/1000000)/10.0 + " Дата: "+ DateTime.Today);

                }
                //api 2
                if (command == "2")
                {
                    ticks = DateTime.Now.Ticks;
                    ArrayList arrayPeriod = new ArrayList();
                    FileLoad(link, path, ref arrayPeriod);
                    Console.Write("Введите дату 1: ");
                    date1 = DateTime.Parse(Console.ReadLine());
                    Console.Write("Введите дату 2: ");
                    date2 = DateTime.Parse(Console.ReadLine());
                    //Console.WriteLine(date1.ToString("dd.MM.yyyy"));
                    //Console.WriteLine(date2.ToString("dd.MM.yyyy"));
                    Console.WriteLine("СРЕДНЯЯ Price: "+ Math.Round((Search2(date1, date2, ref arrayPeriod)),2));//вывод среднеф цены за период времени
                    ticks = DateTime.Now.Ticks - ticks;
                    //Console.WriteLine("Время выполнения: " + (ticks / 1000000) / 10.0);
                }
                //api 3
                if (command == "3")
                {
                    ticks = DateTime.Now.Ticks;
                    ArrayList arrayPeriod = new ArrayList();
                    FileLoad(link, path, ref arrayPeriod);
                    Console.Write("Введите дату 1: ");
                    date1 = DateTime.Parse(Console.ReadLine());
                    Console.Write("Введите дату 2: ");
                    date2 = DateTime.Parse(Console.ReadLine());
                    //Console.WriteLine(date1.ToString("dd.MM.yyyy"));
                    //Console.WriteLine(date2.ToString("dd.MM.yyyy"));
                    Search3(date1, date2, ref arrayPeriod);//вывд макс. и мин. цены за период времени
                    ticks = DateTime.Now.Ticks - ticks;
                    //Console.WriteLine("Время выполнения: " + (ticks / 1000000) / 10.0);
                }
                //api 4
                if (command == "4")
                {
                    ticks = DateTime.Now.Ticks;
                    ArrayList arrayPeriod = new ArrayList();
                    FileLoad(link, path, ref arrayPeriod);
                    FileJSON(path, ref arrayPeriod);// запись статиустики в json файл
                    ticks = DateTime.Now.Ticks - ticks;
                    //Console.WriteLine("Время выполнения: " + (ticks / 1000000) / 10.0);
                }
                //
                //if (command == "load") {
                //    ArrayList arrayPeriod = new ArrayList();
                //    FileLoad(link, path, ref arrayPeriod);
                //}
                // 
                if (command=="exit")
                    break;
            }
            Console.WriteLine("Конец");
        }

        static float Search (DateTime date, ref ArrayList arrayPeriod)
        { //вывд цены за определнную дату
            float price=0;
            foreach (Period period in arrayPeriod)
                if (period.date1 <= date && date <= period.date2)//Если дата попадает в период
                {
                    price=period.price;// то выводим цену
                    break;
                }
            return price;
        }
        //===

        static float Search2(DateTime tdate1, DateTime tdate2, ref ArrayList arrayPeriod)
        {//вывд сред. цены за период времени
            int q = 0;
            float price = 0;
            foreach (Period period in arrayPeriod)
                if (period.date1 <= tdate1 && tdate1 <= period.date2
                   || period.date1 < tdate2 && tdate2 > period.date2)//если первая дата вхдит в период
                {
                    //Console.WriteLine("Price: " + period.price);
                    q++;
                    price += period.price;//то начинам суммирование цен
                }
                else if (period.date1 <= tdate2 && tdate2 <= period.date2)//когда вторая дата входит в период
                {
                    //Console.WriteLine("Price: " + period.price);
                    q++;
                    price += period.price;
                    price= price / q;//то выводим среднию цену
                    break;
                }

            return price;
        }

        //==
        static void Search3(DateTime tdate1, DateTime tdate2, ref ArrayList arrayPeriod)//вывд макс. и мин. цены за период времени
        {
            string text;
            float min = float.MaxValue, max=0;
            foreach (Period period in arrayPeriod)
                if (period.date1 <= tdate1 && tdate1 <= period.date2
                        || period.date1 < tdate2 && tdate2 > period.date2)//если первая дата вхдит в период
                {
                //Console.WriteLine("Price: " + period.price);
                if (period.price < min)//определяем макс. и мин. цены
                    min = period.price;
                if (period.price > max)
                    max = period.price;
            }
            else if (period.date1 <= tdate2 && tdate2 <= period.date2)//когда вторая дата входит в период
                {
                //Console.WriteLine("Price: " + period.price);
                if (period.price < min)
                    min = period.price;
                if (period.price > max)
                    max = period.price;
                    Console.WriteLine("min Price: " + min + " max Price: " + max);//вывод макс. и мин. цены
                    break;
            }
            text = "{\n" +
               "   \"tdate1\": \"" + tdate1.ToString("dd.MM.yyyy") + "\"," +
               "\n \"tdate2\": \"" + tdate2.ToString("dd.MM.yyyy") + "\"," +
               "\n \"max\": \"" + max + "\"," +
               "\n \"min\": \"" + min + "\"" +
               "\n}";
            using (StreamWriter sw = new StreamWriter("download / api3result.json", false, System.Text.Encoding.Default))
            {
                        sw.WriteLine(text);//запись в файл
            }
            Console.WriteLine("файл выгружен");
            return; 
        }

        //==

        static void FileJSON(string path, ref ArrayList arrayPeriod)
        {
                int q = 0;
                string text = "{\n";
                float price = 0;
                foreach (Period period in arrayPeriod)//Составление данных файла JSON
                {
                    text += "\"Period" + q + "\" :{\n";
                    text += "             \"price\": \"" + price + "\",\n";
                    text += "             \"date1\": \"" + period.date1.ToString("dd.MM.yyyy") + "\",\n";
                    text += "             \"date2\": \"" + period.date2.ToString("dd.MM.yyyy") + "\"\n";
                    text += "},\n";
                    q++;
                }
                    
                text = text.Remove(text.Length - 2);
                text += "\n}";
                using (StreamWriter sw = new StreamWriter("download / result.json", false, System.Text.Encoding.Default))
                {
                    sw.WriteLine(text);//запись в файл
                }
                Console.WriteLine("файл выгружен");
                return;
        }
        //==
        static void FileLoad(string link, string path, ref ArrayList arrayPeriod)// запись статистики в json файл
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(new Uri(link), path);
            }
            //Console.WriteLine("Файл скачен");
            //==
            
            using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
            {
                string line;
                float price = 0;
                string[] strmouth = { "янв",
                                       "фев",
                                       "мар",
                                       "апр",
                                       "май",
                                       "июн",
                                       "июл",
                                       "авг",
                                       "сен",
                                       "окт",
                                       "ноя",
                                       "дек" };
                //Месяца нужны для преобразования даты файла
                while ((line = sr.ReadLine()) != null)//читем строку
                {
                    Regex regex;
                    for (int k = 0; k < 2; k++)//в строке 2 даты, поэтому просматриваем ее 2ажды
                    {
                        int i = 0;
                        while (i < 12 && !Regex.IsMatch(line, @"\d+[.]{1}" + strmouth[i] + @"[.]{1}\d+", RegexOptions.IgnoreCase))//если есть дата
                            i++;
                        if (i < 12)
                        {
                            regex = new Regex(@"" + strmouth[i]);//Преобразуем месяц к циферному формату
                            line = regex.Replace(line,
                                              (i + 1) < 10 ? '0' + (i + 1).ToString() : (i + 1).ToString());
                        }
                    }

                    if (Regex.IsMatch(line, @"\d+[.]{1}\d+[.]{1}\d+", RegexOptions.IgnoreCase))
                    {//выделяем дату из строки
                        regex = new Regex(@"\d+[.]{1}\d+[.]{1}\d+");
                        MatchCollection matches = regex.Matches(line);
                        DateTime date1 = DateTime.Parse(matches[0].Value);//и записываем в переменные
                        DateTime date2 = DateTime.Parse(matches[1].Value);

                        regex = new Regex(@"\d+[,]?\d*\S{1}$");
                        matches = regex.Matches(line);
                        regex = new Regex(@"\d+[,]?\d*");//выделяем цену
                        matches = regex.Matches(matches[0].Value);
                        price = float.Parse((matches[0].Value).ToString());// и такэе записываем в переменную

                        Period period = new Period(date1, date2, price);//создаем класс период
                        arrayPeriod.Add(period);//и записываем его в массив
                    }
                }
            }
        }
  
    }
}
