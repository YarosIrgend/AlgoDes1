using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace AlgoDes1
{
    internal static class ModifiedBalancedSort1
    {
        private const int BufferSizeInBytes = 100 * 1024 * 1024; // 100 MB

        public static void BalancedMultiwayMergeSort(string enterFileName)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine("Початок сортування....");
            SplitFile(enterFileName);
            MergeFiles();
            stopwatch.Stop();
            Console.WriteLine($"Час виконання: {stopwatch.ElapsedMilliseconds / 1000} секунд");
            File.Delete("part1.txt");
            File.Delete("part2.txt");
            File.Delete("part3.txt");
        }

        private static void SplitFile(string enterFileName)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string partFile1 = "part1.txt";
            string partFile2 = "part2.txt";
            string partFile3 = "part3.txt";

            int numbersCount = NumbersCount(enterFileName);
            int numbersInPart = numbersCount / 3;

            using (StreamReader reader = new StreamReader(enterFileName))
            {
                int position = 0;
                int countOfBytesToRead = CountOfBytesToRead(enterFileName, numbersInPart, 0);
                using (StreamWriter part1 = new StreamWriter(partFile1))
                {
                    char[] array = new char[countOfBytesToRead];
                    reader.Read(array, position, countOfBytesToRead);
                    position += countOfBytesToRead;
                    part1.WriteLine(array);
                }

                using (StreamWriter part2 = new StreamWriter(partFile2))
                {
                    countOfBytesToRead = CountOfBytesToRead(enterFileName, numbersInPart, position);
                    char[] array = new char[countOfBytesToRead];
                    reader.Read(array, 0, countOfBytesToRead);
                    part2.WriteLine(array);
                }

                using (StreamWriter part3 = new StreamWriter(partFile3))
                {
                    countOfBytesToRead = (int)reader.BaseStream.Length - (countOfBytesToRead * 2);
                    char[] array = new char[countOfBytesToRead];
                    reader.Read(array, 0, countOfBytesToRead);
                    part3.WriteLine(array);
                }
            }

            SortFile(partFile1);
            SortFile(partFile2);
            SortFile(partFile3);
            stopwatch.Stop();
            Console.WriteLine($"Розбиття, час виконання: {stopwatch.ElapsedMilliseconds / 1000} секунд");
        }

        private static void MergeFiles()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string partFile1 = "part1.txt";
            string partFile2 = "part2.txt";
            string partFile3 = "part3.txt";

            using (StreamReader file1 = new StreamReader(partFile1))
            using (StreamReader file2 = new StreamReader(partFile2))
            using (StreamReader file3 = new StreamReader(partFile3))
            using (StreamWriter outputFile = new StreamWriter("result.txt"))
            {
                string line1 = file1.ReadLine();
                string line2 = file2.ReadLine();
                string line3 = file3.ReadLine();
                
                int? number1 = TryParse(line1);
                int? number2 = TryParse(line2);
                int? number3 = TryParse(line3);
                
                while (true)
                {
                    // Проверка на выход из цикла, если все числа равны null
                    if (number1 == null && number2 == null && number3 == null)
                        break;

                    if (number1 != null && (number2 == null || number1 <= number2) &&
                        (number3 == null || number1 <= number3))
                    {
                        outputFile.Write(Convert.ToString(number1) + " ");
                        line1 = file1.ReadLine();
                        number1 = TryParse(line1);
                    }
                    else if (number2 != null && (number1 == null || number2 <= number1) &&
                             (number3 == null || number2 <= number3))
                    {
                        outputFile.Write(Convert.ToString(number2) + " ");
                        line2 = file2.ReadLine();
                        number2 = TryParse(line2);
                    }
                    else if (number3 != null)
                    {
                        outputFile.Write(Convert.ToString(number3) + " ");
                        line3 = file3.ReadLine();
                        number3 = TryParse(line3);
                    }
                }
            }
            stopwatch.Stop();
            Console.WriteLine($"Злиття, час виконання: {stopwatch.ElapsedMilliseconds / 1000} секунд");
        }

        private static int? TryParse(string line)
        {
            if (int.TryParse(line, out int result))
            {
                return result;
            }
            return null;
        }

        private static void SortFile(string filePath)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int bufferSize = BufferSizeInBytes / sizeof(int); // кількість чисел в 100 МБ
            string tempFilePath = filePath + ".sorted";

            using (StreamReader reader = new StreamReader(filePath))
            using (StreamWriter writer = new StreamWriter(tempFilePath, true))
            {
                List<int> chunk = new List<int>(bufferSize);

                string numberStr;
                while ((numberStr = ReadNextNumber(reader)) != null)
                {
                    chunk.Add(int.Parse(numberStr));

                    if (chunk.Count == bufferSize)
                    {
                        chunk.Sort();
                        SaveChunkToFile(writer, chunk);
                        chunk.Clear();
                    }
                }

                if (chunk.Count > 0)
                {
                    chunk.Sort();
                    SaveChunkToFile(writer, chunk);
                }
            }

            File.Delete(filePath);
            File.Move(tempFilePath, filePath);
            stopwatch.Stop();
            Console.WriteLine($"Сортування, час виконання: {stopwatch.ElapsedMilliseconds / 1000} секунд");
        }

        private static void SaveChunkToFile(StreamWriter writer, List<int> chunk)
        {
            foreach (var number in chunk)
            {
                writer.WriteLine(number);
            }
        }

        private static int NumbersCount(string fileName)
        {
            int numberCount = 0;
            bool inNumber = false;

            using (FileStream file = new FileStream(fileName, FileMode.Open))
            using (StreamReader reader = new StreamReader(file))
            {
                while (!reader.EndOfStream)
                {
                    char currentChar = (char)reader.Read();

                    if (char.IsWhiteSpace(currentChar))
                    {
                        if (inNumber)
                        {
                            numberCount++;
                            inNumber = false;
                        }
                    }
                    else
                    {
                        inNumber = true;
                    }
                }

                if (inNumber)
                    numberCount++;
            }

            return numberCount;
        }

        private static string ReadNextNumber(StreamReader reader)
        {
            StringBuilder number = new StringBuilder();

            while (!reader.EndOfStream)
            {
                char currentChar = (char)reader.Read();

                if (char.IsWhiteSpace(currentChar))
                {
                    if (number.Length > 0)
                    {
                        return number.ToString();
                    }
                }
                else
                {
                    number.Append(currentChar);
                }
            }

            return number.Length > 0 ? number.ToString() : null;
        }

        private static int CountOfBytesToRead(string fileName, int numbersCountToRead, int start)
        {
            int countOfReadBytes = 0;
            using (StreamReader file = new StreamReader(fileName))
            {
                file.BaseStream.Seek(start, SeekOrigin.Begin);
                file.DiscardBufferedData();

                int countOfSpaces = 0;

                while (countOfSpaces != numbersCountToRead)
                {
                    int symbol = file.Read();
                    if (symbol == -1)
                        break;
                    if (symbol == ' ')
                        countOfSpaces++;
                    countOfReadBytes++;
                }
            }

            return countOfReadBytes;
        }
    }
}