using System;
using System.Data;

namespace GameServerDB
{
    public class SQLResult : DataTable
    {
        public int Count { get; set; }

        public T Read<T>(int row, string columnName, int number = 0)
        {
            return (T)Convert.ChangeType(Rows[row][columnName + (number != 0 ? (1 + number).ToString() : "")], typeof(T));
        }

        public object[] ReadAllValuesFromField(string columnName)
        {
            object[] obj = new object[Count];

            for (int i = 0; i < Count; i++)
                obj[i] = Rows[i][columnName];

            return obj;
        }
    }
}
