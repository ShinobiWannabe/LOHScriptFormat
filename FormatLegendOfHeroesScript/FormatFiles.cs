using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FormatLegendOfHeroesScript
{
    class FormatFiles
    {



        public void Format()
        {

            try
            {

                //https://msdn.microsoft.com/en-us/library/system.text.encoding.codepage(v=vs.110).aspx#See Also
                //https://www.nuget.org/packages/System.Text.Encoding.CodePages/

               // string line;
                DirectoryInfo di = new DirectoryInfo(@"C: \Users\Michael\Documents\visual studio 2017\Projects\FormatLegendOfHeroesScript\FormatLegendOfHeroesScript\bin\Debug\netcoreapp1.1\");

                foreach (FileInfo fi in di.EnumerateFiles())
                {
                string filename = @"C:\Users\Michael\Documents\visual studio 2017\Projects\FormatLegendOfHeroesScript\FormatLegendOfHeroesScript\bin\Debug\netcoreapp1.1\T_131.DAT";
                string filenameOut = @"C:\Users\Michael\Documents\visual studio 2017\Projects\FormatLegendOfHeroesScript\FormatLegendOfHeroesScript\bin\Debug\netcoreapp1.1\out\T_131.txt";
                string replacedByteArray;
                replacedByteArray = GetModifiedFile(filename);
                CreateFile(filenameOut, replacedByteArray);
                }
             
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static void CreateFile(string filenameOut, string replacedByteArray)
        {
            using (FileStream fs = new FileStream(filenameOut, FileMode.Create))
            using (StreamWriter sw = new StreamWriter(fs, System.Text.CodePagesEncodingProvider.Instance.GetEncoding(932)))
            {
                sw.WriteLine(replacedByteArray);
            }
        }

        private static string GetModifiedFile(string filename)
        {
            string replacedByteArray;
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs, System.Text.CodePagesEncodingProvider.Instance.GetEncoding(932)))
                {
                    //replacedByteArray = sr.ReadToEnd().Replace((char)0x00, (char)0xE1);
                    // replacedByteArray = sr.ReadToEnd();
                    //replacedByteArray = sr.ReadToEnd().Replace((char)0x00, (char)0x0D); //works

                    string twoLinebreaks = ((char)0x0D).ToString();
                    twoLinebreaks += twoLinebreaks;

                    replacedByteArray = sr.ReadToEnd().Replace((char)0x01, (char)0x0D); //if you try and have 4 hex digits as a character it will not work for shift-jis
                    replacedByteArray = replacedByteArray.Replace(((char)0x1E).ToString(), twoLinebreaks);
                  
                }
            }

            return replacedByteArray;
        }

        //public void WorkingFormat()
        //{
        //    //https://msdn.microsoft.com/en-us/library/system.text.encoding.codepage(v=vs.110).aspx#See Also
        //    //https://www.nuget.org/packages/System.Text.Encoding.CodePages/

        //    string line;
        //    string filename = @"C:\Users\Michael\Documents\visual studio 2017\Projects\FormatLegendOfHeroesScript\FormatLegendOfHeroesScript\bin\Debug\netcoreapp1.1\T_131.DAT";
        //    string filenameOut = @"C:\Users\Michael\Documents\visual studio 2017\Projects\FormatLegendOfHeroesScript\FormatLegendOfHeroesScript\bin\Debug\netcoreapp1.1\T_131.txt";
        //    // byte[] data = System.IO.File.ReadAllBytes(filename);

        //    //Console.WriteLine(Encoding.UTF8.GetString(data));
        //    //Console.WriteLine(Encoding.UTF7.GetString(data));
        //    //Console.WriteLine(Encoding.ASCII.GetString(data));
        //    // string dataStr = data.ToString();
        //    //dataStr.Replace((char)0x00, ' ');
        //    //System.IO.File.WriteAllBytes(filenameOut,(byte[])dataStr.ToCharArray().CopyTo();
        //    //byte[] dataBytesToWrite;
        //    string replacedByteArray;
        //    using (FileStream fs = new FileStream(filename, FileMode.Open))
        //    {
        //        using (StreamReader sr = new StreamReader(fs, System.Text.CodePagesEncodingProvider.Instance.GetEncoding(932)))
        //        // using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
        //        {
        //            //replacedByteArray = sr.ReadToEnd().Replace((char)0x00, (char)0xE1);
        //            replacedByteArray = sr.ReadToEnd().Replace((char)0x00, (char)0xE1E1);
        //            //dataBytesToWrite = System.Text.CodePagesEncodingProvider.Instance.GetEncoding(932).GetBytes(replacedByteArray);
        //            //writingTo = Encoding.UTF8.GetBytes(replacedByteArray); //works
        //            //Encoding.Convert(utf8, iso, utfBytes);
        //            //byte[] writingTo = Encoding.UTF7  .GetBytes(replacedByteArray);
        //            // sr.write
        //            //System.IO.File.WriteAllBytes(filenameOut, writingTo);
        //            //while (!sr.EndOfStream)
        //            //{
        //            //    line = sr.ReadLine();
        //            //    Console.WriteLine(line);

        //            //}
        //        }
        //    }
        //    using (FileStream fs = new FileStream(filenameOut, FileMode.Create))
        //    using (StreamWriter sw = new StreamWriter(fs, System.Text.CodePagesEncodingProvider.Instance.GetEncoding(932)))
        //    {
        //        sw.WriteLine(replacedByteArray);
        //    }

        //}


        //public void TestingFormat()
        //{
        //    //https://msdn.microsoft.com/en-us/library/system.text.encoding.codepage(v=vs.110).aspx#See Also
        //    //https://www.nuget.org/packages/System.Text.Encoding.CodePages/

        //    string line;
        //    string filename = @"C:\Users\Michael\Documents\visual studio 2017\Projects\FormatLegendOfHeroesScript\FormatLegendOfHeroesScript\bin\Debug\netcoreapp1.1\T_131.txt";

        //    byte[] data = System.IO.File.ReadAllBytes(filename);

        //    Console.WriteLine(Encoding.UTF8.GetString(data));
        //    Console.WriteLine(Encoding.UTF7.GetString(data));
        //    Console.WriteLine(Encoding.ASCII.GetString(data));


        //    using (FileStream fs = new FileStream(filename, FileMode.Open))
        //    //using (StreamReader sr = new StreamReader(fs, System.Text.CodePagesEncodingProvider.Instance.GetEncoding(932)))

        //    //using (StreamReader sr = new StreamReader(fs, System.Text.CodePagesEncodingProvider.Instance.GetEncoding("Shift-JIS")))
        //    //using (StreamReader sr = new StreamReader(fs, System.Text.CodePagesEncodingProvider.Instance.GetEncoding("windows-874")))
        //    // using (StreamReader sr = new StreamReader(fs, System.Text.CodePagesEncodingProvider.Instance.GetEncoding("gb2312")))
        //    // using (StreamReader sr = new StreamReader(fs, System.Text.CodePagesEncodingProvider.Instance.GetEncoding("ks_c_5601-1987")))
        //    //using (StreamReader sr = new StreamReader(fs, System.Text.CodePagesEncodingProvider.Instance.GetEncoding("big5")))

        //    //using (StreamReader sr = new StreamReader(fs, System.Text.CodePagesEncodingProvider.Instance.GetEncoding("utf-16"))) //does not work
        //    //foreach(Encoding.GetEncoding())
        //    //using (StreamReader sr = new StreamReader(fs, Encoding.ASCII))
        //    //using (StreamReader sr = new StreamReader(fs, Encoding.BigEndianUnicode))
        //    //using (StreamReader sr = new StreamReader(fs, Encoding.UTF7))
        //    //using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
        //    //            using (StreamReader sr = new StreamReader(fs, Encoding.UTF32))
        //    // using (StreamReader sr = new StreamReader(fs, Encoding.Unicode))
        //    //using (StreamReader sr = new StreamReader(fs, System.Text.CodePagesEncodingProvider.Instance.GetEncoding("windows-1250")))
        //    //using (StreamReader sr = new StreamReader(fs, System.Text.CodePagesEncodingProvider.Instance.GetEncoding("windows-1251")))



        //    //using (StreamReader sr = new StreamReader(fs, System.Text.CodePagesEncodingProvider.Instance.GetEncoding("windows-1252")))


        //    using (StreamReader sr = new StreamReader(fs, System.Text.CodePagesEncodingProvider.Instance.GetEncoding(932)))
        //    {
        //        while (!sr.EndOfStream)
        //        {
        //            line = sr.ReadLine();
        //            Console.WriteLine(line);

        //        }
        //    }
        //}
    }

}
