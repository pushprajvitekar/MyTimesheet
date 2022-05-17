using System.Collections.Generic;

namespace MyTimesheet
{
    public class Column
    {
        private Column(string value, int index) { Value = value; Index = index; }
        public string Value { get; private set; }
        public int Index { get; private set; }
        public static Column Date => new Column("Date", 0);
        public static Column DailyDb_Cr => new Column("Daily Debit/Credit", 1);
        public static Column CTO_Bal => new Column("CTO Balance", 2);
        public static Column TimeIn => new Column("Time In", 3);
        public static Column Break1_TimeOut => new Column("Break 1[Out]", 4);
        public static Column Break1_TimeIn => new Column("Break 1[In]", 5);
        public static Column Break2_TimeOut => new Column("Break 2[Out]", 6);
        public static Column Break2_TimeIn => new Column("Break 2[In]", 7);
        public static Column Break3_TimeOut => new Column("Break 3[Out]", 8);
        public static Column Break3_TimeIn => new Column("Break 3[In]", 9);

        public static Column TimeOut => new Column("Time Out", 10);

        public static Column AbsenceMorn => new Column("Absence Morning", 11);
        public static Column AbsenceNoon => new Column("Absence Afternoon", 12);
        public static Column CorrBy => new Column("CorrectedBy", 13);
        public static Column Remarks => new Column("Remarks", 14);
        public static Column ChgApprSta => new Column("Change Approval Status", 15);
        public static Column SysMessage => new Column("System Message", 16);

        public static List<Column> All => new List<Column>
        {
            Date,
            DailyDb_Cr,
            CTO_Bal,
            TimeIn,
             Break1_TimeOut,
            Break1_TimeIn,
            Break2_TimeOut,
            Break2_TimeIn,
            Break3_TimeOut,
            Break3_TimeIn,
            TimeOut,
            AbsenceMorn,
            AbsenceNoon,
            CorrBy,
            Remarks,
            ChgApprSta,
            SysMessage
        };
    }
}
