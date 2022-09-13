using Abp.Data;
using Abp.EntityFrameworkCore;
using Infogroup.IDMS.Contacts.Dtos;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using System;
using System.Collections.Generic;
using System.Data;

namespace Infogroup.IDMS.Contacts
{
    public class ContactRepository : IDMSRepositoryBase<Contact, int>, IContactRepository
    {
        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        public ContactRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, DatabaseHelper.DatabaseHelper databaseHelper)
            : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
        }

        public List<CreateOrEditContactDto> GetContacts(int Id, ContactType contactType, ContactTableType contactTableType)
        {
            var result = new List<CreateOrEditContactDto>();
            _databaseHelper.EnsureConnectionOpen();
            using (var command = _databaseHelper.CreateCommand($@"
                                                SELECT TBLCONTACT.ID, CONTACTID, STUFF(
                                                                                        ISNULL(', ' + nullif(TBLCONTACT.CADDRESS1, ''), '') + 
                                                                                        ISNULL(', ' + nullif( TBLCONTACT.CADDRESS2,''), '') + 
                                                                                        ISNULL(', ' + nullif(TBLCONTACT.CCITY, '') , '')+ 
                                                                                        ISNULL(', ' + nullif(TBLCONTACT.cstate, ''), '')+
                                                                                        ISNULL(', ' + nullif(TBLCONTACT.cZip, ''), '')
                                                 ,1,1,'') as cAddress1, CFIRSTNAME, CTITLE, CLASTNAME, TBLCONTACT.CADDRESS1, TBLCONTACT.CCity, 
	                                            TBLCONTACT.cstate,TBLCONTACT.czip, TBLCONTACT.CADDRESS2, TBLCONTACT.CPHONE1,TBLCONTACT.CPHONE2, 
	                                            TBLCONTACT.CFAX, CEMAILADDRESS, TBLCONTACT.IISACTIVE, CCOMPANY 
                                                FROM  TBLCONTACT 
                                                JOIN {contactTableType.ToString()} ON  {contactTableType.ToString()}.ID = TBLCONTACT.CONTACTID
                                                WHERE cType = {Convert.ToInt32(contactType).ToString()} AND CONTACTID = {Id}
                                                Order By CLASTNAME,CFIRSTNAME", CommandType.Text))

            {
                command.CommandTimeout = 3 * 60;
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        result.Add(new CreateOrEditContactDto
                        {
                            Id = Convert.ToInt32(dataReader["ID"].ToString()),
                            cFirstName = dataReader["cFirstName"].ToString(),
                            cLastName = dataReader["cLastName"].ToString(),
                            cAddress1 = dataReader["cAddress1"].ToString(),
                            cAddress2 = dataReader["cAddress2"].ToString(),
                            cCity = dataReader["cCity"].ToString(),
                            cState = dataReader["cState"].ToString(),
                            cZIP = dataReader["cZIP"].ToString(),
                            cPhone1 = dataReader["cPhone1"].ToString(),
                            cPhone2 = dataReader["cPhone2"].ToString(),
                            cFax=dataReader["CFAX"].ToString(),
                            cEmailAddress = dataReader["cEmailAddress"].ToString()
                        });
                    }

                }
            }
            return result;
        }
    }
}
