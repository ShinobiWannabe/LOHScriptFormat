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
                //DirectoryInfo di = new DirectoryInfo(@".\");
                //string output = "Filename,Speaker,Game_TextBlock,NoSpeaker_TextBlock\n";
                //string output = @"Filename|Speaker|Game_TextBlock|NoSpeaker_TextBlock\n";
                string columns = @"""Filename""|""Position""|""Speaker""|""Game_TextBlock""|""NoSpeaker_TextBlock""\n";
                DeleteOutputIfExists(di);

                //List<Byte> output = new List<Byte>();
                //output.Add(columns.);
                foreach (FileInfo fi in di.EnumerateFiles("*.Dat"))
                {

                    //DirectoryInfo dirOut = new DirectoryInfo(@".\bin\Debug\netcoreapp1.1\out\");
                    // DirectoryInfo dirOut = new DirectoryInfo(@".\out\");
                    //string replacedByteArray;
                    SaveModifiedFile(fi);
                    //WriteFile(fi.DirectoryName + @"\out\output.csv", output);
                    // output = string.Empty;
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
        private void SaveModifiedFile(FileInfo fileInfo)
        {
            List<Byte> output = new List<byte>();
            //List<Byte[]> fsLines = new List<Byte[]>();


            using (FileStream fs = new FileStream(fileInfo.FullName, FileMode.Open))
            {

                int speakerIndex = -1;
                List<Byte> line = new List<Byte>();
                //List<Byte> temp = new List<Byte>();
                Boolean skippedFirst = false; //this alone removes tons of garbage from the .dat file
                int fsPos = 0;
                //int outputPos = 0;

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
                            // output = GetPos(output, fsPos - line.Length, fsPos, outputPos);
                            output = GetSpeaker(output, line, speakerIndex);
                            //line = GetLine(line, fsPos);
                            //  output = GetLineWithoutSpeaker(output, line, speakerIndex);

                            //line.AddRange(temp.ToArray());
  }
                        catch (Exception ex)
                        {

                            throw;
                        }
                            output.Add((byte)NEWLINE);

                            //  output.AddRange(line.ToArray());
                            //output.AddRange(output.ToArray());
                            line.Clear();
                            speakerIndex = -1;
                      
                    }

                }
            }
            try
            {


                DirectoryInfo dirOut = new DirectoryInfo(@".\bin\Debug\netcoreapp1.1\out\");
                using (FileStream fs = new FileStream(dirOut.FullName + @"\output.csv", FileMode.Append))
                {

                    fs.Write(output.ToArray(), 0, output.Count);
                }
            }
            catch (Exception)
            {

                throw;
            }
            //return output;
        }

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

        //private static string GetFileName(FileInfo fileInfo, string output)
        //{
        //    output += QUALIFIER;
        //    output += fileInfo.Name;
        //    output += QUALIFIER;
        //    output += DELIMITER;
        //    return output;
        //}

        private static List<Byte> GetFileName(List<Byte> output, FileInfo fileInfo)
        {
            try
            {


                output.Add((Byte)QUALIFIER);

                output.AddRange(System.Text.CodePagesEncodingProvider.Instance.GetEncoding(932).GetBytes(fileInfo.Name));

                //output.Add(fileInfo.Name);
                output.Add((Byte)QUALIFIER);
                output.Add((Byte)DELIMITER);
            }
            catch (Exception ex)
            {

                throw;
            }
            return output;
        }


        //private static List<Byte> GetFileName(FileInfo fileInfo, List<Byte> output, ref int pos)
        //{
        //    output[pos] = (byte)QUALIFIER;

        //    //output += fileInfo.Name;
        //    //output += QUALIFIER;
        //    //output += DELIMITER;
        //    return output;
        //}


        //private static string GetPos(string output, int pos)
        //{
        //    output += QUALIFIER;
        //    output += pos.ToString();
        //    output += QUALIFIER;
        //    output += DELIMITER;
        //    return output;
        //}


        private byte[] GetPos(byte[] output, int pos)
        {
            //output += QUALIFIER;
            //output += pos.ToString();
            //output += QUALIFIER;
            //output += DELIMITER;
            return output;
        }


        //private static string GetSpeaker(string output, string line, int speakerIndex)
        //{
        //    output += QUALIFIER;
        //    output += speakerIndex > -1 ? line.Substring(0, speakerIndex + 1) : string.Empty;
        //    output += QUALIFIER;
        //    output += DELIMITER;
        //    return output;
        //}


        private static List<Byte> GetSpeaker(List<byte> output, List<Byte> line, int speakerIndex)
        {

            try
            {
                //line.Insert(0, (Byte)QUALIFIER);
                output.Add((Byte)QUALIFIER);
                output.AddRange(speakerIndex > -1 ? line.GetRange(0, speakerIndex + 1) : null);
                output.Add((Byte)QUALIFIER);
                output.Add((Byte)DELIMITER);
            }
            catch (Exception)
            {
                throw;
            }
            return output;
        }


        //private static List<Byte> GetSpeaker(List<Byte> line, int speakerIndex, List<Byte> temp)
        //{

        //    try
        //    {
        //        //line.Insert(0, (Byte)QUALIFIER);
        //        temp.Add((Byte)QUALIFIER);
        //        temp.AddRange(speakerIndex > -1 ? line.GetRange(0, speakerIndex + 1) : null);
        //        temp.Add((Byte)QUALIFIER);
        //        temp.Add((Byte)DELIMITER);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    return temp;
        //}

        //private static string GetLine(string output, string line)
        //{
        //    output += QUALIFIER;
        //    output += line;
        //    output += QUALIFIER;
        //    output += DELIMITER;
        //    return output;
        //}


        private static List<Byte> GetLine(List<Byte> line, int pos)
        {
            line.Insert(0, (Byte)QUALIFIER);
            line.Add((Byte)QUALIFIER);
            line.Add((Byte)DELIMITER);

            return line;
        }

        private static string GetLineWithoutSpeaker(string output, string line, int speakerIndex)
        {
            output += QUALIFIER;
            output += speakerIndex > -1 ? line.Substring(speakerIndex, line.Length - speakerIndex) : line;
            output += QUALIFIER;
            output += DELIMITER;
            return output;
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
