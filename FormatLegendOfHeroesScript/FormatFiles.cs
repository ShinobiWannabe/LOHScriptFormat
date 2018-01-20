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

                DirectoryInfo di = new DirectoryInfo(@".\bin\Debug\netcoreapp1.1\");

                string output = "Filename,Speaker,Game_TextBlock,NoSpeaker_TextBlock\n";

                DeleteOutputIfExists(di);

                foreach (FileInfo fi in di.EnumerateFiles("*.Dat"))
                {
                    //string filenameOut = fi.DirectoryName + @"\out\" + fi.Name.Replace(".DAT", ".csv");
                    DirectoryInfo dirOut = new DirectoryInfo(@".\bin\Debug\netcoreapp1.1\out\");

                    //string replacedByteArray;
                    output += GetModifiedFile(fi);                    
                    WriteFile(fi.DirectoryName + @"\out\output.csv", output);
                    output = string.Empty;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void DeleteOutputIfExists(DirectoryInfo di)
        {
            try
            {
                File.Delete(di.FullName + @"\out\output.csv");
            }
            catch (Exception)
            {
                Console.WriteLine("output.csv exists and is probably in use. Canno delete to overwrite.");
            }
          
        }

        private static string GetModifiedFile(FileInfo fileInfo)
        {
            string output = string.Empty;
            using (FileStream fs = new FileStream(fileInfo.FullName, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs, System.Text.CodePagesEncodingProvider.Instance.GetEncoding(932)))
                {
                    int speakerIndex = -1;
                    string line = string.Empty;
                    while (!sr.EndOfStream)
                    {
                        char chr = (char)sr.Read();
                        line += chr;
                        speakerIndex = -1;
                        if (chr == GAME_TEXTBLOCKEND)
                        {
                            //maybe input a comma on 0x0401. which seems to be after the top of the text block indicating who is speaking. 
                            speakerIndex = line.IndexOf((char)(0x04));

                            output = GetFileName(fileInfo, output);
                            output = GetSpeaker(output, line, speakerIndex);
                            output = GetLine(output, line);
                            output = GetLineWithoutSpeaker(output, line, speakerIndex);

                            output += NEWLINE;

                            line = string.Empty;
                            speakerIndex = -1;
                        }
                    }

                }
            }

            return output;
        }

        private static string GetFileName(FileInfo fileInfo, string output)
        {
            output += fileInfo.Name;
            output += COMMA;
            return output;
        }

        private static string GetSpeaker(string output, string line, int speakerIndex)
        {
            output += speakerIndex > -1 ? line.Substring(0, speakerIndex + 1) : string.Empty;
            output += COMMA;
            return output;
        }


        private static string GetLine(string output, string line)
        {
            output += line;
            output += COMMA;
            return output;
        }

        private static string GetLineWithoutSpeaker(string replacedByteArray, string line, int speakerIndex)
        {
            replacedByteArray += speakerIndex > -1 ? line.Substring(speakerIndex, line.Length - speakerIndex) : line;
            replacedByteArray += COMMA;
            return replacedByteArray;
        }

        private static void WriteFile(string filenameOut, string output)
        {       
            using (FileStream fs = new FileStream(filenameOut, FileMode.Append))
            using (StreamWriter sw = new StreamWriter(fs, System.Text.CodePagesEncodingProvider.Instance.GetEncoding(932)))
            {
                sw.WriteLine(output);
            }
        }

        //older
        //private static void CreateFile(string filenameOut, string replacedByteArray)
        //{

        //    using (FileStream fs = new FileStream(filenameOut, FileMode.Create))
        //    using (StreamWriter sw = new StreamWriter(fs, System.Text.CodePagesEncodingProvider.Instance.GetEncoding(932)))
        //    {
        //        sw.WriteLine(replacedByteArray);
        //    }
        //}


    }

}
