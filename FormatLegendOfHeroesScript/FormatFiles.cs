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
                DirectoryInfo di = new DirectoryInfo(@".\bin\Debug\netcoreapp1.1\");

                foreach (FileInfo fi in di.EnumerateFiles("*.Dat"))
                {
                    //string filename = @"C:\Users\Michael\Documents\visual studio 2017\Projects\FormatLegendOfHeroesScript\FormatLegendOfHeroesScript\bin\Debug\netcoreapp1.1\T_131.DAT";
                    //string filenameOut = @".\bin\Debug\netcoreapp1.1\out\T_131.txt";
                    string filenameOut = fi.DirectoryName + @"\out\" + fi.Name;
                   DirectoryInfo dirOut = new DirectoryInfo(@".\bin\Debug\netcoreapp1.1\out\");

                    string replacedByteArray;
                    replacedByteArray = GetModifiedFile(fi);
                    CreateFile(filenameOut, replacedByteArray);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        private static string GetModifiedFile(FileInfo fileInfo)
        {
            string replacedByteArray;
            using (FileStream fs = new FileStream(fileInfo.FullName, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs, System.Text.CodePagesEncodingProvider.Instance.GetEncoding(932)))
                {

  //0x1E is a new block of text
                    //0x01 is a line end
                    //0x0D is a newline
                    //0x000000ffff00 seems to indicate the end of the text section.

                    //so from 0x1e to 0x1e is a text section.  Can open this is Open Office Calc.  Using Shift JIS and separate by commas.

                    //replacedByteArray = sr.ReadToEnd().Replace((char)0x00, (char)0xE1);
                    // replacedByteArray = sr.ReadToEnd();
                    //replacedByteArray = sr.ReadToEnd().Replace((char)0x00, (char)0x0D); //works

                    string twoLinebreaks = ((char)0x0D).ToString();
                    twoLinebreaks += twoLinebreaks;

                    

                    replacedByteArray = sr.ReadToEnd();

                    //org
                   // replacedByteArray = replacedByteArray.Replace((char)0x01, (char)0x0D); //if you try and have 4 hex digits as a character it will not work for shift-jis
                    //replacedByteArray = replacedByteArray.Replace(((char)0x1E).ToString(), twoLinebreaks);//org
                    replacedByteArray = replacedByteArray.Replace(((char)0x1E), (char)0x2C); //test
                  
                }
            }

            return replacedByteArray;
        }

        private static void CreateFile(string filenameOut, string replacedByteArray)
        {

            using (FileStream fs = new FileStream(filenameOut, FileMode.Create))
            using (StreamWriter sw = new StreamWriter(fs, System.Text.CodePagesEncodingProvider.Instance.GetEncoding(932)))
            {
                sw.WriteLine(replacedByteArray);
            }
        }


    }

}
