using ParquetSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PartASectionB
{
    public partial class Program
    {
        const string DATA_FORMAT = "dd/MM/yyyy HH:mm";
        const string SPLITED_FILES_DIR = "SplitedFiles";
        const bool PRINT_ERRORS = false;

        public enum FileTypes
        {
            Unknow,
            Csv,
            Parquet
        }

        public static async Task Main(string[] args)
        {
            Console.WriteLine("Enter file path:");
            var filePath = Console.ReadLine();
            filePath = filePath != null ? filePath.Trim() : "";
                        
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File " + filePath + " doesn't exists");
                return;
            }              

            var fileNameData = GetFileNameData(filePath);

            if (fileNameData.FileType == FileTypes.Unknow)
            {  
                Console.WriteLine("Wront extension of file " + filePath); 
                return;
            }

            // 1a
            CheckingBeforeDataProcessing(fileNameData, PRINT_ERRORS);

            // 1b
            ComputeAvg(fileNameData);

            // 2
            var filesData = SeparateFile(fileNameData);
            await ComputeAvgByFilesAsync(filesData);
        }

        #region 2     

        private static FileNameData SeparateFile(FileNameData fileNameData)
        {
            if (Directory.Exists(fileNameData.FilesDir))
            {                
                Directory.Delete(fileNameData.FilesDir, true);               
            }

            Directory.CreateDirectory(fileNameData.FilesDir);

            try
            {
                var allLines = GetAllLines(fileNameData);
                var errors = new List<string>();
                var dateTimes = new HashSet<string>();
                var filesData= new Hashtable();

                var rowNumber = 0;               
                var fileNameDateFormat = "{0}_{1}_{2}";
                var titleColums = "";
                foreach (var line in allLines)
                {
                    rowNumber++;
                    if (rowNumber == 1)
                    {
                        titleColums = line;
                        continue;
                    }
                    if (!Validate(rowNumber, line, errors, dateTimes, out DateTime curDateTime, out double value))
                    {
                        continue;
                    }

                    var dateFileName = string.Format(fileNameDateFormat, curDateTime.Date.Day, curDateTime.Month, curDateTime.Year);

                    if (filesData.ContainsKey(dateFileName))
                    {
                        List<string> fileLines = (List<string>)filesData[dateFileName];
                        fileLines.Add(line);
                    }
                    else
                    {
                        filesData.Add(dateFileName, new List<string>(new string[] { titleColums, line }));
                    }
                }

                string fileNameFormat = fileNameData.FileName + "_{0}" + fileNameData.FileExt;
                var totalRows = 0;
                foreach (string key in filesData.Keys)
                {
                    var splitFilePath = Path.Combine(fileNameData.FilesDir, string.Format(fileNameFormat, key));
                    var dataArr = ((List<string>)filesData[key]).ToArray();
                    totalRows += dataArr.Length;
                    File.WriteAllLines(splitFilePath, dataArr);
                }

                Console.WriteLine("SeparateFile: Total Rows data: " + (totalRows - filesData.Count));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

            return fileNameData;
        }

        private static async Task ComputeAvgByFilesAsync(FileNameData fileNameData)
        {
            try
            {
                var fileNames = Directory.GetFiles(fileNameData.FilesDir);
                var filesCount = fileNames.Length;
                var tasks = new Task[filesCount];
                var errors = new List<string>();
                var filesData = new Hashtable();

                var index = 0;               
                foreach (var fileName in fileNames)
                {
                    tasks[index] = Task.Run(() => ReadFile(fileNameData, filesData, errors));
                    index++;
                }               

                await Task.WhenAll(tasks);                    
                                
                IEnumerable<KeyValuePair<DateTime, double>> results = null;
                foreach (string key in filesData.Keys)
                {
                    var fileData = (SortedList<DateTime, double>)filesData[key];
                    if (results == null)
                    {
                        results = fileData;
                    }
                    else
                    {
                        results = results.Union(fileData);
                    }
                }

                var resultsFilePath = Path.Combine(fileNameData.FilesDir, fileNameData.FileName + "Results" + fileNameData.FileExt);
                var lineFormat = "{0}, {1}";
                var avgData = new SortedList<DateTime, double>(results.ToDictionary(k => k.Key, v => v.Value));
                var totalRows = 0;
                using (var sw = new StreamWriter(resultsFilePath))
                {
                    sw.WriteLine(string.Format(lineFormat, "Start Time", "Avarage"));
                    foreach (var item in avgData)
                    {
                        totalRows++;
                        sw.WriteLine(string.Format(lineFormat, item.Key.ToString(DATA_FORMAT), item.Value));
                    }
                }

                Console.WriteLine("ComputeAvgByFilesAsync: Total final rows: " + totalRows);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        private static void ReadFile(FileNameData fileNameData, Hashtable filesData, List<string> errors)
        {
            var avgByDateTime = new SortedList<DateTime, double>();

            var fileName = Path.GetFileNameWithoutExtension(fileNameData.FilePath);

            filesData[fileName] = ComputeFileAvg(fileNameData, errors);
        }

        struct FileNameData
        {
            public string FilePath;
            public string FilesDir;
            public string FileName;
            public string FileExt;
            public FileTypes FileType;
        }

        #endregion

        #region 1b        

        private static void ComputeAvg(FileNameData fileNameData)
        {
            try
            {
                var errors = new List<string>();
                var avgByDateTime = ComputeFileAvg(fileNameData, errors);

                var tableWidth = 50;
                PrintLine(tableWidth);
                PrintRow(tableWidth, "Start Time", "Avarage");
                PrintLine(tableWidth);
                var totalRows = 0;
                foreach (var item in avgByDateTime)
                {
                    totalRows++;
                    double avg = item.Value;
                    PrintRow(tableWidth, item.Key.ToString(DATA_FORMAT), avg.ToString());
                    PrintLine(tableWidth);
                }

                Console.WriteLine("ComputeAvg: Total print rows: " + totalRows);                

                //Console.WriteLine(string.Join(Environment.NewLine, errors.ToArray()));
                //Console.WriteLine("Total rows: " + +rowNumber);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        private static void PrintLine(int tableWidth)
        {
            Console.WriteLine(new string('-', tableWidth));
        }

        private static void PrintRow(int tableWidth, params string[] columns)
        {
            int width = (tableWidth - columns.Length) / columns.Length;
            string row = "|";

            foreach (string column in columns)
            {
                row += AlignCentre(column, width) + "|";
            }

            Console.WriteLine(row);
        }

        private static string AlignCentre(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
            }
        }

        #endregion

        #region 1a

        private static void CheckingBeforeDataProcessing(FileNameData fileNameData, bool printErrors)
        {
            try
            {
                var filePath = fileNameData.FilePath;
                var errors = new List<string>();
                var totalValidRows = 0;
                var dateTimes = new HashSet<string>();
                var rowNumber = 0;

                var lines = GetAllLines(fileNameData);

                foreach (var line in lines)
                {
                    rowNumber++;
                    if (fileNameData.FileType == FileTypes.Csv && rowNumber == 1)
                    {
                        continue;
                    }

                    if (!Validate(rowNumber, line, errors, dateTimes, out DateTime date, out double value))
                    {
                        continue;
                    }

                    totalValidRows++;
                }

                if (printErrors)
                {
                    Console.WriteLine(string.Join(Environment.NewLine, errors.ToArray()));
                }
                Console.WriteLine("CheckingBeforeDataProcessing: Total valid rows: " + totalValidRows + " of total rows: " + (rowNumber - 1));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }      

        #endregion

        #region General Methods

        private static bool Validate(int lineNumber, string line, List<string> errors, HashSet<string> dateTimes, out DateTime curDateTime, out double value)
        {
            curDateTime = default;
            value = -1;
            var values = line.Trim().Split(',');

            if (values.Length != 2)
            {
                errors.Add("Wrong colunms for line: " + line + " in row " + lineNumber);
                return false;
            }

            var dateTimeStr = values[0].Trim();
            if (DateTime.TryParseExact(dateTimeStr, DATA_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out curDateTime))
            {
                if (dateTimes.Contains(dateTimeStr))
                {
                    errors.Add("Dublicate date time " + dateTimeStr);
                    return false;
                }
            }
            else
            {
                errors.Add("Wrong date format for date time " + dateTimeStr + " in row " + lineNumber);
                return false;
            }

            var valueStr = values[1].Trim();
            if (double.TryParse(valueStr, out value))
            {
                if (value < 0)
                {
                    errors.Add("Negative value " + valueStr + " in row " + lineNumber);
                    return false;
                }

                if (value.Equals(double.NaN))
                {
                    errors.Add("NaN value " + valueStr + " in row " + lineNumber);
                    return false;
                }
            }
            else
            {
                errors.Add("Wrong type for value " + valueStr + " in row " + lineNumber);
                return false;
            }

            return true;
        }

        private static SortedList<DateTime, double> ComputeFileAvg(FileNameData fileNameData, List<string> errors)
        {
            var avgByDateTime = new SortedList<DateTime, double>();
            var dataByDateTime = new SortedList<DateTime, (double value, int count)>();

            var lines = GetAllLines(fileNameData);

            var rowNumber = 0;
            var dateTimes = new HashSet<string>();

            foreach (var line in lines)
            {
                rowNumber++;
                if (fileNameData.FileType == FileTypes.Csv && rowNumber == 1)
                {
                    continue;
                }

                if (!Validate(rowNumber, line, errors, dateTimes, out DateTime curDateTime, out double value))
                {
                    continue;
                }

                var startDateTime = new DateTime(curDateTime.Year, curDateTime.Month, curDateTime.Day, curDateTime.Hour, 0, 0);

                if (dataByDateTime.ContainsKey(startDateTime))
                {
                    dataByDateTime[startDateTime] = (dataByDateTime[startDateTime].value + value, dataByDateTime[startDateTime].count + 1);
                }
                else
                {
                    dataByDateTime.Add(startDateTime, (value, 1));
                }
            }      
            
            foreach (var item in dataByDateTime)
            {
                avgByDateTime.Add(item.Key, item.Value.value / item.Value.count);
            }

            return avgByDateTime;
        }

        private static FileNameData GetFileNameData(string filePath)
        {
            var fileNameData = new FileNameData();
            fileNameData.FilePath = filePath;
            fileNameData.FilesDir = Path.Combine(Path.GetDirectoryName(filePath), SPLITED_FILES_DIR);
            fileNameData.FileName = Path.GetFileNameWithoutExtension(filePath);
            fileNameData.FileExt = Path.GetExtension(filePath);

            var extWitoutPoint = fileNameData.FileExt.Replace(".", "").ToLower();
            var fileType = FileTypes.Unknow;
            switch (extWitoutPoint)
            {
                case "csv":
                    fileType = FileTypes.Csv;
                    break;
                case "parquet":
                    fileType = FileTypes.Parquet;
                    break;
                default:
                    break;
            }

            fileNameData.FileType = fileType;

            return fileNameData;
        }

        private static string[] GetAllLines(FileNameData fileNameData)
        {
            if (fileNameData.FileType == FileTypes.Csv)
            {
                return File.ReadLines(fileNameData.FilePath).ToArray();
            }
            else if (fileNameData.FileType == FileTypes.Parquet)
            {
                var lines = new List<string>();
                using (var file = new ParquetFileReader(fileNameData.FilePath))
                {
                    for (int rowGroup = 0; rowGroup < file.FileMetaData.NumRowGroups; ++rowGroup)
                    {
                        using (var rowGroupReader = file.RowGroup(rowGroup))
                        {
                            var groupNumRows = checked((int)rowGroupReader.MetaData.NumRows);

                            var groupTimestamps = rowGroupReader.Column(0).LogicalReader<string>().ReadAll(groupNumRows);
                            var groupValues = rowGroupReader.Column(2).LogicalReader<string>().ReadAll(groupNumRows);

                            for (int i = 0; i < groupNumRows; ++i)
                            {
                                lines.Add(groupTimestamps[i] + ", " + groupValues[i]);
                            }
                        }
                    }

                    file.Close();
                }

                return lines.ToArray();
            }

            return null;
        }


        #endregion
    }
}
