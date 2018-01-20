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

                foreach (FileInfo fi in di.EnumerateFiles("*.Dat"))
                {
                    string filenameOut = fi.DirectoryName + @"\out\" + fi.Name.Replace(".DAT", ".csv");
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
            string replacedByteArray = "Filename,Speaker,Game_TextBlock,NoSpeaker_TextBlock\n";
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

                            replacedByteArray = GetFileName(fileInfo, replacedByteArray);
                            replacedByteArray = GetSpeaker(replacedByteArray, line, speakerIndex);
                            replacedByteArray = GetLine(replacedByteArray, line);
                            replacedByteArray = GetLineWithoutSpeaker(replacedByteArray, line, speakerIndex);

                            replacedByteArray += NEWLINE;

                            line = string.Empty;
                            speakerIndex = -1;
                        }
                    }

                }
            }

            return replacedByteArray;
        }

        private static string GetFileName(FileInfo fileInfo, string replacedByteArray)
        {
            replacedByteArray += fileInfo.Name;
            replacedByteArray += COMMA;
            return replacedByteArray;
        }

        private static string GetSpeaker(string replacedByteArray, string line, int speakerIndex)
        {
            replacedByteArray += speakerIndex > -1 ? line.Substring(0, speakerIndex + 1) : string.Empty;
            replacedByteArray += COMMA;
            return replacedByteArray;
        }


        private static string GetLine(string replacedByteArray, string line)
        {
            replacedByteArray += line;
            replacedByteArray += COMMA;
            return replacedByteArray;
        }

        private static string GetLineWithoutSpeaker(string replacedByteArray, string line, int speakerIndex)
        {
            replacedByteArray += speakerIndex > -1 ? line.Substring(speakerIndex, line.Length - speakerIndex) : line;
            replacedByteArray += COMMA;
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
