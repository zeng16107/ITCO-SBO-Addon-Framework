﻿using SAPbobsCOM;
using System;

namespace ITCO.SboAddon.Framework.Helpers
{
    public class UserDefinedHelper
    {
        public static bool CreateTable(string tableName, string tableDescription, BoFieldTypes type, int size = 30)
        {
            UserTablesMD userTablesMD = null;

            try
            {
                userTablesMD = SboApp.Company.GetBusinessObject(BoObjectTypes.oUserTables) as UserTablesMD;

                if (!userTablesMD.GetByKey(tableName))
                {
                    userTablesMD.TableName = tableName;
                    userTablesMD.TableDescription = tableDescription;
                    ErrorHelper.HandleErrorWithException(
                        userTablesMD.Add(), 
                        string.Format("Could not create UDT {0}", tableName));
                }
            }
            catch (Exception ex)
            {
                SboApp.Application.MessageBox(ex.Message);
                return false;
            }
            finally
            {
                if (userTablesMD != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(userTablesMD);
            }

            return true;
        }

        public static bool CreateField(string tableName, string fieldName, string fieldDescription, BoFieldTypes type, int size = 30)
        {
            UserFieldsMD userFieldsMD = null;

            try
            {
                userFieldsMD = SboApp.Company.GetBusinessObject(BoObjectTypes.oUserFields) as UserFieldsMD;

                var fieldId = UserDefinedHelper.GetFieldId(tableName, fieldName);
                if (fieldId == -1)
                {
                    userFieldsMD.TableName = tableName;
                    userFieldsMD.Name = fieldName;
                    userFieldsMD.Description = fieldDescription;
                    userFieldsMD.Type = type;
                    userFieldsMD.Size = size;
                    userFieldsMD.EditSize = size;
                    ErrorHelper.HandleErrorWithException(
                        userFieldsMD.Add(), 
                        string.Format("Could not create {0} on {1}", fieldName, tableName));
                }
            }
            catch (Exception ex)
            {
                SboApp.Application.MessageBox(ex.Message);
                return false;
            }
            finally
            {
                if (userFieldsMD != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(userFieldsMD);
            }

            return true;
        }


        public static int GetFieldId(string tableName, string fieldAlias)
        {
            var recordSet = SboApp.Company.GetBusinessObject(BoObjectTypes.BoRecordset) as Recordset;

            try
            {
                recordSet.DoQuery(string.Format("SELECT FieldID FROM CUFD WHERE TableID='{0}' AND AliasID='{1}'", tableName, fieldAlias));

                if (recordSet.RecordCount == 1)
                {
                    var fieldId = recordSet.Fields.Item("FieldID").Value as int?;
                    return fieldId.Value;
                }
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(recordSet);
            }
            return -1;
        }
    }
}
