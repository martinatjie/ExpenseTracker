using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis;
using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;
using UglyToad.PdfPig.DocumentLayoutAnalysis.ReadingOrderDetector;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;
using UglyToad.PdfPig.Fonts.Standard14Fonts;
using UglyToad.PdfPig.Writer;

namespace ExpenseProcessing.Services;

//TODO : MOVE THIS INTERFACE INTO ITS OWN FILE AND CONSIDER RENAMING THIS TRANSFORMATION SERVICE TO
//ABSA TRANSFORMATION SERVICE, AS IT DEALS WITH ABSA SPECIFIC PDF FILE LAYOUTS.
//ALTERNATIVELY, EXTRACT ABSA-SPECIFIC SYNTAX LOGIC INTO ANOTHER SERVICE.
public interface ITransformationService
{
    void TransformData(string path);
    //void TransformDataAdvanced(string sourcePdfPath, string outputPath);
    //void TransformDataUsingBlocksXyCut(string sourcePdfPath);
}

public class TransformationService(ILogger<ITransformationService> logger) : ITransformationService
{
    public void TransformData(string path)
    {
        //for ecrypted:
        //using (PdfDocument document = PdfDocument.Open(@"C:\my-file.pdf",  new ParsingOptions { Password = "password here" }))
        using (PdfDocument document = PdfDocument.Open(path))
        {
            foreach (Page page in document.GetPages())
            {
                Console.WriteLine("------------------------------------Next Page------------------------------------");
                string text = ContentOrderTextExtractor.GetText(page);
                IEnumerable<Word> words = page.GetWords(NearestNeighbourWordExtractor.Instance);

                var previousWord = "";
                foreach (Word word in words)
                {
                    // if the word text is a date and does not end in ), add a new line
                    if (IsDate(word.Text) && !word.Text.EndsWith(")"))
                    {
                        Console.WriteLine();
                    }

                    if (word.Text != " " && previousWord != " ")
                    {
                        Console.Write("   " + word.Text);
                    }
                    else if (word.Text == " ") {
                        Console.Write("+");
                    }
                    else
                    {
                        Console.Write(word.Text);
                    }

                    if (word.Text == "Balance" && previousWord == "Amount")
                    {
                        Console.WriteLine();
                    }

                    previousWord = word.Text;
                }
                Console.WriteLine();
            }

            logger.LogInformation("done");
        }
    }

    //public void TransformDataAdvanced(string sourcePdfPath, string outputPath)
    //{
    //    var pageNumber = 1;
    //    using (var document = PdfDocument.Open(sourcePdfPath))
    //    {
    //        var builder = new PdfDocumentBuilder { };
    //        PdfDocumentBuilder.AddedFont font = builder.AddStandard14Font(Standard14Font.Helvetica);
    //        var pageBuilder = builder.AddPage(document, pageNumber);
    //        pageBuilder.SetStrokeColor(0, 255, 0);
    //        var page = document.GetPage(pageNumber);

    //        var letters = page.Letters; // no preprocessing

    //        // 1. Extract words
    //        var wordExtractor = NearestNeighbourWordExtractor.Instance;

    //        var words = wordExtractor.GetWords(letters);

    //        // 2. Segment page
    //        var pageSegmenter = DocstrumBoundingBoxes.Instance;

    //        var textBlocks = pageSegmenter.GetBlocks(words);

    //        // 3. Postprocessing
    //        var readingOrder = UnsupervisedReadingOrderDetector.Instance;
    //        var orderedTextBlocks = readingOrder.Get(textBlocks);

    //        // 4. Add debug info - Bounding boxes and reading order
    //        foreach (var block in orderedTextBlocks)
    //        {
    //            var bbox = block.BoundingBox;
    //            pageBuilder.DrawRectangle(bbox.BottomLeft, bbox.Width, bbox.Height);
    //            pageBuilder.AddText(block.ReadingOrder.ToString(), 8, bbox.TopLeft, font);
    //        }

    //        // 5. Write result to a file
    //        byte[] fileBytes = builder.Build();
    //        File.WriteAllBytes(outputPath, fileBytes); // save to file
    //    }
    //}

    //public void TransformDataUsingBlocksXyCut(string sourcePdfPath) 
    //{
        
    //    using (var document = PdfDocument.Open(sourcePdfPath))
    //    {
    //        for (var i = 0; i < document.NumberOfPages; i++)
    //        {
    //            var page = document.GetPage(i + 1);

    //            Console.WriteLine($"----------------Page {i + 1} of {document.NumberOfPages}----------------------");

    //            var words = page.GetWords();

    //            /*var recursiveXYCut = new RecursiveXYCut(new RecursiveXYCut.RecursiveXYCutOptions()
    //            {
    //                MinimumWidth = page.Width / 7,
    //                //DominantFontWidthFunc = _ => page.Letters.Average(l => l.GlyphRectangle.Width),
    //                //DominantFontHeightFunc = _ => page.Letters.Average(l => l.GlyphRectangle.Height)
    //                //DominantFontWidthFunc = letters => letters.Select(l => l.GlyphRectangle.Width).Average(),
    //                //DominantFontHeightFunc = letters => letters.Select(l => l.GlyphRectangle.Height).Average()
    //            });*/

    //            // Use default parameters
    //            // - mode of letters' height and width used as gap size
    //            // - no minimum block width 
    //            //var blocks = RecursiveXYCut.Instance.GetBlocks(words);

    //            var blocks = RecursiveXYCut.Instance.GetBlocks(words);

    //            foreach (var block in blocks)
    //            {
    //                Console.WriteLine("--------------------- Block -----------------------");
    //                // Do something
    //                // E.g. Output the blocks
    //                foreach (TextLine line in block.TextLines)
    //                {
    //                    Console.WriteLine(line.Text);
    //                }

    //            }
    //        }
    //    }
    //}

    // Helper method to robustly check if a string is a South African date (dd/MM/yyyy) and not a decimal number
    private static bool IsDate(string text)
    {
        var trimmed = text.Trim();

        // Exclude decimal numbers (e.g., 433.08)
        if (decimal.TryParse(trimmed, NumberStyles.Number, CultureInfo.InvariantCulture, out _))
            return false;

        // South African date format: dd/MM/yyyy
        var saCulture = new CultureInfo("en-ZA");
        DateTime dt;

        // Try parsing with explicit format and South African culture
        if (DateTime.TryParseExact(trimmed, "dd/MM/yyyy", saCulture, DateTimeStyles.None, out dt))
            return true;

        // Fallback: Try parsing with other common formats and cultures
        if (DateTime.TryParse(trimmed, saCulture, DateTimeStyles.None, out dt))
            return true;

        // Fallback: Try parsing with InvariantCulture and other formats
        if (DateTime.TryParseExact(trimmed, "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            return true;

        // Optional: Character-by-character check for dd/MM/yyyy pattern
        if (trimmed.Length == 10 &&
            char.IsDigit(trimmed[0]) && char.IsDigit(trimmed[1]) &&
            trimmed[2] == '/' &&
            char.IsDigit(trimmed[3]) && char.IsDigit(trimmed[4]) &&
            trimmed[5] == '/' &&
            char.IsDigit(trimmed[6]) && char.IsDigit(trimmed[7]) && char.IsDigit(trimmed[8]) && char.IsDigit(trimmed[9]))
        {
            return true;
        }

        return false;
    }
}
