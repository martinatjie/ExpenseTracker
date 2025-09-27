using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;
using UglyToad.PdfPig.DocumentLayoutAnalysis.ReadingOrderDetector;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;
using UglyToad.PdfPig.Fonts.Standard14Fonts;
using UglyToad.PdfPig.Writer;

namespace ExpenseProcessing.Services
{
    public interface ITransformationService
    {
        void TransformData(string path);
        void TransformDataAdvanced(string sourcePdfPath, string outputPath);
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
                    string text = ContentOrderTextExtractor.GetText(page);
                    IEnumerable<Word> words = page.GetWords(NearestNeighbourWordExtractor.Instance);
                }

                logger.LogInformation("done");
            }
        }

        public void TransformDataAdvanced(string sourcePdfPath, string outputPath)
        {
            var pageNumber = 1;
            using (var document = PdfDocument.Open(sourcePdfPath))
            {
                var builder = new PdfDocumentBuilder { };
                PdfDocumentBuilder.AddedFont font = builder.AddStandard14Font(Standard14Font.Helvetica);
                var pageBuilder = builder.AddPage(document, pageNumber);
                pageBuilder.SetStrokeColor(0, 255, 0);
                var page = document.GetPage(pageNumber);

                var letters = page.Letters; // no preprocessing

                // 1. Extract words
                var wordExtractor = NearestNeighbourWordExtractor.Instance;

                var words = wordExtractor.GetWords(letters);

                // 2. Segment page
                var pageSegmenter = DocstrumBoundingBoxes.Instance;

                var textBlocks = pageSegmenter.GetBlocks(words);

                // 3. Postprocessing
                var readingOrder = UnsupervisedReadingOrderDetector.Instance;
                var orderedTextBlocks = readingOrder.Get(textBlocks);

                // 4. Add debug info - Bounding boxes and reading order
                foreach (var block in orderedTextBlocks)
                {
                    var bbox = block.BoundingBox;
                    pageBuilder.DrawRectangle(bbox.BottomLeft, bbox.Width, bbox.Height);
                    pageBuilder.AddText(block.ReadingOrder.ToString(), 8, bbox.TopLeft, font);
                }

                // 5. Write result to a file
                byte[] fileBytes = builder.Build();
                File.WriteAllBytes(outputPath, fileBytes); // save to file
            }
        }
    }

}
