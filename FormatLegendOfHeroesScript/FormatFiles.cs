using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace FormatLegendOfHeroesScript
{
    class FormatFiles
    {
        const char COMMA = (char)0x2C;
        const char NEWLINE = (char)0x0D;

        const char GAME_NEWLINE = (char)0x01;
        const char GAME_TEXTBLOCKEND = (char)0x1E;

        //0x000000ffff00 seems to indicate the end of the text section.

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
                    replacedByteArray = "FILENAME,GAME_SPEACHBLOCK\n";
                    // replacedByteArray = GetModifiedFile(fi);
                    replacedByteArray = GetRegexModifiedFile(fi);
                    CreateFile(filenameOut, replacedByteArray);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        private static string GetRegexModifiedFile(FileInfo fileInfo)
        {
            string replacedByteArray =string.Empty ;
            List<string> speachBlocks =new List<string>();
            using (FileStream fs = new FileStream(fileInfo.FullName, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs, System.Text.CodePagesEncodingProvider.Instance.GetEncoding(932)))
                {


                    //string twoLinebreaks = NEWLINE.ToString();
                    //twoLinebreaks += twoLinebreaks;

                    //string commaAndNewLine = COMMA.ToString() + NEWLINE.ToString();



                    //replacedByteArray = sr.ReadToEnd();


                    //string pattern = "GAME_TEXTBLOCKEND; 
                    string temp;
                    
                        string line = string.Empty; //= (string)sr.ReadLine();
                    while (!sr.EndOfStream )
                    {
                      


                         char chr = (char)sr.Read();
                        line += chr;
                        //speachBlocks.Add(line);
                        if (chr == GAME_TEXTBLOCKEND)
                        {

                            //maybe input a comma on 0x0401. which seems to be after the top of the text block indicating who is speaking. 
                            replacedByteArray += fileInfo.Name + ",";
                            replacedByteArray += line;
                            replacedByteArray += NEWLINE;
                            line = string.Empty;
                        }

                    }

                    //org
                    // replacedByteArray = replacedByteArray.Replace((char)0x01, (char)0x0D); //if you try and have 4 hex digits as a character it will not work for shift-jis
                    //replacedByteArray = replacedByteArray.Replace(((char)0x1E).ToString(), twoLinebreaks);//org
                    // replacedByteArray = replacedByteArray.Replace(((char)0x1E), (char)0x2C); //best looking so far
                   // replacedByteArray = replacedByteArray.Replace(GAME_TEXTBLOCKEND, COMMA);





                }
            }
            
            return replacedByteArray ;
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

                    string twoLinebreaks = NEWLINE.ToString();
                    twoLinebreaks += twoLinebreaks;

                    string commaAndNewLine = COMMA.ToString() + NEWLINE.ToString();



                    replacedByteArray = sr.ReadToEnd();

                    //org
                    // replacedByteArray = replacedByteArray.Replace((char)0x01, (char)0x0D); //if you try and have 4 hex digits as a character it will not work for shift-jis
                    //replacedByteArray = replacedByteArray.Replace(((char)0x1E).ToString(), twoLinebreaks);//org
                    // replacedByteArray = replacedByteArray.Replace(((char)0x1E), (char)0x2C); //best looking so far
                    replacedByteArray = replacedByteArray.Replace(GAME_TEXTBLOCKEND, COMMA);

                    



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
