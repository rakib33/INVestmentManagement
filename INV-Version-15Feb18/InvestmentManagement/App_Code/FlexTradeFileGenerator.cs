using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using System.IO;
using System.Security.Cryptography;
using System.Collections;
using System.Xml.Schema;

namespace InvestmentManagement.App_Code
{
    public class FlexTradeFileGenerator
    {
        bool removePadding;
        XmlTextReader reader;
        ArrayList results;

        #region Client Limit
        /// <summary>
        /// Generate Flex trade client limit file.
        /// </summary>
        /// <param name="purchansePower">List of client in a DataTable</param>
        /// <param name="fileFullPath">XML file path</param>
        /// <param name="processingMode">BatchInsert / IncrementQuantity . See detail on DSE specification file. </param>
        /// <param name="brokerID">3 char Trading code.</param>
        /// <param name="removePadding">Is Account number add after remove padding?</param>
        /// <param name="addDeleteAllTag">Delete all client from FlexTrade</param>
        /// <param name="registerClients">Register client before add limit</param>
        /// <param name="suspendClients">Suspend client before add limit</param>
        /// <param name="deactivateClients">Deactivate client before add limit</param>
        /// <param name="clientLimit">Add limit in the flex file. You can only generate file with limit.</param>
        //public void GenerateLimitFile(DataTable purchansePower, string fileFullPath, string processingMode, string brokerID, bool removePadding, bool addDeleteAllTag, bool registerClients, bool suspendClients, bool deactivateClients, bool clientLimit)
        //{
        //    try
        //    {
        //        this.removePadding = removePadding;

        //        XmlTextWriter writer = new XmlTextWriter(fileFullPath, System.Text.Encoding.UTF8);
        //        writer.WriteStartDocument(true);
        //        writer.Formatting = Formatting.Indented;
        //        writer.WriteStartElement("Clients");
        //        writer.WriteAttributeString("ProcessingMode", processingMode);
        //        writer.WriteAttributeString("BrokerID", brokerID);
        //        writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
        //        writer.WriteAttributeString("xsi:noNamespaceSchemaLocation", "Flextrade-BOS-Positions.xsd");
        //        writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");

        //        if (addDeleteAllTag)
        //        {
        //            //Deactivate all client
        //            writer.WriteStartElement("DeactivateAllClients");
        //            writer.WriteEndElement();
        //        }

        //        //Register Client                
        //        RegisterClients(purchansePower, writer, registerClients);

        //        //Suspend Client
        //        if (suspendClients)
        //            SuspendClients(purchansePower, writer);

        //        //Deactivate Client
        //        if (deactivateClients)
        //            DeactivateClients(purchansePower, writer);

        //        //Clients Limit
        //        if (clientLimit)
        //            ClientsLimit(purchansePower, writer);


        //        writer.WriteEndElement();
        //        writer.WriteEndDocument();
        //        writer.Close();
        //        //writer.Flush();

        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }

        //}

        //private void RegisterClients(DataTable purchansePower, XmlTextWriter writer, bool registerClients)
        //{
        //    try
        //    {
        //        DataRow[] clientList;

        //        if (registerClients)
        //            clientList = purchansePower.Select();
        //        else
        //            clientList = purchansePower.Select("IsRegistered = 'False'");


        //        foreach (DataRow dtRow in clientList)
        //        {
        //            writer.WriteStartElement("Register");

        //            writer.WriteStartElement("ClientCode");
        //            writer.WriteString(removePadding == false ? dtRow["AccountNumber"].ToString().Trim() : AppVariable.RemoveAccountNumberPadding(dtRow["AccountNumber"].ToString().Trim()));
        //            writer.WriteEndElement();

        //            //writer.WriteStartElement("BranchID");
        //            //writer.WriteString(dtRow["BranchID"].ToString().Trim());
        //            //writer.WriteEndElement();

        //            writer.WriteStartElement("BOID");
        //            writer.WriteString(dtRow["BONumber"].ToString().Trim());
        //            writer.WriteEndElement();

        //            writer.WriteStartElement("WithNetAdjustment");
        //            writer.WriteString("Yes");
        //            writer.WriteEndElement();

        //            writer.WriteStartElement("Name");
        //            string accountName = dtRow["AccountName"].ToString().Trim().Replace('&', ' ').Replace('<', ' ').Replace('>', ' ');
        //            accountName = accountName.Length > 50 ? accountName.Trim().Substring(0, 49) : accountName.ToString().Trim();
        //            writer.WriteString(accountName);
        //            writer.WriteEndElement();

        //            if (dtRow["Mobile"].ToString().Trim().Length > 0)
        //            {
        //                writer.WriteStartElement("Tel");
        //                if (dtRow["Mobile"].ToString().Length > 20)
        //                {
        //                    writer.WriteString(dtRow["Mobile"].ToString().Substring(0, 19).Trim());
        //                }
        //                else
        //                    writer.WriteString(dtRow["Mobile"].ToString().Trim());
        //                writer.WriteEndElement();
        //            }

        //            writer.WriteStartElement("DealerID");
        //            writer.WriteString(dtRow["Trader"].ToString().Trim());
        //            writer.WriteEndElement();

        //            writer.WriteStartElement("AccountType");
        //            writer.WriteString(dtRow["ClientType"].ToString().Trim().Substring(0, 1));
        //            writer.WriteEndElement();


        //            //End Parent Node
        //            writer.WriteEndElement();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //private void SuspendClients(DataTable purchansePower, XmlTextWriter writer)
        //{
        //    try
        //    {
        //        //purchansePower.Select("Status = 'Suspend'");

        //        foreach (DataRow dtRow in purchansePower.Select("Status = 'Suspend'"))
        //        {
        //            writer.WriteStartElement("Suspend");

        //            writer.WriteStartElement("ClientCode");
        //            writer.WriteString(removePadding == false ? dtRow["AccountNumber"].ToString().Trim() : AppVariable.RemoveAccountNumberPadding(dtRow["AccountNumber"].ToString().Trim()));
        //            writer.WriteEndElement();

        //            //writer.WriteStartElement("BranchID");
        //            //writer.WriteString(dtRow["BranchID"].ToString());
        //            //writer.WriteEndElement();

        //            writer.WriteStartElement("Buy_Suspend");
        //            writer.WriteString("Suspend");
        //            writer.WriteEndElement();

        //            writer.WriteStartElement("Sell_Suspend");
        //            writer.WriteString("Suspend");
        //            writer.WriteEndElement();

        //            writer.WriteStartElement("Remark");
        //            writer.WriteString("");
        //            writer.WriteEndElement();


        //            //End Parent Node
        //            writer.WriteEndElement();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //private void DeactivateClients(DataTable purchansePower, XmlTextWriter writer)
        //{
        //    try
        //    {
        //        foreach (DataRow dtRow in purchansePower.Select("Status = 'Closed'"))
        //        {
        //            writer.WriteStartElement("Deactivate");

        //            writer.WriteStartElement("ClientCode");
        //            writer.WriteString(removePadding == false ? dtRow["AccountNumber"].ToString().Trim() : AppVariable.RemoveAccountNumberPadding(dtRow["AccountNumber"].ToString().Trim()));
        //            writer.WriteEndElement();

        //            //writer.WriteStartElement("BranchID");
        //            //writer.WriteString(dtRow["BranchID"].ToString());
        //            //writer.WriteEndElement();


        //            //End Parent Node
        //            writer.WriteEndElement();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //private void ClientsLimit(DataTable purchansePower, XmlTextWriter writer)
        //{
        //    try
        //    {
        //        foreach (DataRow dtRow in purchansePower.Rows)
        //        {
        //            writer.WriteStartElement("Limits");

        //            writer.WriteStartElement("ClientCode");
        //            writer.WriteString(removePadding == false ? dtRow["AccountNumber"].ToString().Trim() : AppVariable.RemoveAccountNumberPadding(dtRow["AccountNumber"].ToString().Trim()));
        //            writer.WriteEndElement();

        //            ////writer.WriteStartElement("BranchID");
        //            ////writer.WriteString(dtRow["BranchID"].ToString());
        //            ////writer.WriteEndElement();

        //            //if (dtRow["AccountType"].ToString() != "NDA" && dtRow["AccountType"].ToString() != "Cash")
        //            //{
        //            //    writer.WriteStartElement("Margin");
        //            //    string margin = dtRow["MarginRatio"].ToString().Substring(0,4);
        //            //    writer.WriteAttributeString("MarginRatio", ".5".ToString());


        //            //    string AccountBalance = dtRow["AccountBalance"].ToString().Split('.')[0];
        //            //    Int64 Balance = Int64.Parse(AccountBalance) < 0 ? 0 : Int64.Parse(AccountBalance);
        //            //    writer.WriteAttributeString("Deposit", Balance.ToString());
        //            //    writer.WriteEndElement();


        //            //    writer.WriteStartElement("Cash");
        //            //    string puchasePower = dtRow["PurchasePower"].ToString().Split('.')[0];
        //            //    writer.WriteString(Int64.Parse(puchasePower).ToString());
        //            //    writer.WriteEndElement();
        //            //}
        //            //else
        //            //{
        //            //    writer.WriteStartElement("Cash");
        //            //    string puchasePower = dtRow["PurchasePower"].ToString().Split('.')[0];                        
        //            //    writer.WriteString(Int64.Parse(puchasePower).ToString());
        //            //    writer.WriteEndElement();
        //            //}

        //            writer.WriteStartElement("Cash");
        //            string puchasePower = "0.00";
        //            try
        //            {
        //                puchasePower = dtRow["PurchasePower"] == null ? "0.00" : dtRow["PurchasePower"].ToString();
        //            }
        //            catch (Exception)
        //            {
        //                puchasePower = "0.00";
        //            }

        //            writer.WriteString(Math.Round(Double.Parse(puchasePower), 0).ToString());
        //            writer.WriteEndElement();
        //            //End Parent Node
        //            writer.WriteEndElement();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        #endregion

        #region Position
        /// <summary>
        /// Generate Position for Flex Trade
        /// </summary>
        /// <param name="shareLimit">All client share limit</param>
        /// <param name="fileFullPath">Position file saving location</param>
        /// <param name="processingMode">BatchInsertOrUpdate / IncrementQuantity</param>
        /// <param name="brokerID">Boker TREC code</param>
        /// <param name="removePadding">Remove Padding from account number such as '00001' will generate as '1'</param>
        /// <param name="deleteAllPosition">All tag to force Flex trade to delete all postion before insert new</param>
        /// <param name="deleteSelectedClient">Delete only those client position whom share limit has send as sharelimit</param>
        public void GeneratePositionFile(List<SellLimit> shareLimit, string fileFullPath, string processingMode, string brokerID, bool removePadding, bool deleteAllPosition, bool deleteSelectedClient)
        {
            try
            {
                this.removePadding = removePadding;

                XmlTextWriter writer = new XmlTextWriter(fileFullPath, System.Text.Encoding.UTF8);
                writer.WriteStartDocument(true);
                writer.Formatting = Formatting.Indented;
                writer.WriteStartElement("Positions");
                writer.WriteAttributeString("ProcessingMode", processingMode);
                writer.WriteAttributeString("BrokerID", brokerID);
                writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
                writer.WriteAttributeString("xsi:noNamespaceSchemaLocation", "Flextrade-BOS-Positions.xsd");
                writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");

                if (deleteAllPosition)
                {
                    writer.WriteStartElement("DeleteAllPositions");
                    writer.WriteEndElement();
                }

                if (deleteSelectedClient)
                    DeletePosition(shareLimit, writer);

                //Insert Position
                InsertPosition(shareLimit, writer);


                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void DeletePosition(List<SellLimit> shareLimit, XmlTextWriter writer)
        {
            try
            {
                foreach (var dtRow in shareLimit)  //DataRow dtRow in new DataView(shareLimit).ToTable(true, "AccountNumber").Rows
                {
                    writer.WriteStartElement("Delete");

                    //writer.WriteStartElement("BranchID");
                    //writer.WriteString(dtRow["BranchID"].ToString());
                    //writer.WriteEndElement();

                    writer.WriteStartElement("ClientCode");
                    string accountNumber = dtRow.AccountNumber.ToString();
                        //removePadding == false ? dtRow["AccountNumber"].ToString().Trim() : "00001"; //AppVariable.RemoveAccountNumberPadding(dtRow["AccountNumber"].ToString().Trim()
                    writer.WriteString(accountNumber);
                    writer.WriteEndElement();

                    //End Parent Node
                    writer.WriteEndElement();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void InsertPosition(List<SellLimit> shareLimit, XmlTextWriter writer)
        {
            try
            {
                foreach (var dtRow in shareLimit)
                {
                    writer.WriteStartElement("InsertOne");
                   
                    writer.WriteStartElement("ClientCode");
                    string accountNumber = dtRow.AccountNumber;
                        //removePadding == false ? dtRow["AccountNumber"].ToString().Trim() : "00001"; //AppVariable.RemoveAccountNumberPadding(dtRow["AccountNumber"].ToString().Trim()
                    writer.WriteString(Convert.ToString(accountNumber));
                    writer.WriteEndElement();

                    writer.WriteStartElement("SecurityCode");
                    writer.WriteString(Convert.ToString(dtRow.ShortName));  //dtRow["ShortName"].ToString()
                    writer.WriteEndElement();

                    writer.WriteStartElement("ISIN");
                    writer.WriteString(Convert.ToString(dtRow.ISIN));  //dtRow["ISIN"].ToString()
                    writer.WriteEndElement();

                    writer.WriteStartElement("Quantity");
                   // string maturedBalance = dtRow["MaturedBalance"].ToString().Split('.')[0];
                    writer.WriteString(Convert.ToString(dtRow.MaturedBalance)); //Int64.Parse(maturedBalance).ToString()
                    writer.WriteEndElement();

                    writer.WriteStartElement("TotalCost");
                    writer.WriteString(Convert.ToString(dtRow.SellLimitCostvalue));  //dtRow["TotalCost"].ToString()
                    writer.WriteEndElement();


                    //End Parent Node
                    writer.WriteEndElement();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Control File
        /// <summary>
        /// Generate Control file for clients or position.
        /// </summary>
        /// <param name="fileFullPath"></param>
        /// <param name="fileSaveLocation">Control file save location</param>
        public void GenerateControlFile(string fileFullPath, string fileSaveLocation)
        {
            try
            {
              //  fileSaveLocation=  //using (StreamWriter w = new StreamWriter(Server.MapPath("~/Reports/ShareReport/SellLimitFile.txt"), true))
            //{
            //    

               XmlTextWriter writer = new XmlTextWriter(fileSaveLocation, System.Text.Encoding.UTF8);
           
                writer.WriteStartDocument(true);
                writer.Formatting = Formatting.Indented;
                writer.WriteStartElement("Control");
                writer.WriteAttributeString("Method", "MD5");
                writer.WriteAttributeString("Hash", GetMD5HashFromFile(fileFullPath));
                writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
                writer.WriteAttributeString("xsi:noNamespaceSchemaLocation", "Flextrade-BOS-Control.xsd");
                writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");


                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
                }

            
            catch (Exception)
            {
                throw;
            }

        }

        private string GetMD5HashFromFile(string fullfileName)
        {
            FileStream file = new FileStream(fullfileName, FileMode.Open);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(file); //32 
            file.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }

            file.Close();
            return sb.ToString();
        }
        #endregion

        #region XML validation with Scheme
        /// <summary>
        /// Validate XML file againts XSD file
        /// </summary>
        /// <param name="xmlFile">XML file location</param>
        /// <param name="xsdFile">XSD file location</param>
        /// <returns></returns>
        public string XMLValidation(string xmlFile, string xsdFile)
        {
            string error = string.Empty;
            results = new ArrayList();
            try
            {


                //Read XML file content
                reader = new XmlTextReader(xmlFile);

                //Read Schema file content
                StreamReader SR = new StreamReader(xsdFile);

                //Create a new instance of XmlSchema object
                XmlSchema Schema = new XmlSchema();


                //Set Schema object by calling XmlSchema.Read() method
                Schema = XmlSchema.Read(SR, new ValidationEventHandler(ReaderSettings_ValidationEventHandler));


                //Create a new instance of XmlReaderSettings object
                XmlReaderSettings ReaderSettings = new XmlReaderSettings();

                //Add Schema to XmlReaderSettings Schemas collection
                ReaderSettings.Schemas.Add(Schema);


                //Set ValidationType for XmlReaderSettings object
                ReaderSettings.ValidationType = ValidationType.Schema;

                //ReaderSettings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
                //ReaderSettings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
                //ReaderSettings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;


                //Add your ValidationEventHandler address to XmlReaderSettings ValidationEventHandler
                ReaderSettings.ValidationEventHandler += new ValidationEventHandler(ReaderSettings_ValidationEventHandler);


                //Create a new instance of XmlReader object
                XmlReader objXmlReader = XmlReader.Create(reader, ReaderSettings);


                //Read XML content in a loop
                while (objXmlReader.Read())
                { }



            }

            catch (UnauthorizedAccessException AccessEx)
            {
                throw AccessEx;
            }

            catch (Exception Ex)
            {
                throw Ex;
            }

            finally
            {
                foreach (string result in results)
                {
                    error = error + result + "\n";
                }
                //if (!(error == string.Empty))
                //{
                //    throw new Exception(error);
                //}

            }

            reader.Close();

            return error;

        }

        private void ReaderSettings_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            string error;
            error = "Line: " + this.reader.LineNumber + " - Position: "
                + this.reader.LinePosition + " - " + e.Message + this.reader;

            this.results.Add(error);

        }
        #endregion

        #region Trade File

        /// <summary>
        /// Generate Trade file
        /// </summary>
        /// <param name="filePath">File save location</param>
        /// <param name="dtTrades">Trade date</param>
        public void GenerateTradeFile(string filePath, DataTable dtTrades)
        {

            XmlTextWriter writer = new XmlTextWriter(filePath, Encoding.GetEncoding("ISO-8859-1"));
            writer.WriteStartDocument(true);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartElement("Trades");

            foreach (DataRow dtRow in dtTrades.Rows)
            {
                writer.WriteStartElement("Detail");


                writer.WriteAttributeString("Action", "EXEC");

                string fillType = "FILL";
                if (dtRow["FillType"].ToString() == "P")
                    fillType = "PF";
                if (dtRow["FillType"].ToString() == "W")
                    fillType = "WON";

                writer.WriteAttributeString("Status", fillType);//Fill Type
                writer.WriteAttributeString("ISIN", dtRow["ISIN"].ToString());

                string assetClass = "EQ";
                if (dtRow["Instrument"].ToString().Contains("MF"))
                    assetClass = "MF";
                if (dtRow["Market"].ToString() == "D")
                    assetClass = "CB";

                writer.WriteAttributeString("AssetClass", assetClass);
                writer.WriteAttributeString("OrderID", dtRow["OrderNo"].ToString());//Order ID
                writer.WriteAttributeString("RefOrderID", dtRow["OrderNo"].ToString());
                writer.WriteAttributeString("Side", dtRow["TransactionType"].ToString());
                writer.WriteAttributeString("BOID", dtRow["BONumber"].ToString());
                writer.WriteAttributeString("SecurityCode", dtRow["Instrument"].ToString());

                string board = "PUBLIC";
                if (dtRow["Market"].ToString() == "B")
                    board = "BLOCK";
                if (dtRow["Market"].ToString() == "D")
                    board = "DEBT";
                if (dtRow["CompSpotID"].ToString() == "Y")
                    board = "SPOT";

                writer.WriteAttributeString("Board", board);
                writer.WriteAttributeString("Date", DateTime.Parse(dtRow["TransactionDate"].ToString()).ToString("yyyyMMdd"));
                writer.WriteAttributeString("Time", DateTime.Parse(dtRow["TransactionTime"].ToString()).ToString("HH:mm:ss"));
                writer.WriteAttributeString("Quantity", dtRow["ShareQuantity"].ToString().Split('.')[0]);
                writer.WriteAttributeString("Price", dtRow["Rate"].ToString());
                writer.WriteAttributeString("Value", dtRow["TotalAmount"].ToString());
                writer.WriteAttributeString("ExecID", dtRow["ContractNo"].ToString());
                writer.WriteAttributeString("Session", "CONTINUOUS");
                writer.WriteAttributeString("FillType", fillType);
                writer.WriteAttributeString("Category", dtRow["InstrumentCategory"].ToString());
                writer.WriteAttributeString("CompulsorySpot", dtRow["CompSpotID"].ToString());
                string accountNumber = dtRow["InvestorACRef"].ToString();
                writer.WriteAttributeString("ClientCode", accountNumber);
                writer.WriteAttributeString("TraderDealerID", dtRow["Trader"].ToString());
                writer.WriteAttributeString("OwnerDealerID", dtRow["Trader"].ToString());
                writer.WriteAttributeString("TradeReportType", "-");

                writer.WriteFullEndElement();
                //writer.WriteEndElement();

            }


            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();

        }


        #endregion


    }
}