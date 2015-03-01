using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simulation
{
    public partial class InputAndOutput : Form
    {
        int NoOfCustomer, ArrivalTime, NoOfService , SelectServer;
        int[] TimeServiceEnds;

        public void Input()
        {
            try
            {
                FullTable(ref IDOC);//Fill "Interarrival Distribution of calls" Table
                FullTable(ref ASTD);//Fill "Able’s Service Time  distribution " Table
                FullTable(ref BSTD);//Fill "Baker’s Service Time  distribution " Table
                NoOfCustomer = int.Parse(NoCus.Text);//Get #Customer from user
                ArrivalTime = int.Parse(Arrival_Time.Text);//Get first Arrival Time
                NoOfService = 2;//#Service
                TimeServiceEnds = new int[NoOfService];//stor the time when this server end current customer
                string Select = ServerSelection.SelectedItem.ToString();//Kind of select when server is busy
                switch(Select)
                {
                    case "Highest Priority":
                        SelectServer = 0;
                        break;
                    case"Random":
                        SelectServer = 1;
                        break;
                    case"Lowest utilization":
                        SelectServer = 2;
                        break;
                    default:
                        SelectServer = 0;
                        break;
                }
            }
            catch
            {
                MessageBox.Show("Error!!");
            }
        }

        void FullTable(ref DataGridView Table)
        {
            double CP = 0/*Cumulative probability*/, OldCP = 0/*Last Cumulative probability*/;
            for (int row = 0; row < Table.RowCount && Table[0, row].Value != null && Table[1, row].Value != null; row++)//Loop for every valid row
             {
                OldCP = CP;
                CP += double.Parse(Table[1, row].Value.ToString());//This row probability added to Cumulative
                Table[2, row].Value = CP;//Fill Cumulative probability
                Table[3, row].Value = ((OldCP * 100).ToString() + "-" + (CP * 100).ToString());//Fill Rang
            }
        }

        public void Output()
        {
            ResultTable.RowCount = NoOfCustomer;//make #rows in result table = #customers
            for (int Customer = 1; Customer <= NoOfCustomer; Customer++)//Loop each customer
            {
                ResultTable[0, Customer - 1].Value = Customer.ToString();//#customer
                if (Customer == 1)//If first customer
                {
                    ResultTable[1, Customer - 1].Value = "0";//Inter Arrival Time
                    ResultTable[2, Customer - 1].Value = ArrivalTime.ToString();//Arrival Time
                    ResultTable[3, Customer - 1].Value = "1";//Server Index
                    ResultTable[4, Customer - 1].Value = ArrivalTime.ToString();//Time Serves begin
                    ResultTable[5, Customer - 1].Value = ServiceTime(1).ToString();//Serves duration
                    ResultTable[6, Customer - 1].Value = (int.Parse(ResultTable[5, Customer - 1].Value.ToString()) + int.Parse(ResultTable[4, Customer - 1].Value.ToString())).ToString();//Time Serves End
                    ResultTable[7, Customer - 1].Value = "0";//Delay

                    TimeServiceEnds[0] = int.Parse(ResultTable[6, Customer - 1].Value.ToString());//End time for Server 1
                }
                else
                {
                    ResultTable[1, Customer - 1].Value = InterarrivalTime().ToString();//Inter Arrival Time
                    ResultTable[2, Customer - 1].Value = (int.Parse(ResultTable[1, Customer - 1].Value.ToString()) + int.Parse(ResultTable[2, Customer - 2].Value.ToString())).ToString();//Arrival Time
                    int Arrival_Time = int.Parse(ResultTable[2, Customer - 1].Value.ToString());//Arrival_Time
                    int SN=GetServerNum(Arrival_Time);//Get Server Number
                    //Calculate Time Service Begins
                    int Time_Service_Begins = TimeServiceEnds[SN];//Get Tame when server be free
                    if (Arrival_Time >= TimeServiceEnds[SN])// if this time is befor customer arrive then Service Begins will be customer Arrival_Time
                         Time_Service_Begins = Arrival_Time;
                    ResultTable[3, Customer - 1].Value = (SN + 1).ToString();//Server Index
                    ResultTable[4, Customer - 1].Value = Time_Service_Begins.ToString();//Time Serves begin
                    ResultTable[5, Customer - 1].Value = ServiceTime(int.Parse(ResultTable[3, Customer - 1].Value.ToString())).ToString();//Serves duration
                    ResultTable[6, Customer - 1].Value = (int.Parse(ResultTable[5, Customer - 1].Value.ToString()) + int.Parse(ResultTable[4, Customer - 1].Value.ToString())).ToString();//Time Serves End
                    ResultTable[7, Customer - 1].Value = (int.Parse(ResultTable[4, Customer - 1].Value.ToString()) - int.Parse(ResultTable[2, Customer - 1].Value.ToString())).ToString();//Delay

                    TimeServiceEnds[SN] = int.Parse(ResultTable[6, Customer - 1].Value.ToString());//End time for Server #??
                }

            }

            SystemOutput();
        }

        int GetServerNum(int Arrival_Time)
        {
            int Min = int.MaxValue, index = 0;//Min value
            if (SelectServer == 0)//First way
            {
                for (int i = 0; i < NoOfService; i++)//Loop on all servers
                {
                    if (Arrival_Time >= TimeServiceEnds[i])//if there is one free now get it
                        return i;
                    if (TimeServiceEnds[i] < Min)//get min time to finish in each server
                    {
                        Min = TimeServiceEnds[i];
                        index = i;
                    }
                }
                return index;//if all busy return first one will free
            }
            if (SelectServer == 1)//Second way
            {
                bool[] free = new bool[NoOfService];
                Random rnd = new Random();
                int value = rnd.Next(0, NoOfService),NF=0;
                for (int i = 0; i < NoOfService; i++)//Loop on all servers
                {
                    if (Arrival_Time >= TimeServiceEnds[i])//if there is one free now get it
                    {
                        free[i] = true;
                        NF++;
                    }
                    if (TimeServiceEnds[i] < Min)//get min time to finish in each server
                    {
                        Min = TimeServiceEnds[i];
                        index = i;
                    }
                }
                if (NF == 0)//if all busy return first one will free
                    return index;
                while (true)//else random on free one
                {
                    if (free[value] == true)
                        return value;
                    value = rnd.Next(0, NoOfService);
                }
            }
            return 0;
        }

        int ServiceTime(int ServerNo)
        {
            Random rnd = new Random();
            int value = rnd.Next(0, 100);

            if(ServerNo==1)
            {
                return GetTime(ASTD, value);
            }
            else if(ServerNo==2)
            {
                return GetTime(BSTD, value);
            }

            return 0;
        }

        int InterarrivalTime()
        {
            Random rnd = new Random();
            int value = rnd.Next(0, 100);

            return GetTime(IDOC, value);
        }

        int GetTime(DataGridView Table,int val)
        {
            for (int row = 0; row < Table.RowCount && Table[3, row].Value != null; row++)//Loop on table
            {
                string[] arr = Table[3, row].Value.ToString().Split('-');
                int A = int.Parse(arr[0]), B = int.Parse(arr[1]);

                if (val >= A && val < B)//if random in this rang return value
                {
                    return int.Parse(Table[0,row].Value.ToString());
                }
            }
            return 0;
        }

        void SystemOutput()
        {
            Dictionary<int,int> MyDictionary = new Dictionary<int,int>();
            CustomersQueueGraph.Series["Series1"].Points.Clear();//CustomersQueueGraph
            QueueSizeHistogram.Series["Series1"].Points.Clear();//QueueSizeHistogramGraph
            BusyTime1.Series["Series1"].Points.Clear();//BusyTime1Graph
            BusyTime2.Series["Series1"].Points.Clear();//BusyTime2Graph
            //////////////////////////////////////////////////////////////
            double[] AverageServiceTime = new double[NoOfService], ProbIdelTime = new double[NoOfService]; 
            double AverageWaitingTimeInTheQueue; 
            int MaximumQueueLength; 
            double ProbabilityThatCustomerWaitInTheQueue;

            int WaitingTimeInTheQueue = 0, CustomerWaitInTheQueue=0 , TotalServTime = int.Parse(ResultTable[6,ResultTable.RowCount-1].Value.ToString());
            int[] IdleTimeOfServer = new int[NoOfService], LastWorkTime = new int[NoOfService], ServiceTimeOfServer = new int[NoOfService], ServiceCustomer = new int[NoOfService];
            ///////////////////////////////////////////////////////////////
            bool[,] ServerWork = new bool[TotalServTime + 1, NoOfService]; 
            ///////////////////////////////////////////////////////////////
            for (int row = 0; row < ResultTable.RowCount && ResultTable[7, row].Value != null; row++)//Loop on result table
            {
                WaitingTimeInTheQueue += int.Parse(ResultTable[7, row].Value.ToString());
                if (ResultTable[7, row].Value.ToString() !="0") 
                    CustomerWaitInTheQueue++;
                //
                IdleTimeOfServer[int.Parse(ResultTable[3, row].Value.ToString()) - 1] = int.Parse(ResultTable[4, row].Value.ToString()) - LastWorkTime[int.Parse(ResultTable[3, row].Value.ToString()) - 1];
                LastWorkTime[int.Parse(ResultTable[3, row].Value.ToString()) - 1] = int.Parse(ResultTable[6, row].Value.ToString());
                //
                ServiceTimeOfServer[int.Parse(ResultTable[3, row].Value.ToString()) - 1] += int.Parse(ResultTable[5, row].Value.ToString());
                ServiceCustomer[int.Parse(ResultTable[3, row].Value.ToString()) - 1]++;

                //
                CustomersQueueGraph.Series["Series1"].Points.AddXY(row, int.Parse(ResultTable[7, row].Value.ToString()));

                //
                if (MyDictionary.ContainsKey(int.Parse(ResultTable[7, row].Value.ToString())))
                    MyDictionary[int.Parse(ResultTable[7, row].Value.ToString())]++;
                else
                    MyDictionary[int.Parse(ResultTable[7, row].Value.ToString())] = 1;
                /////

                for(int s= int.Parse(ResultTable[4, row].Value.ToString()) ; s< int.Parse(ResultTable[6, row].Value.ToString()) ; s++)
                {
                    ServerWork[s, int.Parse(ResultTable[3, row].Value.ToString()) - 1] = true;
                }

            }
            
            AverageWaitingTimeInTheQueue = (double)WaitingTimeInTheQueue / (double)NoOfCustomer;
            ProbabilityThatCustomerWaitInTheQueue = (double)CustomerWaitInTheQueue / (double)NoOfCustomer;


            string OUT = "";
            OUT = "Average Waiting Time In The Queue = " + AverageWaitingTimeInTheQueue.ToString() + "\n";
            OUT += "Probability ThatCustomer Wait In The Queue = " + ProbabilityThatCustomerWaitInTheQueue.ToString() + "\n";
            

            for(int i=0 ; i<NoOfService ; i++)
            {
                ProbIdelTime[i] = (double)IdleTimeOfServer[i] / (double)TotalServTime;
                OUT += "Probability Idel Server (" + (i + 1).ToString() + ") = " + ProbIdelTime[i].ToString() + "\n";
                //
                AverageServiceTime[i] = (double)ServiceTimeOfServer[i] / (double)ServiceCustomer[i];
                OUT += "Average Server Time (" + (i + 1).ToString() + ") = " + AverageServiceTime[i].ToString() + "\n";
            }

            MaximumQueueLength = 0;//

            OUT += "Maximum queue length = " + MaximumQueueLength.ToString() + "\n";

            //MessageBox.Show(OUT);
            listView.Items.Clear();
            listView.Items.Add(OUT);
            foreach(var x in MyDictionary)
            {
                QueueSizeHistogram.Series["Series1"].Points.AddXY(x.Key, x.Value);
            }

            for (int x = 0; x <= TotalServTime; x++)
            {
                BusyTime1.Series["Series1"].Points.AddXY(x, ServerWork[x,0]);
                BusyTime2.Series["Series1"].Points.AddXY(x, ServerWork[x,1]);
            }

        }
    }
}
