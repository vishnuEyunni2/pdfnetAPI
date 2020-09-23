using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using pdftron;
using pdftron.Common;
using pdftron.PDF;
using pdftron.PDF.Annots;
using pdftron.SDF;

namespace Sample {
    
    public class RemoteSignatureTimeStampPdfHandler : SignatureHandler{
        
        private List<byte> m_data;
        private readonly HttpClient _client;

        public RemoteSignatureTimeStampPdfHandler(HttpClient client)
        {
            _client = client;
            m_data = new List<byte>();
        }

        public override void AppendData(byte[] data)
        {
            m_data.AddRange(data);
        }

        public override bool Reset()
        {
            m_data.Clear();
            return true;
        }

        public override byte[] CreateSignature()
        {
            //var bdata = m_data.ToArray();
            try {
                //Simulate an http request to CA to create TimeStamp
                _client.GetAsync("https://google.com"); 
                return new byte[] { 0, 100, 120, 210, 255};
            }
            catch (Exception e) {
                Console.Error.WriteLine(e);
            }
            return null;
        }

        public override string GetName()
        {
            return "Adobe.PPKLite";
        }
    }
    
    static class Program {

        private const string Message = "Page number {0} saved.";
        
        static void Main(string[] args) {
            try { 
                //for (int i = 0; i < 100; i++) {
                //    Console.WriteLine(string.Format(Message, i));
                //}
                    TestOCR();
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
            
        }
        
        private static PDFNetLoader loader = PDFNetLoader.Instance();

		static void TestOCR()
		{
			PDFNet.Initialize();

			// Can optionally set path to the OCR module
			PDFNet.AddResourceSearchPath("../lib/");
			if (!OCRModule.IsModuleAvailable())
			{
				Console.WriteLine("");
				Console.WriteLine("Unable to run OCRTest: PDFTron SDK OCR module not available.");
				Console.WriteLine("---------------------------------------------------------------");
				Console.WriteLine("The OCR module is an optional add-on, available for download");
				Console.WriteLine("at http://www.pdftron.com/. If you have already downloaded this");
				Console.WriteLine("module, ensure that the SDK is able to find the required files");
				Console.WriteLine("using the PDFNet.AddResourceSearchPath() function.");
				Console.WriteLine("");

				return;
			}

			// Relative path to the folder containing test files.
			string input_path = "../TestFiles/OCR/";
			string output_path = "../TestFiles/Output/";

			//--------------------------------------------------------------------------------
			// Example 1) Process image
			try
			{

				// A) Setup empty destination doc
				using (PDFDoc doc = new PDFDoc(input_path+ "discharge - summary.pdf"))
				{
					// B) Set English as the language of choice
					OCROptions opts = new OCROptions();
					opts.AddLang("eng");

					// C) Run OCR on the .png with options            
					string jsonOCRData = OCRModule.GetOCRJsonFromPDF(doc, opts);
					//OCRModule.ImageToPDF(doc, input_path + "psychomachia_excerpt.png", opts);
					Console.WriteLine(jsonOCRData);
					// D) check the result
					doc.Save(output_path + "discharge - summary.pdf", SDFDoc.SaveOptions.e_remove_unused);

					Console.WriteLine("Example 1: discharge - summary.pdf");
				}

			}
			catch (PDFNetException e)
			{
				Console.WriteLine(e.Message);
			}
		}

        static void SignPDF() {
            PDFNet.Initialize();
            // Create a page
            using (var doc = new PDFDoc()) {
                var page = doc.PageCreate(new Rect(0, 0, 595, 842));
                page.SetRotation(Page.Rotate.e_0);
                page.SetCropBox(new Rect(0, 0, 595, 842));
                doc.PagePushBack(page);
                
                var rect = new Rect(0, 0, 0, 0);
                var fieldId = Guid.NewGuid().ToString();

                var fieldToSign = doc.FieldCreate(fieldId, Field.Type.e_signature);
                var signatureAnnotation = Widget.Create(doc, rect, fieldToSign);

                signatureAnnotation.SetFlag(Annot.Flag.e_print, true);
                signatureAnnotation.SetPage(page);
                var widgetObj = signatureAnnotation.GetSDFObj();
                widgetObj.PutNumber("F", 132.0);
                widgetObj.PutName("Type", "Annot");

                page.AnnotPushBack(signatureAnnotation);

                //Create the signature handler
                var sigHandler = new RemoteSignatureTimeStampPdfHandler(new HttpClient());

                //Add handler to PDFDoc instance
                var sigHandlerId = doc.AddSignatureHandler(sigHandler);

                // Add the SignatureHandler instance to PDFDoc, making sure to keep track of it using the ID returned.
                var sigDict = fieldToSign.UseSignatureHandler(sigHandlerId);

                var signatureObject = signatureAnnotation.GetSDFObj();

                var cultureInfo = new CultureInfo("en-US");
                var gmt1Date = DateTime.Now;

                var value = gmt1Date.ToString("'D:'yyyyMMddHHmmsszzz", cultureInfo);

                // Add signing date
                sigDict.PutString("M", value);

                doc.Save(SDFDoc.SaveOptions.e_incremental);
            }
        }
    }
}