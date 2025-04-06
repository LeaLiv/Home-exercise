
using Microsoft.VisualBasic.FileIO;

namespace PartA;

internal class SectionB
{
    const string filePath = @"C:\Lea\Home exercise\files\time_series.csv";

    public static void CheckingBeforeDataProcessing()
    {
        try
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File not found");
                return;
            }
            //using (TextFieldParser csvParser = new TextFieldParser(filePath))
            //{
            //    while(!sw.EndOfStream)
            //    {
            //        Console.WriteLine(sw.ReadLine());
            //    }
            //}
        }
        catch (Exception e)
        {

            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
        }

    }
    //public static void Main(string[] args)
    //{
    //    CheckingBeforeDataProcessing();
    //}
}
