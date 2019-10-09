using System;
using System.Collections.Generic;
using iText.Forms.Fields;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms {
    public class PdfFormFieldMultilineTextTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/PdfFormFieldMultilineTextTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/PdfFormFieldMultilineTextTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void MultilineFormFieldTest() {
            String filename = destinationFolder + "multilineFormFieldTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            PdfTextFormField name = PdfFormField.CreateMultilineText(pdfDoc, new Rectangle(150, 600, 277, 44), "fieldName"
                , "", null, 0);
            name.SetScroll(false);
            name.SetBorderColor(ColorConstants.GRAY);
            String itextLicence = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, " + "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. "
                 + "Lorem ipsum dolor sit amet, consectetur adipiscing elit, " + "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";
            name.SetValue(itextLicence);
            form.AddField(name);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_multilineFormFieldTest.pdf"
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void MultilineTextFieldWithAlignmentTest() {
            String outPdf = destinationFolder + "multilineTextFieldWithAlignment.pdf";
            String cmpPdf = sourceFolder + "cmp_multilineTextFieldWithAlignment.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outPdf));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            Rectangle rect = new Rectangle(210, 600, 150, 100);
            PdfTextFormField field = PdfFormField.CreateMultilineText(pdfDoc, rect, "fieldName", "some value\nsecond line\nthird"
                );
            field.SetJustification(PdfTextFormField.ALIGN_RIGHT);
            form.AddField(field);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void MultilineFormFieldNewLineTest() {
            String testName = "multilineFormFieldNewLineTest";
            String outPdf = destinationFolder + testName + ".pdf";
            String cmpPdf = sourceFolder + "cmp_" + testName + ".pdf";
            String srcPdf = sourceFolder + testName + ".pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            PdfReader reader = new PdfReader(srcPdf);
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            IDictionary<String, PdfFormField> fields = form.GetFormFields();
            fields.Get("BEMERKUNGEN").SetValue("First line\n\n\nFourth line");
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void MultilineFormFieldNewLineFontType3Test() {
            String testName = "multilineFormFieldNewLineFontType3Test";
            String outPdf = destinationFolder + testName + ".pdf";
            String cmpPdf = sourceFolder + "cmp_" + testName + ".pdf";
            String srcPdf = sourceFolder + testName + ".pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            PdfReader reader = new PdfReader(srcPdf);
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            PdfTextFormField info = (PdfTextFormField)form.GetField("info");
            info.SetValue("A\n\nE");
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void NotFittingByHeightTest() {
            String filename = "notFittingByHeightTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            for (int i = 15; i <= 50; i += 15) {
                PdfFormField[] fields = new PdfFormField[] { PdfFormField.CreateMultilineText(pdfDoc, new Rectangle(100, 800
                     - i * 4, 150, i), "multi " + i, "MULTI"), PdfFormField.CreateText(pdfDoc, new Rectangle(300, 800 - i 
                    * 4, 150, i), "single " + i, "SINGLE") };
                foreach (PdfFormField field in fields) {
                    field.SetFontSize(40);
                    field.SetBorderColor(ColorConstants.BLACK);
                    form.AddField(field);
                }
            }
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(destinationFolder + filename, sourceFolder + "cmp_" + filename
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void BorderWidthIndentMultilineTest() {
            String filename = destinationFolder + "borderWidthIndentMultilineTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            PdfTextFormField field = PdfFormField.CreateMultilineText(pdfDoc, new Rectangle(100, 500, 400, 300), "multi"
                , "Does this text overlap the border? Well it shouldn't!");
            field.SetFontSize(30);
            field.SetBorderColor(ColorConstants.RED);
            field.SetBorderWidth(50);
            form.AddField(field);
            PdfTextFormField field2 = PdfFormField.CreateMultilineText(pdfDoc, new Rectangle(100, 400, 400, 50), "multiAuto"
                , "Does this autosize text overlap the border? Well it shouldn't! Does it fit accurately though?");
            field2.SetFontSize(0);
            field2.SetBorderColor(ColorConstants.RED);
            field2.SetBorderWidth(20);
            form.AddField(field2);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_borderWidthIndentMultilineTest.pdf"
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }
    }
}