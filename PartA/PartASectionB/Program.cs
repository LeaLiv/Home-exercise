using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace PartASectionB
{
    public class Program
    {
        const string DATA_FORMAT = "dd/MM/yyyy HH:mm";
        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Wrong number of arguments");
                return;
            }
            var filePath = args[0];
            filePath = filePath != null ? filePath.Trim() : "";
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File " + filePath + " doesn't exists");
                return;
            }

            CheckingBeforeDataProcessing(filePath);
        }

        public static void CheckingBeforeDataProcessing(string filePath)
        {
            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    var errors = new List<string>();
                    var rowNumber = 0;
                    var dateTimesRows = new Hashtable();                    

                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        rowNumber++;
                        if (rowNumber == 1)
                        {
                            continue;
                        }
                        
                        var values = line.Trim().Split(',');

                        if (values.Length != 2)
                        {
                            errors.Add("Wrong colunms for line: " + line + " in row " + rowNumber);
                            continue;
                        }

                        var dateTimeStr = values[0].Trim();
                        if (DateTime.TryParseExact(dateTimeStr, DATA_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                        {
                            AddDate(dateTimeStr, dateTimesRows, rowNumber);
                        }
                        else
                        {
                            errors.Add("Wrong date format for date time " + dateTimeStr + " in row " + rowNumber);
                        }

                        var valueStr = values[1].Trim();
                        if (double.TryParse(valueStr, out double value))
                        {
                            if (value < 0)
                            {
                                errors.Add("Negative value " + valueStr + " in row " + rowNumber);
                            }                            
                        }
                        else
                        {
                            errors.Add("Wrong type for value " + valueStr + " in row " + rowNumber);
                        }
                    }
                 
                    foreach (string key in dateTimesRows.Keys)
                    {
                        var value = (string)dateTimesRows[key];
                        if (value.Split(',').Length > 1)
                        {
                            errors.Add("Dublicate date time " +  key + " in rows " + value);
                        }
                    }
                    
                    Console.WriteLine(string.Join(Environment.NewLine, errors.ToArray()));
                    Console.WriteLine("Total rows: " + +rowNumber);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        private static void AddDate(string dateTime, Hashtable dateTimesCount, int rowNumber)
        {
            if (dateTimesCount.ContainsKey(dateTime))
            {
                dateTimesCount[dateTime] = (string)dateTimesCount[dateTime] + "," + rowNumber;
            }
            else
            {
                dateTimesCount.Add(dateTime, rowNumber.ToString());
            }
        }      
    }
}
