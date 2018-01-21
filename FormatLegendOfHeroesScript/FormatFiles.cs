using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace FormatLegendOfHeroesScript
{
    class FormatFiles
    {

        //const char DELIMITER = (char)0x2C; //COMMA, not as good as the pipe
        const char DELIMITER = (char)0x7C; //Vertical Line aka Pipe '|'
        const char QUALIFIER = (char)0x22; //Quote '"'
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

                //need to find how to output also to windows and I guess linux and mac.  Seems to be a way of doing this for whomever may want to use this an a utility.
                // Console.Writeline("Paste folder directory)
                //String s = Console.ReadLine();
                DirectoryInfo di = new DirectoryInfo(@".\bin\Debug\netcoreapp1.1\");

                List<Byte> output = new List<byte>();
                //string columns = @"""Filename""|""Position""|""Speaker""|""NoSpeaker_TextBlock""";
                string columns = @"""Filename""|""StartPosition""|""FullTextBlockByteLength""|""FullTextBlock""|""Speaker""|""NoSpeaker_TextBlock""";

                output.AddRange(System.Text.CodePagesEncodingProvider.Instance.GetEncoding(932).GetBytes(columns));
                output.Add((byte)NEWLINE);

                DeleteOutputIfExists(di);

                DirectoryInfo dirOut = new DirectoryInfo(@".\bin\Debug\netcoreapp1.1\out\");
                foreach (FileInfo fi in di.EnumerateFiles("*.Dat"))
                {
                    output = GetModifiedFile(output, fi);
                    WriteFile(output, dirOut);
                    output.Clear();
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
                Console.WriteLine("output.csv exists and is probably in use. Cannot delete to overwrite.");
            }

        }

        //While this is more complicated that dealing wth StreamReader, it allows me to get the positions. 
        private List<Byte> GetModifiedFile(List<Byte> output, FileInfo fileInfo)
        {

            using (FileStream fs = new FileStream(fileInfo.FullName, FileMode.Open))
            {
                int speakerIndex = -1;
                List<Byte> line = new List<Byte>();
                Boolean skippedFirst = false; //this alone removes tons of garbage from the .dat file
                int fsPos = 0;
                while (fs.Position != fs.Length)
                {
                    Byte fsByte = (byte)fs.ReadByte();
                    line.Add(fsByte);
                    fsPos++;

                    speakerIndex = -1;

                    if (fsByte == (Byte)GAME_TEXTBLOCKEND)  //use this if something is blantantly wrong with the below check.
                                                            //if ((char)((chr) & 0xF0) == (char)(0x00) && fs.) == GAME_TEXTBLOCKEND) //filestream does not support Peek() //0x1E's PREVIOUS byte seems to always follow the format of 0x0F, where the high byte is 0.  This removes lots of bogus shit.
                    {
                        if (skippedFirst == false) //the beginning of every file has garbage.
                        {
                            line.Clear();
                            speakerIndex = -1;
                            skippedFirst = true;
                            continue;
                        }

                        speakerIndex = line.IndexOf((Byte)(0x04));

                        try
                        {

                            output = GetFileName(output, fileInfo);
                            output = GetPos(output, line, fsPos);
                            output = GetFullLineLength(output, line);
                            output = GetFullLine(output, line, fsPos);
                            output = GetSpeaker(output, line, speakerIndex);
                            output = GetLineWithoutSpeaker(output, line, speakerIndex);

                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                        output.Add((byte)NEWLINE);

                        line.Clear();
                        speakerIndex = -1;

                    }

                }
            }

            return output;
        }

        private static List<Byte> GetFileName(List<Byte> output, FileInfo fileInfo)
        {
            try
            {
                output.Add((Byte)QUALIFIER);
                output.AddRange(System.Text.CodePagesEncodingProvider.Instance.GetEncoding(932).GetBytes(fileInfo.Name));
                output.Add((Byte)QUALIFIER);
                output.Add((Byte)DELIMITER);
            }
            catch (Exception ex)
            {
                throw;
            }
            return output;
        }

        private static List<Byte> GetSpeaker(List<byte> output, List<Byte> line, int speakerIndex)
        {
            try
            {
                output.Add((Byte)QUALIFIER);
                output.AddRange(speakerIndex > -1 ? line.GetRange(0, speakerIndex + 1) : new List<Byte>());
                output.Add((Byte)QUALIFIER);
                output.Add((Byte)DELIMITER);
            }
            catch (Exception ex)
            {
                throw;
            }
            return output;
        }

        private List<Byte> GetPos(List<Byte> output, List<Byte> line, int fsPos)
        {
            output.Add((Byte)QUALIFIER);
            int x = fsPos - line.Count;
            Byte[] num = System.Text.CodePagesEncodingProvider.Instance.GetEncoding(932).GetBytes(x.ToString());
            output.AddRange(num);
            output.Add((Byte)QUALIFIER);
            output.Add((Byte)DELIMITER);

            return output;
        }

        private List<Byte> GetFullLineLength(List<Byte> output, List<Byte> line)
        {
            output.Add((Byte)QUALIFIER);
            //subtracting 2 here. Only want the length of the text between the starting marker 0x1E and the ending 0x1E.
            Byte[] num = System.Text.CodePagesEncodingProvider.Instance.GetEncoding(932).GetBytes((line.Count -2).ToString());
            output.AddRange(num);
            output.Add((Byte)QUALIFIER);
            output.Add((Byte)DELIMITER);

            return output;
        }

        private static List<Byte> GetFullLine(List<Byte> output, List<Byte> line, int pos)
        {
            output.Add((Byte)QUALIFIER);
            output.AddRange(line);
            output.Add((Byte)QUALIFIER);
            output.Add((Byte)DELIMITER);
            return output;
        }

        private static List<Byte> GetLineWithoutSpeaker(List<Byte> output, List<Byte> line, int speakerIndex)
        {
            output.Add((Byte)QUALIFIER);
            output.AddRange(speakerIndex > -1 ? line.GetRange(speakerIndex, line.Count - speakerIndex) : new List<Byte>());
            output.Add((Byte)QUALIFIER);
            output.Add((Byte)DELIMITER);
            return output;
        }


        private static void WriteFile(List<Byte> output, DirectoryInfo dirOut)
        {
            try
            {

                using (FileStream fs = new FileStream(dirOut.FullName + @"\output.csv", FileMode.Append))
                {
                    fs.Write(output.ToArray(), 0, output.Count);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }



        //private static void WriteFile(string output, string filenameOut )
        //{
        //    using (FileStream fs = new FileStream(filenameOut, FileMode.Append))
        //    using (StreamWriter sw = new StreamWriter(fs, System.Text.CodePagesEncodingProvider.Instance.GetEncoding(932)))
        //    {
        //        sw.WriteLine(output);
        //    }
        //}



        ////Uses streamreader.  Cannot be used to get position due to characters not being consistent with byte length.
        //private static string GetModifiedFile(FileInfo fileInfo)
        //{
        //    string output = string.Empty;
        //    using (FileStream fs = new FileStream(fileInfo.FullName, FileMode.Open))
        //    {
        //        using (StreamReader sr = new StreamReader(fs, System.Text.CodePagesEncodingProvider.Instance.GetEncoding(932)))
        //        {
        //            int speakerIndex = -1;
        //            string line = string.Empty;
        //            Boolean skippedFirst = false; //this alone removes tons of garbage from the .dat file
        //            int pos = 0;
        //            while (!sr.EndOfStream)
        //            {
        //                char chr = (char)sr.Read();
        //                pos++; //this is not going to work.  characters can be multiple characters.
        //                line += chr;
        //                speakerIndex = -1;

        //               // if (chr == GAME_TEXTBLOCKEND)  //use this if something is blantantly wrong with the below check.
        //                 if ( (char)((chr) & 0xF0) == (char)(0x00) && sr.Peek() ==  GAME_TEXTBLOCKEND) //0x1E's PREVIOUS byte seems to always follow the format of 0x0F, where the high byte is 0.  This removes lots of bogus shit.
        //                {
        //                    if(skippedFirst==false) //the beginning of every file has garbage.
        //                    {
        //                        line = string.Empty;
        //                        speakerIndex = -1;
        //                        skippedFirst = true;
        //                        continue;
        //                    }
        //                    //maybe input a comma on 0x0401. which seems to be after the top of the text block indicating who is speaking. 
        //                    //if (line.Contains((string)(0x0A00).ToString()))
        //                    //{
        //                    //    Console.Write("empty");
        //                    //}
        //                    speakerIndex = line.IndexOf((char)(0x04));

        //                   // line.CopyTo(new Byte[])
        //                    output = GetFileName(fileInfo, output);
        //                    output = GetPos(output, pos - System.Text.CodePagesEncodingProvider.Instance.GetEncoding(932).GetBytes(line).Length);
        //                    output = GetSpeaker(output, line, speakerIndex);
        //                    output = GetLine(output, line);
        //                    output = GetLineWithoutSpeaker(output, line, speakerIndex);

        //                    output += NEWLINE;

        //                    line = string.Empty;
        //                    speakerIndex = -1;
        //                }
        //            }

        //        }
        //    }

        //    return output;
        //}




    }

}
