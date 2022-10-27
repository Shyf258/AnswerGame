//2019.11.14    关林
//excel 导出逻辑

using Excel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;

namespace ExcelToJson
{
    public class EJ_ExportLogic
    {
        //excel 文件列表
        public List<string> _excelFileList = new List<string>();

        //表格列表
        public List<SDataTable> _excelDataList = new List<SDataTable>();


        public void ExportExcel(EJ_ExportToolInfo exportToolInfo)
        {
            
            foreach (var data in _excelDataList)
            {
                ExportLogic(data, exportToolInfo);
            }
        }

        public void ExportExcelAll(EJ_ExportToolInfo exportToolInfo)
        {
            List<List<SDataTable>> totalList = new List<List<SDataTable>>();

            //读取sheet
            foreach (var file in _excelFileList)
            {
                string currentOpenFilePath = exportToolInfo._excelFileImportPath + "/" + file;
                //currentOpenFilePath = currentOpenFilePath.Replace("/", "\\");
                using (FileStream excelFile = File.Open(currentOpenFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    List<SDataTable> temp = new List<SDataTable>();
                    IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(excelFile);
                    var book = excelReader.AsDataSet();

                    foreach (DataTable dataTable in book.Tables)
                    {
                        SDataTable data = new SDataTable();
                        data._data = dataTable;
                        temp.Add(data);
                    }
                    totalList.Add(temp);
                }
            }

            //读表 创建json
            foreach (var excelData in totalList)
            {
                foreach (var data in excelData)
                {
                    ExportLogic(data, exportToolInfo);
                    //List<int> format = new List<int>();
                    //if (data._isSelected)
                    //{
                    //    ExportJson.ExportJsonFileVertical(data._data, format, exportToolInfo);
                    //    if (exportToolInfo._isExportCs)
                    //        ExportCS.ExportCSFileVertical(data._data, data._data.TableName, format, exportToolInfo);
                    //}
                }
            }
            

        }

        //导出逻辑
        private void ExportLogic(SDataTable data, EJ_ExportToolInfo exportToolInfo)
        {
            bool isVertical;
            List<int> format = new List<int>();
            if (data._isSelected)
            {
                isVertical = ExportJson.IsVertical(data._data, ref format);

                if (isVertical)
                {
                    ExportJson.ExportJsonFileVertical(data._data, format, exportToolInfo);
                    if (exportToolInfo._isExportCs)
                        ExportCS.ExportCSFileVertical(data._data, data._data.TableName, format, exportToolInfo);
                }
                else
                {
                    ExportJson.ExportJsonFile(data._data, format, exportToolInfo);
                    if (exportToolInfo._isExportCs)
                        ExportCS.ExportCSFile(data._data, data._data.TableName, format, exportToolInfo);
                }
            }
        }

        private static EJ_ExportLogic _inst;
        public static EJ_ExportLogic _instance
        {
            private set
            {
                _inst = value;
            }

            get
            {
                if (null == _inst)
                    _inst = new EJ_ExportLogic();
                return _inst;
            }
        }
    }

    public class SDataTable
    {
        public bool _isSelected = true;
        public DataTable _data;
    }

}
