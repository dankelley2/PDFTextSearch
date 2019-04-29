using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Text;
using iTextSharp.text.pdf.parser;

namespace LoopPDFs
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> filePaths = new List<string>();

            foreach (string val in args)
            {
                Console.WriteLine(val);
                filePaths.Add(val);
            }

            Console.Write("please enter the string to search: ");
            string strToFind = Console.ReadLine();

            if (filePaths.Count == 0) //If none are given, ask for a new one. We will handle batch references differently. this is mainly for debugging
            {
                do
                {
                    filePaths.Clear();
                    Console.WriteLine("Please specify a PDF file path:");
                    filePaths.Add(Console.ReadLine());
                }
                while (UTIL.IsInvalidPdfPath(filePaths[0]));

                findStringInPDF(filePaths[0], strToFind);

            }
            else
            {
                try
                {
                    foreach (string val in filePaths)
                    {
                        if (UTIL.IsInvalidPdfPath(val))
                        {
                            throw new System.ArgumentException("One or more file paths supplied are invalid", val);
                        }
                    }
                    foreach (string val in filePaths)
                    {
                        findStringInPDF(val, strToFind);
                    }
                }
                catch (ArgumentException ae)
                {
                    Console.WriteLine("Error: {0}", ae.Message, ae.InnerException);
                }
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();

        }

        static bool findStringInPDF(string filePath, string strToFind)
        {
            bool success = false;
            try
            {
                if (readPdfFile(filePath).Contains(strToFind))
                {
                    Console.Write("String found in: ");
                    Console.WriteLine(filePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading PDF: {0}", ex.Message);
            }
            return success;
        }

        //static void removePagesFromPdf(PdfReader workingFile, String destinationFile, List<int> pagesToKeep)
        //{
        //    //Create our destination file
        //    using (FileStream fs = new FileStream(destinationFile, FileMode.Truncate, FileAccess.Write, FileShare.None))
        //    {
        //        using (Document doc = new Document(PageSize.LETTER))
        //        {
        //            using (PdfWriter w = PdfWriter.GetInstance(doc, fs))
        //            {
        //                //Open the desitination for writing
        //                doc.Open();
        //                //Loop through each page that we want to keep
        //                foreach (int page in pagesToKeep)
        //                {
        //                    //Add a new blank page to destination document
        //                    doc.NewPage();
        //                    //Extract the given page from our reader and add it directly to the destination PDF
        //                    w.DirectContent.AddTemplate(w.GetImportedPage(workingFile, page), 0, 0);
        //                }
        //                //Close our document
        //                doc.Close();
        //            }
        //        }
        //    }
        //}

        public static string readPdfFile(string fileName)
        {
            StringBuilder text = new StringBuilder();

            if (File.Exists(fileName))
            {
                PdfReader pdfReader = new PdfReader(fileName);

                for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                {
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    string currentText = PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);

                    text.Append(currentText);
                }
                pdfReader.Close();
            }
            return text.ToString();
        }
        //public static string readPdfFile(PdfReader workingFile, int pageNumber)
        //{

        //    if (workingFile.NumberOfPages < pageNumber)
        //    {
        //        throw new System.ArgumentException("PDF contains fewer pages than the supplied pageNumber parameter.");
        //    }
        //    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
        //    string text = PdfTextExtractor.GetTextFromPage(workingFile, pageNumber, strategy);

        //    return text;
        //}

    }

    public static class UTIL
    {
        public static bool IsInvalidPdfPath(string FilePath)
        {
            if (string.IsNullOrWhiteSpace(FilePath))
            {
                return true;
            }

            if (FilePath.Substring(Math.Max(0, FilePath.Length - 4)).ToUpper() != ".PDF")
            {
                return true;
            }

            foreach (char C in System.IO.Path.GetInvalidPathChars())
            {
                if (FilePath.Contains(C) == true)
                {
                    return true;
                }
            }
            return false;
        }
    }
}


