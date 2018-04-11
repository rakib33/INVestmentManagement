using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

using InvestmentManagement.InvestmentManagement.Models;


namespace InvestmentManagement.App_Code
{
    public class PayInOutFileGenarator
    {
        Variable _var = new Variable();

        //firstLine20 char(7char = Investor Number,13 char Total PayIn Share Qty)<1 space>admin(6 digitdp Id)(DSE code=10)
        //CSE code=11
        //businessDate looks = datemmyyyy eg 26092017 for 26th Sep 17
        public string PayInOutFileWrite(string filepath, string payOption, List<SettlementViewModel> models, string businessDate)
        {

            try
            {
                _var.Total = models.Sum(t=>t.ShareQuantity); //get Total Share Quantity that to be Pay In
                _var.TotalRow = models.Count();

                _var.CdblDPId = models.Take(1).SingleOrDefault().CdblDpId;
                _var.DSEExchange = models.Take(1).SingleOrDefault().dseexchangeid;
                              
              
                if (payOption == ConstantVariable.SettlementPayIn)
                {
                    _var.fileName = ConstantVariable.SettlementPayInPrefix;
                    _var.tagChar = ConstantVariable.SettlementPayInTag;

                    //Total 20 digit ,first 7 digit number of Investor ,last 13 digit Total Pay In Share
                    //here DLIC is One Investor so 1st 7 digit 0000001
                    _var.firstCode = StringPaddingLeft(_var.TotalRow.ToString(), 7, '0') + StringPaddingLeft(Convert.ToString(_var.Total), 13, '0');                        
                  
                }
                else if (payOption == ConstantVariable.SettlementPayOut)
                {
                    _var.fileName = ConstantVariable.SettlementPayOutPrefix;
                    _var.tagChar = ConstantVariable.SettlementPayOutTag;
                    _var.firstCode = StringPaddingLeft(_var.TotalRow.ToString(), 7, '0') + StringPaddingLeft(Convert.ToString(_var.Total), 13, '0');                        
                      
                }
                else
                {
                    return "PayIn PayOut option not found."+",";
                }


                _var.CdblDPId = StringPaddingLeft(_var.CdblDPId, 6, '0');

                _var.fileName = _var.fileName + _var.CdblDPId +businessDate + ConstantVariable.SettlementPayInOutFileExt;
                _var.PathString = filepath; //filepath + _var.fileName;

              
                    //first clear all previous text
                    File.WriteAllText(_var.PathString, String.Empty);
                    
                    //then write the file
                    System.IO.TextWriter writeFile = new StreamWriter(_var.PathString, true);                  
                   

                    //write the first line
                    writeFile.WriteLine(_var.firstCode+" "+ ConstantVariable.SettlementAdminMsg + _var.CdblDPId + _var.DSEExchange); // one space after code+6 digit CDBLDpId+..

                    int i = 1;
                    foreach (var item in models)
                    {
                        string Qty = StringPaddingLeft(Convert.ToString(item.ShareQuantity), 12, '0');
                        string SlNo = "" + i;
                       
                        string line = businessDate + item.bonumber +  item.dseclearingbo + StringPaddingLeft(item.TradeNumber, 6, '0') + item.isin + Qty + _var.tagChar + item.investoracref + ConstantVariable.SettlementpayInOutSpace + StringPaddingLeft(SlNo, 12, '0'); //11 space getSerial(SlNo)
                     
                        writeFile.WriteLine(line);
                        i++;
                    }
                    writeFile.Flush();
                    writeFile.Close();
                    writeFile = null;

                    return ConstantVariable.Settlement_Success_msg+","+_var.fileName;

               

            }
            catch (IOException ex)
            {
                return ex.Message+",";
            }

        }

        public string StringPaddingLeft(string Data, int maxlength, char AddValue)
        {
            if (Data.Length < maxlength)
                Data = Data.ToString().PadLeft(maxlength, AddValue);
            return Data;
        }
      
    }
}