using System;
using System.Collections.Generic;
using System.Data.EntityClient;
using System.Linq;
using System.Web;
using InvestmentManagement.Models;

namespace InvestmentManagement.InvestmentManagement.Models
{
    public class TransactionEngine
    {
        public bool CreateFinancialTransaction(string transactionType, decimal? amount, DateTime? transactionDate, string description = "")
        {
            try
            {
                Entities dbEntityies = new Entities(HttpContext.Current.Session["Connection"] as EntityConnection);

                TRANSACTIONPOSTINGMATRIX oMatrix = dbEntityies.TRANSACTIONPOSTINGMATRIces.Where(tpm => tpm.TRANSACTIONCODE == transactionType).FirstOrDefault();

                if (oMatrix == null)
                    throw new Exception("Posting martix for [" + transactionType + "] is not found.");


                //Create Jounal Head
                JOURNALHEAD oJournalHead = new JOURNALHEAD();
                oJournalHead.REFERENCE = Guid.NewGuid().ToString();
                oJournalHead.TRANSACTIONDATE = transactionDate;
                oJournalHead.JOURNALTYPE = "NOM";
                oJournalHead.DESCRIPTION = oMatrix.DESCRIPTION;
                oJournalHead.STATUS = "Posted";
                oJournalHead.CREATEDBY = HttpContext.Current.Session["UserId"].ToString();
                oJournalHead.CREATEDDATE = DateTime.Now;
                oJournalHead.LASTUPDATED = DateTime.Now;
                new CommonFunction().CustomObjectNullValidation(ref oJournalHead);
                ////Add Jounal Head
                //dbEntityies.JOURNALHEADs.Add(oJournalHead);


                //Add Debit side
                JOURNALLINE debitJounal = new JOURNALLINE();
                debitJounal.REFERENCE = Guid.NewGuid().ToString();
                debitJounal.JOURNALHEAD = oJournalHead;
                debitJounal.NOMINALACCOUNT = GetDebitNominalAccount(oMatrix, dbEntityies);
                debitJounal.DESCRIPTION = oMatrix.DESCRIPTION;
                debitJounal.DEBIT = amount;
                debitJounal.NETAMOUNT = amount;
                new CommonFunction().CustomObjectNullValidation(ref debitJounal);
                //Add 
                dbEntityies.JOURNALLINEs.Add(debitJounal);

                //Add Credit Side
                JOURNALLINE creditJounal = new JOURNALLINE();
                creditJounal.REFERENCE = Guid.NewGuid().ToString();
                creditJounal.JOURNALHEAD = oJournalHead;
                creditJounal.NOMINALACCOUNT = GetCreditNominalAccount(oMatrix, dbEntityies);
                creditJounal.DESCRIPTION = oMatrix.DESCRIPTION;
                creditJounal.DEBIT = -1 * amount;
                creditJounal.NETAMOUNT = -1 * amount;
                new CommonFunction().CustomObjectNullValidation(ref creditJounal);
                //Add 
                dbEntityies.JOURNALLINEs.Add(creditJounal);

                dbEntityies.SaveChanges();

            }
            catch (Exception)
            {
                throw;
            }

            return true;
        }

        private NOMINALACCOUNT GetCreditNominalAccount(TRANSACTIONPOSTINGMATRIX oTransactionPostingMatrix, Entities dbEntityies)
        {
            NOMINALACCOUNT oNominalAccount = null;


            try
            {

                if (oTransactionPostingMatrix != null)
                {
                    if (oTransactionPostingMatrix.BSCREDITCONTROL != null)
                    {
                        oNominalAccount = dbEntityies.NOMINALACCOUNTs.Where(nl => nl.CODE == oTransactionPostingMatrix.BSCREDITCONTROL).FirstOrDefault();
                    }
                    if (oTransactionPostingMatrix.PLCREDITCONTROL != null)
                    {
                        oNominalAccount = dbEntityies.NOMINALACCOUNTs.Where(nl => nl.CODE == oTransactionPostingMatrix.PLCREDITCONTROL).FirstOrDefault();

                    }

                    if (oTransactionPostingMatrix.BANKCREDITCONTROL != null)
                    {
                        oNominalAccount = dbEntityies.NOMINALACCOUNTs.Where(nl => nl.CODE == oTransactionPostingMatrix.BANKCREDITCONTROL).FirstOrDefault();
                    }
                }
                else
                {
                    //throw new Exception("Transaction Matrix is not found for Transaction Type " + oTransactionType);

                }

                if (oNominalAccount == null)
                {
                    //throw new Exception("Credit control is not found for Transaction Type " + oTransactionType + " in Transaction Matrix");
                }

            }
            catch (Exception)
            {

                throw;
            }


            return oNominalAccount;
        }

        private NOMINALACCOUNT GetDebitNominalAccount(TRANSACTIONPOSTINGMATRIX oTransactionPostingMatrix, Entities dbEntityies)
        {
            NOMINALACCOUNT oNominalAccount = null;



            try
            {
                if (oTransactionPostingMatrix != null)
                {
                    if (oTransactionPostingMatrix.BSDEBITCONTROL != null)
                    {
                        oNominalAccount = dbEntityies.NOMINALACCOUNTs.Where(nl => nl.CODE == oTransactionPostingMatrix.BSDEBITCONTROL).FirstOrDefault();
                    }
                    if (oTransactionPostingMatrix.PLDEBITCONTROL != null)
                    {
                        oNominalAccount = dbEntityies.NOMINALACCOUNTs.Where(nl => nl.CODE == oTransactionPostingMatrix.PLDEBITCONTROL).FirstOrDefault();
                    }

                    if (oTransactionPostingMatrix.BANKDEBITCONTROL != null)
                    {
                        oNominalAccount = dbEntityies.NOMINALACCOUNTs.Where(nl => nl.CODE == oTransactionPostingMatrix.BANKDEBITCONTROL).FirstOrDefault();
                    }
                }
                else
                {
                    //throw new Exception("Transaction Matrix is not found for Transaction Type " + oTransactionType);

                }

                if (oNominalAccount == null)
                {
                    //throw new Exception("Debit control is not found for Transaction Type " + oTransactionType + " in Transaction Matrix");
                }

            }
            catch (Exception)
            {

                throw;
            }


            return oNominalAccount;
        }
    }
}