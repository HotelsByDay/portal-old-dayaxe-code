using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI;
using AutoMapper;
using CsvHelper;
using DayaxeDal;
using DayaxeDal.Custom;
using DayaxeDal.Repositories;

namespace h.dayaxe.com.Helper
{
    public partial class HelperImportDiscount : Page
    {
        private readonly DiscountRepository _discountRepository = new DiscountRepository();
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Import_OnClick(object sender, EventArgs e)
        {
            if (ImportDiscountFileUpload.HasFile)
            {
                using (var reader = new StreamReader(ImportDiscountFileUpload.FileContent))
                using (var csvReader = new CsvReader(reader))
                {
                    csvReader.Configuration.HasHeaderRecord = false;
                    csvReader.Configuration.RegisterClassMap<InportDiscountObjectMap>();
                    // Use While(csvReader.Read()); if you want to read all the rows in the records)
                    csvReader.Read();
                    var result = csvReader.GetRecords<InportDiscountObject>().ToList();

                    var discounts = Mapper.Map<List<InportDiscountObject>, List<Discounts>>(result);

                    _discountRepository.Add(discounts);

                    _discountRepository.RefreshData();
                }

                ImportDiscountFileUpload.SaveAs(Server.MapPath("~/Helper/") + ImportDiscountFileUpload.FileName);
            }
        }
    }
}