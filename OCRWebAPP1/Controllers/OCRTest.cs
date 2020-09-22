using System;

// Most commonly used namespaces for PDFTron SDK.
using pdftron;
using pdftron.Common;
using pdftron.PDF;
using pdftron.SDF;

namespace OCRWebAPP1.Controllers
{
	internal class OCRTest
	{
		private pdftron.PDFNetLoader pdfNetLoader = pdftron.PDFNetLoader.Instance();
		public string Test()
		{
			string s = "";

			// Initialize PDFNet before using any PDFTron related
			// classes and methods (some exceptions can be found in API)
			PDFNet.Initialize();

			// Using PDFNet related classes and methods, must  
			// catch or throw PDFNetException
			try
			{
				string input_path = "./TestFiles/";
				s = processOCR(input_path+ "discharge-summary.pdf");

				return s;
			}
			catch (PDFNetException e)
			{
				return e.Message;
			}
		}

		private string processOCR(string fileName)
		{
			//PDFNet.Initialize();
			//PDFNet.AddResourceSearchPath("./Lib/");
			PDFNet.SetResourcesPath("./Lib/");
			var s = PDFNet.GetResourcesPath();

			if (!OCRModule.IsModuleAvailable())
			{
				return "MODULE NOT FOUND";
			}
			try
			{
				using (PDFDoc doc = new PDFDoc(fileName))
				{
					OCROptions opts = new OCROptions();
					opts.AddLang("eng");
					string jsonOCRData = OCRModule.GetOCRJsonFromPDF(doc, opts);
					//string appendedString = getOCRDataFromJson(jsonOCRData);
					return jsonOCRData;
				}
				//return appenedString;
			}
			catch (PDFNetException e)
			{
				return e.Message;
			}
		}
	}
}