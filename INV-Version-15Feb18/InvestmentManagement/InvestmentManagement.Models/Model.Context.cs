﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace InvestmentManagement.InvestmentManagement.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;
    using System.Linq;
    
    public partial class Entities : DbContext
    {

        public Entities()
            : base("name=Entities")
        {

        }
        public Entities(System.Data.EntityClient.EntityConnection entityConnection)
        {
            // TODO: Complete member initialization
            this.entityConnection = entityConnection;
        }
        public System.Data.EntityClient.EntityConnection entityConnection { get; set; }

        
        public DbSet<APPLICATIONPARAMETER> APPLICATIONPARAMETERs { get; set; }
        public DbSet<APPLICATIONPARAMETERDETAIL> APPLICATIONPARAMETERDETAILS { get; set; }
        public DbSet<APPLICATIONUSER> APPLICATIONUSERs { get; set; }
        public DbSet<BANKACCOUNT> BANKACCOUNTs { get; set; }
        public DbSet<BOMAINTENANCELOG> BOMAINTENANCELOGs { get; set; }
        public DbSet<BOND> BONDs { get; set; }
        public DbSet<BOND_CHEQUEDRAWN> BOND_CHEQUEDRAWN { get; set; }
        public DbSet<BONDENCASHMENT> BONDENCASHMENTs { get; set; }
        public DbSet<BONDREDEMPTIONSCHEDULE> BONDREDEMPTIONSCHEDULEs { get; set; }
        public DbSet<BONUS> Bonuses { get; set; }
        public DbSet<BRANCH> BRANCHes { get; set; }
        public DbSet<BROKER> BROKERs { get; set; }
        public DbSet<CASHDIVIDENDDECLARATION> CASHDIVIDENDDECLARATIONs { get; set; }
        public DbSet<CHARGEHEAD> CHARGEHEADs { get; set; }
        public DbSet<CHEQUEDRAWN> CHEQUEDRAWNs { get; set; }
        public DbSet<COMPANY> COMPANies { get; set; }
        public DbSet<CONTROLACCOUNT> CONTROLACCOUNTs { get; set; }
        public DbSet<CORPORATEACTION> CORPORATEACTIONs { get; set; }
        public DbSet<CORPORATEACTIONRECEIVABLE> CORPORATEACTIONRECEIVABLEs { get; set; }
        public DbSet<CURRENCY> CURRENCies { get; set; }
        public DbSet<DAILYCHARGE> DAILYCHARGEs { get; set; }
        public DbSet<DAILYCHARGEDETAIL> DAILYCHARGEDETAILs { get; set; }
        public DbSet<DEMATCONFIRM> DEMATCONFIRMs { get; set; }
        public DbSet<DEPARTMENT> DEPARTMENTs { get; set; }
        public DbSet<DEPOSITORYSETTING> DEPOSITORYSETTINGS { get; set; }
        public DbSet<DIVIDEND> DIVIDENDs { get; set; }
        public DbSet<EXTRADIVIDENDRECEIVED> EXTRADIVIDENDRECEIVEDs { get; set; }
        public DbSet<FDRENCASHMENT> FDRENCASHMENTs { get; set; }
        public DbSet<FDRINTEREST> FDRINTERESTs { get; set; }
        public DbSet<FDRNOTE> FDRNOTEs { get; set; }
        public DbSet<FDRPROPOSAL> FDRPROPOSALs { get; set; }
        public DbSet<FDRPROPOSALDETAIL> FDRPROPOSALDETAILS { get; set; }
        public DbSet<FIBRANCH> FIBRANCHes { get; set; }
        public DbSet<FINANCIALINSTITUTION> FINANCIALINSTITUTIONs { get; set; }
        public DbSet<FINANCIALYEAR> FINANCIALYEARs { get; set; }
        public DbSet<FIXEDDEPOSIT> FIXEDDEPOSITs { get; set; }
        public DbSet<GOVBONDINTERESTSCHEDULE> GOVBONDINTERESTSCHEDULEs { get; set; }
        public DbSet<HOLDING> HOLDINGs { get; set; }
        public DbSet<HOLIDAYCALENDER> HOLIDAYCALENDERs { get; set; }
        public DbSet<INSTRUMENT> INSTRUMENTs { get; set; }
        public DbSet<INSTRUMENTCATEGORY> INSTRUMENTCATEGORies { get; set; }
        public DbSet<INVESTMENTPARTICULAR> INVESTMENTPARTICULARS { get; set; }
        public DbSet<INVESTOR> INVESTORs { get; set; }
        public DbSet<INVESTORGROUP> INVESTORGROUPs { get; set; }
        public DbSet<INVESTORPROFILE> INVESTORPROFILEs { get; set; }
        public DbSet<INVPARTICULARSDETAIL> INVPARTICULARSDETAILS { get; set; }
        public DbSet<IPO> IPOes { get; set; }
        public DbSet<JOINTHOLDER> JOINTHOLDERs { get; set; }
        public DbSet<JOURNALHEAD> JOURNALHEADs { get; set; }
        public DbSet<JOURNALLINE> JOURNALLINEs { get; set; }
        public DbSet<MD_ADDITIONAL_PROPERTIES> MD_ADDITIONAL_PROPERTIES { get; set; }
        public DbSet<MD_APPLICATIONFILES> MD_APPLICATIONFILES { get; set; }
        public DbSet<MD_APPLICATIONS> MD_APPLICATIONS { get; set; }
        public DbSet<MD_CATALOGS> MD_CATALOGS { get; set; }
        public DbSet<MD_COLUMNS> MD_COLUMNS { get; set; }
        public DbSet<MD_CONNECTIONS> MD_CONNECTIONS { get; set; }
        public DbSet<MD_CONSTRAINT_DETAILS> MD_CONSTRAINT_DETAILS { get; set; }
        public DbSet<MD_CONSTRAINTS> MD_CONSTRAINTS { get; set; }
        public DbSet<MD_DERIVATIVES> MD_DERIVATIVES { get; set; }
        public DbSet<MD_FILE_ARTIFACTS> MD_FILE_ARTIFACTS { get; set; }
        public DbSet<MD_GROUP_MEMBERS> MD_GROUP_MEMBERS { get; set; }
        public DbSet<MD_GROUP_PRIVILEGES> MD_GROUP_PRIVILEGES { get; set; }
        public DbSet<MD_GROUPS> MD_GROUPS { get; set; }
        public DbSet<MD_INDEX_DETAILS> MD_INDEX_DETAILS { get; set; }
        public DbSet<MD_INDEXES> MD_INDEXES { get; set; }
        public DbSet<MD_MIGR_DEPENDENCY> MD_MIGR_DEPENDENCY { get; set; }
        public DbSet<MD_MIGR_PARAMETER> MD_MIGR_PARAMETER { get; set; }
        public DbSet<MD_MIGR_WEAKDEP> MD_MIGR_WEAKDEP { get; set; }
        public DbSet<MD_OTHER_OBJECTS> MD_OTHER_OBJECTS { get; set; }
        public DbSet<MD_PACKAGES> MD_PACKAGES { get; set; }
        public DbSet<MD_PARTITIONS> MD_PARTITIONS { get; set; }
        public DbSet<MD_PRIVILEGES> MD_PRIVILEGES { get; set; }
        public DbSet<MD_PROJECTS> MD_PROJECTS { get; set; }
        public DbSet<MD_REGISTRY> MD_REGISTRY { get; set; }
        public DbSet<MD_REPOVERSIONS> MD_REPOVERSIONS { get; set; }
        public DbSet<MD_SCHEMAS> MD_SCHEMAS { get; set; }
        public DbSet<MD_SEQUENCES> MD_SEQUENCES { get; set; }
        public DbSet<MD_STORED_PROGRAMS> MD_STORED_PROGRAMS { get; set; }
        public DbSet<MD_SYNONYMS> MD_SYNONYMS { get; set; }
        public DbSet<MD_TABLES> MD_TABLES { get; set; }
        public DbSet<MD_TABLESPACES> MD_TABLESPACES { get; set; }
        public DbSet<MD_TRIGGERS> MD_TRIGGERS { get; set; }
        public DbSet<MD_USER_DEFINED_DATA_TYPES> MD_USER_DEFINED_DATA_TYPES { get; set; }
        public DbSet<MD_USER_PRIVILEGES> MD_USER_PRIVILEGES { get; set; }
        public DbSet<MD_USERS> MD_USERS { get; set; }
        public DbSet<MD_VIEWS> MD_VIEWS { get; set; }
        public DbSet<MENU> MENUs { get; set; }
        public DbSet<MIGR_DATATYPE_TRANSFORM_MAP> MIGR_DATATYPE_TRANSFORM_MAP { get; set; }
        public DbSet<MIGR_DATATYPE_TRANSFORM_RULE> MIGR_DATATYPE_TRANSFORM_RULE { get; set; }
        public DbSet<MIGR_GENERATION_ORDER> MIGR_GENERATION_ORDER { get; set; }
        public DbSet<MIGRLOG> MIGRLOGs { get; set; }
        public DbSet<NOMINALACCOUNT> NOMINALACCOUNTs { get; set; }
        public DbSet<PERIOD> PERIODs { get; set; }
        public DbSet<PLEDGE> PLEDGEs { get; set; }
        public DbSet<PORTFOLIOINSTRUMENT> PORTFOLIOINSTRUMENTs { get; set; }
        public DbSet<PRICEINDEX> PRICEINDEXes { get; set; }
        public DbSet<PRIVATEBOND> PRIVATEBONDs { get; set; }
        public DbSet<RIGHTSHAREDECLARATION> RIGHTSHAREDECLARATIONs { get; set; }
        public DbSet<SCRIPTTRANSFER> SCRIPTTRANSFERs { get; set; }
        public DbSet<SETTLEMENT> SETTLEMENTs { get; set; }
        public DbSet<SIGNATORY> SIGNATORies { get; set; }
        public DbSet<SIGNATORYDETAIL> SIGNATORYDETAILS { get; set; }
        public DbSet<STATUSPARAMETER> STATUSPARAMETERs { get; set; }
        public DbSet<STATUSPARAMETERDETAIL> STATUSPARAMETERDETAILS { get; set; }
        public DbSet<TEST> TESTs { get; set; }
        public DbSet<TRADE> TRADEs { get; set; }
        public DbSet<TRADINGCHARGE> TRADINGCHARGEs { get; set; }
        public DbSet<TRANSACTIONPOSTINGMATRIX> TRANSACTIONPOSTINGMATRIces { get; set; }
        public DbSet<TRANSFEROFSECURITy> TRANSFEROFSECURITIES { get; set; }
        public DbSet<USERGROUP> USERGROUPs { get; set; }
        public DbSet<WEEKEND> WEEKENDs { get; set; }
        public DbSet<XX_INVEST_GL_INTEGRATION_DATA> XX_INVEST_GL_INTEGRATION_DATA { get; set; }
        public DbSet<FDR_MAINSTATEMENT_VIEW> FDR_MAINSTATEMENT_VIEW { get; set; }
        public DbSet<PARTICULARS_VIEW> PARTICULARS_VIEW { get; set; }
    
        public virtual int FIX_MUTUALFUND_CAP_BONUS(string gIVEN_ISIN)
        {
            var gIVEN_ISINParameter = gIVEN_ISIN != null ?
                new ObjectParameter("GIVEN_ISIN", gIVEN_ISIN) :
                new ObjectParameter("GIVEN_ISIN", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("FIX_MUTUALFUND_CAP_BONUS", gIVEN_ISINParameter);
        }
    
        public virtual int GETBALANCESHEET()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GETBALANCESHEET");
        }
    
        public virtual int GETMAXPOSTINGMATRIX(string tRANTYPE)
        {
            var tRANTYPEParameter = tRANTYPE != null ?
                new ObjectParameter("TRANTYPE", tRANTYPE) :
                new ObjectParameter("TRANTYPE", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GETMAXPOSTINGMATRIX", tRANTYPEParameter);
        }
    
        public virtual int GETPORTFOLIOCARECEIVABLE(Nullable<System.DateTime> eFFECTIVE_DATE)
        {
            var eFFECTIVE_DATEParameter = eFFECTIVE_DATE.HasValue ?
                new ObjectParameter("EFFECTIVE_DATE", eFFECTIVE_DATE) :
                new ObjectParameter("EFFECTIVE_DATE", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GETPORTFOLIOCARECEIVABLE", eFFECTIVE_DATEParameter);
        }
    
        public virtual int GETPOSTINGMATRIX(string tRANCODE)
        {
            var tRANCODEParameter = tRANCODE != null ?
                new ObjectParameter("TRANCODE", tRANCODE) :
                new ObjectParameter("TRANCODE", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GETPOSTINGMATRIX", tRANCODEParameter);
        }
    
        public virtual int GET_BONUS_SHARE_RECEIVED(Nullable<System.DateTime> fROMDATE, Nullable<System.DateTime> tODATE)
        {
            var fROMDATEParameter = fROMDATE.HasValue ?
                new ObjectParameter("FROMDATE", fROMDATE) :
                new ObjectParameter("FROMDATE", typeof(System.DateTime));
    
            var tODATEParameter = tODATE.HasValue ?
                new ObjectParameter("TODATE", tODATE) :
                new ObjectParameter("TODATE", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GET_BONUS_SHARE_RECEIVED", fROMDATEParameter, tODATEParameter);
        }
    
        public virtual int GET_CLIENTACCOUNTOPENBALANCE(string aCCOUNTNUMBER, Nullable<System.DateTime> fROMDATE)
        {
            var aCCOUNTNUMBERParameter = aCCOUNTNUMBER != null ?
                new ObjectParameter("ACCOUNTNUMBER", aCCOUNTNUMBER) :
                new ObjectParameter("ACCOUNTNUMBER", typeof(string));
    
            var fROMDATEParameter = fROMDATE.HasValue ?
                new ObjectParameter("FROMDATE", fROMDATE) :
                new ObjectParameter("FROMDATE", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GET_CLIENTACCOUNTOPENBALANCE", aCCOUNTNUMBERParameter, fROMDATEParameter);
        }
    
        public virtual int GET_COMMISSION_STATEMENT(Nullable<System.DateTime> fROMDATE, Nullable<System.DateTime> tODATE)
        {
            var fROMDATEParameter = fROMDATE.HasValue ?
                new ObjectParameter("FROMDATE", fROMDATE) :
                new ObjectParameter("FROMDATE", typeof(System.DateTime));
    
            var tODATEParameter = tODATE.HasValue ?
                new ObjectParameter("TODATE", tODATE) :
                new ObjectParameter("TODATE", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GET_COMMISSION_STATEMENT", fROMDATEParameter, tODATEParameter);
        }
    
        public virtual int GET_DIVIDEND_LIST(string sTATUS)
        {
            var sTATUSParameter = sTATUS != null ?
                new ObjectParameter("STATUS", sTATUS) :
                new ObjectParameter("STATUS", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GET_DIVIDEND_LIST", sTATUSParameter);
        }
    
        public virtual int GET_DIVIDEND_RECEIVABLE(Nullable<System.DateTime> eFFECTIVE_DATE)
        {
            var eFFECTIVE_DATEParameter = eFFECTIVE_DATE.HasValue ?
                new ObjectParameter("EFFECTIVE_DATE", eFFECTIVE_DATE) :
                new ObjectParameter("EFFECTIVE_DATE", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GET_DIVIDEND_RECEIVABLE", eFFECTIVE_DATEParameter);
        }
    
        public virtual int GET_EXTRADIVIDEND_RECEIVED(Nullable<System.DateTime> fROMDATE, Nullable<System.DateTime> tODATE)
        {
            var fROMDATEParameter = fROMDATE.HasValue ?
                new ObjectParameter("FROMDATE", fROMDATE) :
                new ObjectParameter("FROMDATE", typeof(System.DateTime));
    
            var tODATEParameter = tODATE.HasValue ?
                new ObjectParameter("TODATE", tODATE) :
                new ObjectParameter("TODATE", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GET_EXTRADIVIDEND_RECEIVED", fROMDATEParameter, tODATEParameter);
        }
    
        public virtual int GET_INVESTORLEDGERSTATEMENT(string aCCOUNTNUMBER, Nullable<System.DateTime> fROMDATE, Nullable<System.DateTime> tODATE)
        {
            var aCCOUNTNUMBERParameter = aCCOUNTNUMBER != null ?
                new ObjectParameter("ACCOUNTNUMBER", aCCOUNTNUMBER) :
                new ObjectParameter("ACCOUNTNUMBER", typeof(string));
    
            var fROMDATEParameter = fROMDATE.HasValue ?
                new ObjectParameter("FROMDATE", fROMDATE) :
                new ObjectParameter("FROMDATE", typeof(System.DateTime));
    
            var tODATEParameter = tODATE.HasValue ?
                new ObjectParameter("TODATE", tODATE) :
                new ObjectParameter("TODATE", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GET_INVESTORLEDGERSTATEMENT", aCCOUNTNUMBERParameter, fROMDATEParameter, tODATEParameter);
        }
    
        public virtual int GET_PROFITLOSS_STATEMENT(string tO_DATE, string fROM_DATE, string aCCOUNTNUMBER, string sHORTNAME)
        {
            var tO_DATEParameter = tO_DATE != null ?
                new ObjectParameter("TO_DATE", tO_DATE) :
                new ObjectParameter("TO_DATE", typeof(string));
    
            var fROM_DATEParameter = fROM_DATE != null ?
                new ObjectParameter("FROM_DATE", fROM_DATE) :
                new ObjectParameter("FROM_DATE", typeof(string));
    
            var aCCOUNTNUMBERParameter = aCCOUNTNUMBER != null ?
                new ObjectParameter("ACCOUNTNUMBER", aCCOUNTNUMBER) :
                new ObjectParameter("ACCOUNTNUMBER", typeof(string));
    
            var sHORTNAMEParameter = sHORTNAME != null ?
                new ObjectParameter("SHORTNAME", sHORTNAME) :
                new ObjectParameter("SHORTNAME", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GET_PROFITLOSS_STATEMENT", tO_DATEParameter, fROM_DATEParameter, aCCOUNTNUMBERParameter, sHORTNAMEParameter);
        }
    
        public virtual int GET_RIGHT_SHARE_RECEIVED(Nullable<System.DateTime> fROMDATE, Nullable<System.DateTime> tODATE)
        {
            var fROMDATEParameter = fROMDATE.HasValue ?
                new ObjectParameter("FROMDATE", fROMDATE) :
                new ObjectParameter("FROMDATE", typeof(System.DateTime));
    
            var tODATEParameter = tODATE.HasValue ?
                new ObjectParameter("TODATE", tODATE) :
                new ObjectParameter("TODATE", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GET_RIGHT_SHARE_RECEIVED", fROMDATEParameter, tODATEParameter);
        }
    }
}