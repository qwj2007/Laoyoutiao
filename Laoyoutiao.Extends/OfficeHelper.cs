namespace Laoyoutiao.Extends
{
    public class OfficeHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="savepath"></param>
        /// <returns></returns>

        //public ResultMsg PPT(string filepath, string savepath)
        //{
        //    try
        //    {
        //        if (!System.IO.File.Exists(filepath))
        //        {
        //            return ResultMsg.Fail("源文件不存在");
        //        }
        //        if (!System.IO.Directory.Exists(savepath))
        //        {
        //            System.IO.Directory.CreateDirectory(savepath);
        //        }

        //        savepath = $"{savepath}/{Guid.NewGuid()}.pdf";
        //        Aspose.Slides.Presentation pptx = new Aspose.Slides.Presentation(filepath);
        //        pptx.Save(savepath, Aspose.Slides.Export.SaveFormat.Pdf);

        //        return ResultMsg.Success(savepath, "pdf");

        //    }
        //    catch (Exception ex)
        //    {
        //        return ResultMsg.Fail(ex.Message);
        //    }
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="filepath"></param>
        ///// <param name="savepath"></param>
        ///// <returns></returns>

        //public ResultMsg Word(string filepath, string savepath)
        //{
        //    try
        //    {
        //        if (!System.IO.File.Exists(filepath))
        //        {
        //            return ResultMsg.Fail("源文件不存在");
        //        }
        //        if (!System.IO.Directory.Exists(savepath))
        //        {
        //            System.IO.Directory.CreateDirectory(savepath);
        //        }

        //        Aspose.Words.Document doc = new Aspose.Words.Document(filepath);
        //        if (doc == null)
        //        {
        //            return ResultMsg.Fail("读取文档失败");

        //        }
        //        savepath = $"{savepath}/{Guid.NewGuid()}.pdf";


        //        doc.Save(savepath, Aspose.Words.SaveFormat.Pdf);

        //        return ResultMsg.Success(savepath, "pdf");

        //    }
        //    catch (Exception ex)
        //    {
        //        return ResultMsg.Fail(ex.Message);
        //    }
        //}


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="filepath"></param>
        ///// <param name="savepath"></param>
        ///// <returns></returns>

        //public ResultMsg Excel(string filepath, string savepath)
        //{
        //    try
        //    {
        //        if (!System.IO.File.Exists(filepath))
        //        {
        //            return ResultMsg.Fail("源文件不存在");
        //        }
        //        if (!System.IO.Directory.Exists(savepath))
        //        {
        //            System.IO.Directory.CreateDirectory(savepath);
        //        }

        //        savepath = $"{savepath}/{Guid.NewGuid()}.html";


        //        Aspose.Cells.Workbook excel = new Aspose.Cells.Workbook(filepath);

        //        excel.Save(savepath, Aspose.Cells.SaveFormat.Html);

        //        return ResultMsg.Success(savepath, "html");

        //    }
        //    catch (Exception ex)
        //    {
        //        return ResultMsg.Fail(ex.Message);
        //    }
        //}


        //public ResultMsg Csv(string filepath, string savepath)
        //{
        //    try
        //    {
        //        if (!System.IO.File.Exists(filepath))
        //        {
        //            return ResultMsg.Fail("源文件不存在");
        //        }
        //        if (!System.IO.Directory.Exists(savepath))
        //        {
        //            System.IO.Directory.CreateDirectory(savepath);
        //        }

        //        savepath = $"{savepath}/{Guid.NewGuid()}.html";


        //        Aspose.Cells.TxtLoadOptions excelcsv = new Aspose.Cells.TxtLoadOptions();
        //        excelcsv.Encoding = Encoding.Default;
        //        Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(filepath, excelcsv);
        //        workbook.FileFormat = Aspose.Cells.FileFormatType.CSV;
        //        workbook.Save(savepath, Aspose.Cells.SaveFormat.Html);

        //        return ResultMsg.Success(savepath, "html");

        //    }
        //    catch (Exception ex)
        //    {
        //        return ResultMsg.Fail(ex.Message);
        //    }
        //}
    }
}
