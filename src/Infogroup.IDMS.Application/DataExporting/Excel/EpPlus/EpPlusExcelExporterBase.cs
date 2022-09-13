using System;
using System.Collections;
using System.Collections.Generic;
using Abp.AspNetZeroCore.Net;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.Storage;
using OfficeOpenXml;

namespace Infogroup.IDMS.DataExporting.Excel.EpPlus
{
    public abstract class EpPlusExcelExporterBase : IDMSServiceBase, ITransientDependency
    {
        private readonly ITempFileCacheManager _tempFileCacheManager;

        protected EpPlusExcelExporterBase(ITempFileCacheManager tempFileCacheManager)
        {
            _tempFileCacheManager = tempFileCacheManager;
        }

        protected FileDto CreateExcelPackage(string fileName, Action<ExcelPackage> creator)
        {
            var file = new FileDto(fileName, MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet);

            using (var excelPackage = new ExcelPackage())
            {
                creator(excelPackage);
                Save(excelPackage, file);
            }

            return file;
        }

        protected void AddHeader(ExcelWorksheet sheet, params string[] headerTexts)
        {
            if (headerTexts.IsNullOrEmpty())
            {
                return;
            }

            for (var i = 0; i < headerTexts.Length; i++)
            {
                AddHeader(sheet, i + 1, headerTexts[i]);
            }
        }

        protected void AddHeader(ExcelWorksheet sheet, int columnIndex, string headerText)
        {
            sheet.Cells[1, columnIndex].Value = headerText;
            sheet.Cells[1, columnIndex].Style.Font.Bold = true;
        }

        protected void AddHeader(ExcelWorksheet sheet, int rowindex, params string[] headerTexts)
        {
            for (var i = 0; i < headerTexts.Length; i++)
            {
                sheet.Cells[rowindex, i + 1].Value = headerTexts[i];
                sheet.Cells[rowindex, i + 1].Style.Font.Bold = true;
            }
        }

        protected void AddHeader(ExcelWorksheet sheet, int rowindex, int columnIndex, params string[] headerTexts)
        {
            for (var i = 0; i < headerTexts.Length; i++)
            {
                sheet.Cells[rowindex, columnIndex + i].Value = headerTexts[i];
                sheet.Cells[rowindex, columnIndex + i].Style.Font.Bold = true;
            }
        }

        protected void AddObject(ExcelWorksheet sheet, int startRowIndex, string item)
        {

            sheet.Cells[startRowIndex, 1].Value = item;
        }

        protected void AddObjects<T>(ExcelWorksheet sheet, int startRowIndex, IList<T> items, params Func<T, object>[] propertySelectors)
        {
            GenerateExcel(sheet, startRowIndex, items, false, propertySelectors);
        }
        protected int AddObjectsManager<T>(ExcelWorksheet sheet, int startRowIndex, IList<T> items, params Func<T, object>[] propertySelectors)
        {
            return GenerateExcel(sheet, startRowIndex, items, true, propertySelectors);
        }
        private int GenerateExcel<T>(ExcelWorksheet sheet, int startRowIndex, IList<T> items,bool isFromManger=false, params Func<T, object>[] propertySelectors)
        {
            if (items.IsNullOrEmpty() || propertySelectors.IsNullOrEmpty())
            {
                return 0;
            }
            var tempStartIndex = startRowIndex;
            var maxIndex = 0;
            for (var i = 0; i < items.Count; i++)
            {
                for (var j = 0; j < propertySelectors.Length; j++)
                {
                    if (propertySelectors[j](items[i]) is IList && propertySelectors[j](items[i]).GetType().IsGenericType
                        && propertySelectors[j](items[i])?.GetType().GetGenericTypeDefinition() == typeof(List<>))
                    {
                        startRowIndex = tempStartIndex;
                        var collection = (IList)propertySelectors[j](items[i]);
                        for (var k = 0; k < collection.Count; k++)
                        {    
                            if (isFromManger)                            
                                sheet.Cells[i + startRowIndex++, j + 1].Value = collection[k];                            
                            else
                                sheet.Cells[i + startRowIndex, j + 1].Value = k == 0 ? collection[k] : sheet.Cells[i + startRowIndex, j + 1].Value + "\n" + collection[k];
                            sheet.Cells[i + startRowIndex, j + 1].Style.WrapText = true;
                        }
                        maxIndex = startRowIndex > maxIndex ? startRowIndex - 1 : maxIndex;
                    }
                    else
                        sheet.Cells[i + startRowIndex, j + 1].Value = propertySelectors[j](items[i]);                   
                }                
            }
            if (isFromManger)
                return maxIndex;
            else
                return startRowIndex;
        }
  
        protected void AddObjects<T>(ExcelWorksheet sheet, int startRowIndex, int startColumcIndex, IList<T> items, params Func<T, object>[] propertySelectors)
        {
            if (items.IsNullOrEmpty() || propertySelectors.IsNullOrEmpty())
            {
                return;
            }

            for (var i = 0; i < items.Count; i++)
            {
                for (var j = 0; j < propertySelectors.Length; j++)
                {
                    sheet.Cells[i + startRowIndex, startColumcIndex + j + 1].Value = propertySelectors[j](items[i]);
                }
            }
        }

        protected void Save(ExcelPackage excelPackage, FileDto file)
        {
            _tempFileCacheManager.SetFile(file.FileToken, excelPackage.GetAsByteArray());
        }
    }
}